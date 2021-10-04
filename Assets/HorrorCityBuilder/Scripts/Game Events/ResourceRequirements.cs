using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Requirements", menuName = "Create New Resource Requirements")]
public class ResourceRequirements : ScriptableObject
{
    public ResourceSystem.ResourceAndBuildingRecords resourceRequired;
    public int quantityNeeded;
}
