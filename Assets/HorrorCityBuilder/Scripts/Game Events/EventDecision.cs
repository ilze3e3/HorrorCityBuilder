using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Decision", menuName = "Create New Decision")]
public class EventDecision : ScriptableObject
{
    [TextAreaAttribute(15,20)]
    public string decisionNarrative;
    public List<EffectsToResources> decisionEffects;
    public List<ResourceRequirements> decisionRequirements;
}
