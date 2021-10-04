using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Game Event", menuName = "Create New Game Event")]
public class GameEvent : ScriptableObject
{
    public string title;
    [TextAreaAttribute(15,20)]
    public string narrative;
    public List<EventDecision> decisions;
    public List<ResourceRequirements> eventRequirements;
}
