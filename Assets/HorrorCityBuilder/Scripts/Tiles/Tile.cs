using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Tile", menuName ="Create New Tile")]
public class Tile : ScriptableObject
{
    public string tileName;
    public ResourceSystem.ResourceAndBuildingRecords tileType;
    public List<EffectsToResources> tileEffects;
}
