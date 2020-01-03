using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isActive = true;
    HandController hand;
    public string punchButton;
    public string blockButton;
    public string biteButton;

    private void Start()
    {
        hand = GetComponent<HandController>();
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(punchButton))
        {
            hand.SendCommand(Command.Punch);
        }

        if (Input.GetKeyDown(biteButton))
        {
            hand.SendCommand(Command.Bite);
        }

        if (Input.GetKeyDown(blockButton))
        {
            hand.SendCommand(Command.StartBlock);
        }

        if (!Input.GetKey(blockButton))
        {
            if (hand.blocking)
                hand.SendCommand(Command.StopBlock);
        }
    }
}
