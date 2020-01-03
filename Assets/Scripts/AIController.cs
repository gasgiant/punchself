using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool doSomething = true;
    public AISettings settings;
    HandController hand;

    public float nextTimeCanBeActive;
    public float timeToStopBlock;

    public float time;

    float lastTimeActivatedBite;


    private void Start()
    {
        hand = GetComponent<HandController>();
    }

    void SendCommand(Command command)
    {
        //Debug.Log(command);
        hand.SendCommand(command);
    }

    void Update()
    {
        time = Time.time;
        MakeDecision();
    }

    public void MakeDecision()
    {
        if (!doSomething) return;

        if (hand.blocking && Time.time > timeToStopBlock)
        {
            SetDelay(DefaultDelay);
            SendCommand(Command.StopBlock);
            return;
        }

        if (Time.time < nextTimeCanBeActive) return;

        if (hand.blocking && hand.bodyController.InBiteProcess)
        {
            SetDelay(DefaultDelay);
            if (Random.value < settings.biteEvadeChance)
            {
                SendCommand(Command.StopBlock);
                return;
            }
        }


        if (hand.opponent.currentState == HandState.Bitten)
        {
            SetDelay(DefaultDelay);
            if (Random.value < settings.biteFollowUpChance)
            {
                SendCommand(Command.Punch);
                return;
            }
        }

        if (hand.opponent.currentState == HandState.Blocking)
        {
            SetDelay(DefaultDelay);
            if (Random.value < settings.biteAttemptChance)
            {
                SendCommand(Command.Bite);
                lastTimeActivatedBite = Time.time;
                return;
            }
        }

        if (Random.value < settings.idleChance)
        {
            SetDelay(DefaultDelay);
            return;
        }

        if (hand.currentState != HandState.Blocking)
        {
            if ((Random.value < settings.punchChance || hand.stamina > GameManager.staminaParams.maxStamina * 0.7f)
                && Time.time - lastTimeActivatedBite > 0.5f)
            {
                SetDelay(DefaultDelay);
                SendCommand(Command.Punch);
                return;
            }
            
            if (!hand.bodyController.InBiteProcess)
            {
                SetDelay(DefaultDelay);
                SendCommand(Command.StartBlock);
                timeToStopBlock = Time.time + Random.Range(settings.minBlockDur, settings.maxBlockDur);
                return;
            }

            
        }
    }

    public void SetDelay(float delay)
    {
        nextTimeCanBeActive = Time.time + delay;
    }

    public float DefaultDelay {get { return Random.Range(settings.minDefDel, settings.maxDefDel); } }
}
