using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Staminaparams staminaParams = new Staminaparams();

    
    public Transform bodyTransform;
    public float distance;

    public BodyController body;
    public PlayerController leftPlayer;
    public PlayerController rightPlayer;
    public AIController rightAI;

    public HandController leftHand;
    public HandController rightHand;

    public bool twoPlayers = false;

    Coroutine gameRoutine;
    UIManager uiManager;

    public SpriteSwitcher pauseButtonSwitcher;

    enum GameState { Normal, Pause, MainMenu }
    GameState currentState;

    private void Awake()
    {
        Instance = this;
        uiManager = GetComponent<UIManager>();
        ResetArena();
        currentState = GameState.MainMenu;
        uiManager.mainMenu.SetActive(true);
    }

    private void Start()
    {
        //AudioManager.Instance.PlayMenuMusic();
    }

    public void SetTwoPlayers(bool b)
    {
        twoPlayers = b;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(true);
        }
    }

    public void TogglePause(bool esc)
    {
        if (currentState == GameState.Normal)
        {
            Time.timeScale = 0;
            uiManager.pasueMenu.SetActive(true);
            currentState = GameState.Pause;
        }
        else
        {
            if (currentState == GameState.Pause)
            {
                Time.timeScale = 1;
                uiManager.pasueMenu.SetActive(false);
                currentState = GameState.Normal;
            }
        }
        if (esc)
        {
            pauseButtonSwitcher.SwitchSprite();
        }
    }

    public void RestartGame()
    {
        if (gameRoutine == null)
            StopCoroutine(GameRoutine());
        gameRoutine = StartCoroutine(GameRoutine());
    }

    public void ToMainMenu()
    {
        if (currentState == GameState.Pause)
            pauseButtonSwitcher.SwitchSprite();
        StopCoroutine(GameRoutine());
        ResetArena();
        uiManager.pasueMenu.SetActive(false);
        uiManager.mainMenu.SetActive(true);
        currentState = GameState.MainMenu;
        AudioManager.Instance.PlayMenuMusic();
    }

    void SetControlsActive(bool b)
    {
        leftPlayer.isActive = b;
        if (twoPlayers)
        {
            rightPlayer.isActive = b;
        }
        else
        {
            rightAI.doSomething = b;
        }

        if (!b)
            body.rb.velocity = Vector2.zero;
    }

    public void ResetArena()
    {
        uiManager.HideWinTexts();
        SetControlsActive(false);
        leftHand.Reset();
        rightHand.Reset();
        bodyTransform.position = new Vector2(0, bodyTransform.position.y);
    }

    IEnumerator GameRoutine()
    {
        uiManager.scoreText.SetScore(0, 0);
        //if (currentState == GameState.MainMenu)
        //    AudioManager.Instance.PlayBattleMusic();
        if (currentState == GameState.Pause)
            pauseButtonSwitcher.SwitchSprite();
        Time.timeScale = 1;
        uiManager.pasueMenu.SetActive(false);
        uiManager.mainMenu.SetActive(false);
        currentState = GameState.Normal;

        int leftScore = 0;
        int rightScore = 0;
        int roundNumber = 1;
        while (leftScore < 2 && rightScore < 2)
        {
            ResetArena();
            yield return uiManager.Announce("ROUND " + roundNumber);
            SetControlsActive(true);
            while (Mathf.Abs(bodyTransform.position.x) < distance + 5)
            {
                yield return null;
            }
            SetControlsActive(false);
            if (bodyTransform.position.x < 0)
            {
                rightScore += 1;
            }
            else
            {
                leftScore += 1;
            }
            yield return uiManager.scoreText.ShowScore(leftScore, rightScore);
            roundNumber += 1;
        }

        if (leftScore > 1)
        {
            leftHand.SetFinAnimation(true);
            rightHand.SetFinAnimation(false);
            uiManager.ShowWinTexts(true);
        }

        if (rightScore > 1)
        {
            leftHand.SetFinAnimation(false);
            rightHand.SetFinAnimation(true);
            uiManager.ShowWinTexts(false);
        }
        gameRoutine = null;
    }
    

}

public class Staminaparams
{
    public float maxStamina = 100;
    public float staminaRegen = 70;
    public float punchCost = 33;
    public float regenDelay = 1f;
}
