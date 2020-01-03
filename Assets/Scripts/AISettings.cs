using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI settings", menuName = "AI Settings")]
public class AISettings : ScriptableObject
{
    //public bool enableAttacks;
    [Range(0, 1)]
    public float biteFollowUpChance;

    [Range(0, 1)]
    public float biteAttemptChance;

    [Range(0, 1)]
    public float biteEvadeChance;

    [Range(0, 1)]
    public float blockIntoPunchChance;

    [Range(0, 1)]
    public float idleChance;

    [Range(0, 1)]
    public float punchChance;

    

    public float maxDefDel;
    public float minDefDel;

    public float maxBlockDur;
    public float minBlockDur;

}
