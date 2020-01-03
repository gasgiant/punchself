using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public BodyController bodyController;
    public HandController opponent;
    public List<SpriteRenderer> coloredParts;
    public SmoothSlider staminaSlider;
    public int direction;
    public Animator animator;

    public bool IsBuisy { get { return currentCommandRoutine != null; } }
    

    public bool blocking;

    public float stamina;
    float nextTimeCanRegen = 0;
    
    public HandState currentState;

    int commandQueueSize = 1;
    List<Command> commandQueue;
    Coroutine currentCommandRoutine = null;
    Coroutine flickRoutine = null;

    Color baseColor;

    public void Reset()
    {
        animator.SetBool("IsWin", false);
        animator.SetBool("IsLose", false);
        animator.SetBool("IsBlocking", false);
        blocking = false;
        stamina = GameManager.staminaParams.maxStamina;
    }

    public void SetFinAnimation(bool b)
    {
        if (b)
            animator.SetBool("IsWin", true);
        else
            animator.SetBool("IsLose", true);
    }

    private void Start()
    {
        commandQueue = new List<Command>();
        stamina = GameManager.staminaParams.maxStamina;
        currentState = HandState.Idle;
        baseColor = coloredParts[0].color;
    }

    private void Update()
    {
        HandleCommands();
        StaminaUpdate();
    }

    public void SendCommand(Command command)
    {
        EnqueueCommand(command);
    }

    void EnqueueCommand(Command command)
    {
        if (commandQueue.Count < commandQueueSize && stamina > 0)
        {
            commandQueue.Add(command);
        }
    }

    void HandleCommands()
    {
        if (commandQueue.Count > 0)
        {
            //if (commandQueue[0] != Command.Bite) blocking = false;

            switch (commandQueue[0])
            {
                case Command.Punch:
                    if (currentCommandRoutine == null && stamina > GameManager.staminaParams.punchCost / 2)
                    {
                        currentCommandRoutine = StartCoroutine(PunchRoutine());
                        commandQueue.RemoveAt(0);
                        DecrementStamina(GameManager.staminaParams.punchCost);
                    }
                    break;
                case Command.StartBlock:
                    if (currentCommandRoutine == null)
                    {
                        currentCommandRoutine = StartCoroutine(BlockRoutine());
                        commandQueue.RemoveAt(0);
                    }
                    break;
                case Command.StopBlock:
                    blocking = false;
                    commandQueue.RemoveAt(0);
                    break;
                case Command.Bite:
                    bodyController.StartBite();
                    commandQueue.RemoveAt(0);
                    break;
            }
        }
    }

    IEnumerator PunchRoutine()
    {
        currentState = HandState.Punching;
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(3.0f / 60);
        if (opponent.blocking)
        {
            CameraController.RandomShake(0.3f, 0.07f, 3, 0, 1);
            bodyController.TakePunch(direction, true);
            animator.SetTrigger("PunchBlocked");
            AudioManager.Instance.PlaySound("Block");
            opponent.animator.SetTrigger("BlockShake");
            yield return new WaitForSeconds(25.0f / 60);
        }
        else
        {
            yield return new WaitForSeconds(1.0f / 60);
            bodyController.TakePunch(direction, false);
            AudioManager.Instance.PlaySound("Punch");
            CameraController.RandomShake(1.5f, 0.07f, 3, 1.3f, 1);
            yield return new WaitForSeconds(23.0f / 60);
        }
        currentState = HandState.Idle;
        currentCommandRoutine = null;
    }

    IEnumerator BlockRoutine()
    {
        currentState = HandState.Blocking;
        animator.SetBool("IsBlocking", true);
        yield return new WaitForSeconds(0.02f);
        blocking = true;
        while (blocking)
        {
            if (bodyController.isBiting)
            {
                currentState = HandState.Bitten;
                blocking = false;
                currentCommandRoutine = StartCoroutine(BittenRoutine());
                break;
            }
            yield return null;
        }
        animator.SetBool("IsBlocking", false);
        if (currentState == HandState.Blocking)
        {
            yield return new WaitForSeconds(0.05f);
            currentState = HandState.Idle;
            currentCommandRoutine = null;
        }
    }

    IEnumerator BittenRoutine()
    {
        animator.SetTrigger("Bitten");
        Flick(Color.white, 0.1f, 5);
        yield return new WaitForSeconds(1f);
        currentState = HandState.Idle;
        currentCommandRoutine = null;
    }

    void DecrementStamina(float value)
    {
        stamina -= value;
        if (stamina < 0)
        {
            commandQueue.Clear();
            stamina = 0;
            nextTimeCanRegen = Time.time + GameManager.staminaParams.regenDelay;
        }
    }

    public void StaminaUpdate()
    {
        if (stamina < GameManager.staminaParams.maxStamina &&
            nextTimeCanRegen < Time.time)
        {
            if (currentState == HandState.Idle)
                stamina += GameManager.staminaParams.staminaRegen * Time.deltaTime;
            if (currentState == HandState.Blocking)
                stamina += GameManager.staminaParams.staminaRegen * Time.deltaTime;
        }
        staminaSlider.SetTargetValue(stamina / GameManager.staminaParams.maxStamina);
    }

    void Flick(Color color, float duration, int count)
    {
        if (flickRoutine == null)
            flickRoutine = StartCoroutine(FlickRoutine(color, duration, count));
    }

    IEnumerator FlickRoutine(Color color, float duration, int count)
    {
        float t;

        for (int i = 0; i < count; i++)
        {
            t = 0;
            while (t < 1)
            {
                SetColor(Color.Lerp(baseColor, color, t));
                t += Time.deltaTime / duration;
                yield return null;
            }
            t = 1;
            while (t > 0)
            {
                SetColor(Color.Lerp(baseColor, color, t));
                t -= Time.deltaTime / duration;
                yield return null;
            }
            SetColor(baseColor);
        }
        flickRoutine = null;
    }

    void SetColor(Color color)
    {
        foreach (var item in coloredParts)
        {
            item.color = color;
        }
    }
}

public enum HandState { Idle, Punching, Blocking, Bitten};
public enum Command { Punch, StartBlock, StopBlock, Bite };
