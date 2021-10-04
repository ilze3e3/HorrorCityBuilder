using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Store Object", menuName = "Create new store object")]
public class StoreSlots : ScriptableObject
{
    public GameObject model;
    public Tile tile;
    public int price;
    public Sprite buildingImage;
}
