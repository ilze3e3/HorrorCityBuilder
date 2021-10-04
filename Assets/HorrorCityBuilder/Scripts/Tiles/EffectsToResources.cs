using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Effect", menuName = "Create new effects")]
public class EffectsToResources : ScriptableObject
{
    public ResourceSystem.ResourceAndBuildingRecords affectedResourceType;
    public float affectedQuantity;
}
