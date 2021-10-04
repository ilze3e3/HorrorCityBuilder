using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    // Start is called before the first frame 

    public List<StoreSlots> allAvailableBuildings = new List<StoreSlots>();
    public GameObject storeButtonParent;
    public PlacementScript placementFunction;
    public GameObject storeButtonPrefab;
    public ResourceSystem resources;
    

    void Start()
    {
        InitialiseShop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyListener(StoreSlots building)
    {
        if (building.price <= resources.currentMoney)
        {
            Debug.Log("BuyListener triggered");           
            placementFunction.PlaceBuilding(building);
        }        
    }

    public void InitialiseShop()
    {
        foreach(StoreSlots s in allAvailableBuildings)
        {
            GameObject button = Instantiate(storeButtonPrefab, storeButtonParent.transform);
            TextMeshProUGUI[] allTextComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(TextMeshProUGUI t in allTextComponents)
            {
                if(t.name.Contains("Name"))
                {
                    t.text = s.tile.tileName;
                }
                else if(t.name.Contains("Price")) 
                {
                    t.text = s.price.ToString();
                }
                else if(t.name.Contains("Effects"))
                {
                    string effects = "Effects: \n";
                    foreach(EffectsToResources tileEffect in s.tile.tileEffects)
                    {
                        string sign = "";
                        if (tileEffect.affectedQuantity > 0) sign = "+";
                        effects += sign + tileEffect.affectedQuantity + " " + tileEffect.affectedResourceType.ToString() + "\n";
                    }

                    t.text = effects;
                }
            }
            //TODO: Remove comment when sprites are added 
            //button.GetComponentInChildren<Image>().sprite = s.buildingImage;
            button.GetComponent<Button>().onClick.AddListener(delegate {BuyListener(s); });
        }
    }
    
}
