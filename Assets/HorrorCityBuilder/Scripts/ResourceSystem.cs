using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ResourceSystem : MonoBehaviour
{
    [SerializeField] public enum ResourceAndBuildingRecords
    {
        Population,
        InjuredPopulation,
        Money, 
        Horror,
        AvailableHousing,
        HealingRate,
        Grass,
        Forest,
        House,
        Hospital,
        GarlicLampPost,
        DreamCatcherHouse,
        Cemetery,
        TownHall,
        Lamp,
    };
    #region Population
    public float currentPop;
    [SerializeField] float maxiumPopGrowthChance; // The highest level of population growth if horror is 0.
    [SerializeField] float currentGrowthChance; // Current population growth. F = maximumPopGrowthMultiplier - minimumHorrorMultiplier (possibly * by some other multiplier to slow it down.)
    [SerializeField] float popGrowthMultiplier; // How much pop is added if it does go up. 
    [SerializeField] float startingPop;
    [SerializeField] int availableHousing;
    public float injuredPop;
    [SerializeField, Tooltip("Multiplier that affects healing per tick to balance it out.")] float popHealingMultiplier;
    [SerializeField, Tooltip("Injured Pops healed per day. Affected by healing buildings. E.g. Hospitals")] float popHealingPerTick;
    #endregion

    #region Horror
    public float horrorLevel;
    [SerializeField] float maxHorrorMultiplierForPopGrowth; // The minimum horror level for population growth. The higher the horror the less pop there is
    #endregion

    #region Money
    public float currentMoney;
    [SerializeField] float startingMoney;
    [SerializeField] float taxMultipler;
    #endregion

    public DayNightCycle dayNightCycle;

    public TextMeshProUGUI popText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI horrorText;
    public TextMeshProUGUI housingText;

    Dictionary<ResourceAndBuildingRecords, List<Transform>> tileRecords = new Dictionary<ResourceAndBuildingRecords, List<Transform>>();

    private void Start()
    {
        dayNightCycle.monthChange.AddListener(TaxTime);
        dayNightCycle.monthChange.AddListener(PopulationGrowth);
        dayNightCycle.monthChange.AddListener(ListOutBuildingRecords);
        currentMoney = startingMoney;

        UpdateText();
    }
    #region Event Listeners
    private void TaxTime() // Gain money per month
    {
        float moneyToGain = (currentPop - injuredPop) * taxMultipler;
        currentMoney += moneyToGain;
    }

    private void PopulationGrowth() // Gain population per month
    {
        if(horrorLevel <= maxHorrorMultiplierForPopGrowth)
        {
            currentGrowthChance = maxiumPopGrowthChance - (horrorLevel / 50);
        }
        else
        {
            currentGrowthChance = 1;
        }
        Debug.Log("Rolling for Pop Growth");
        if (currentPop < availableHousing)
        {
            
            //Chance to gain population
            int chance = Mathf.CeilToInt(currentGrowthChance);
            if (chance > 100) chance = 100;
            if (ChanceSystem.PercentChance(chance)) // if true then add new population
            {
                Debug.Log("Pop Added");
                currentPop += popGrowthMultiplier;
                if(currentPop >= availableHousing)
                {
                    currentPop = availableHousing;
                }
                UpdateText();
            }
        }
    }

    private void HealingInjuredPopulation()
    {
        if (injuredPop > 0)
        {
            injuredPop -= (popHealingPerTick * popHealingMultiplier);
            injuredPop = Mathf.Round(injuredPop);
            if (injuredPop < 0) injuredPop = 0;
        }
    }

    private void ListOutBuildingRecords()
    {
        string recordString = "";
        foreach(ResourceAndBuildingRecords r in Enum.GetValues(typeof(ResourceAndBuildingRecords)))
        {
            if(tileRecords.ContainsKey(r))
            {
                recordString += r.ToString() + ": " + tileRecords[r].Count + "\n";
            }
        }

        Debug.Log(recordString);
    }
    #endregion

    #region Setter Helper Function
    public void AddHousing(int _housingQuantity)
    {
        availableHousing += _housingQuantity;
    }

    public void LowerHousing(int _housingQuantity)
    {
        availableHousing -= _housingQuantity; 
    }
    public void AddHorror(float _horror)
    {
        horrorLevel += _horror;
    }

    public void ReduceHorror(float _horror)
    {
        horrorLevel -= _horror;
    }
    #endregion

    public void ApplyResourceEffects(List<EffectsToResources> _effects)
    {
        //Debug.Log("Apply Building Effects Invoked");
        foreach (EffectsToResources t in _effects)
        {
            Debug.Log("Affected Resources:" + t.affectedResourceType.ToString());
            switch(t.affectedResourceType)
            {
                case ResourceAndBuildingRecords.AvailableHousing:
                    
                    availableHousing += Mathf.RoundToInt(t.affectedQuantity);
                    break;
                case ResourceAndBuildingRecords.Population:
                    currentPop += t.affectedQuantity;
                    if (currentPop > availableHousing) currentPop = availableHousing;
                    if (currentPop < 0) currentPop = 0;
                    break;
                case ResourceAndBuildingRecords.Horror:
                    horrorLevel += t.affectedQuantity;
                    if (horrorLevel < 0) horrorLevel = 0;
                    break;
                case ResourceAndBuildingRecords.Money:
                    currentMoney += t.affectedQuantity;
                    if (currentMoney < 0) currentMoney = 0;
                    break;
                case ResourceAndBuildingRecords.HealingRate:
                    popHealingPerTick += t.affectedQuantity;
                    if (popHealingPerTick < 0) popHealingPerTick = 0;
                    break;
                case ResourceAndBuildingRecords.InjuredPopulation:
                    injuredPop += t.affectedQuantity;
                    if (injuredPop < 0) injuredPop = 0;
                    if (injuredPop > currentPop) injuredPop = currentPop;
                    break;
            }
        }

        UpdateText();
    }

    public void RemoveResourceEffects(List<EffectsToResources> _effects)
    {
        foreach (EffectsToResources t in _effects)
        {
            Debug.Log("Affected Resources:" + t.affectedResourceType.ToString());
            switch (t.affectedResourceType)
            {
                case ResourceAndBuildingRecords.AvailableHousing:

                    availableHousing -= Mathf.RoundToInt(t.affectedQuantity);
                    if (availableHousing < 0) availableHousing = 0;
                    if (currentPop > availableHousing) currentPop = availableHousing;
                    if (injuredPop > currentPop) injuredPop = currentPop;
                    break;
                case ResourceAndBuildingRecords.Population:
                    currentPop -= t.affectedQuantity;
                    if (currentPop > availableHousing) currentPop = availableHousing;
                    if (injuredPop > currentPop) injuredPop = currentPop;
                    if (currentPop < 0) currentPop = 0;
                    break;
                case ResourceAndBuildingRecords.Horror:
                    horrorLevel -= t.affectedQuantity;
                    if (horrorLevel < 0) horrorLevel = 0;
                    break;
                case ResourceAndBuildingRecords.Money:
                    currentMoney -= t.affectedQuantity;
                    if (currentMoney < 0) currentMoney = 0;
                    break;
                case ResourceAndBuildingRecords.HealingRate:
                    popHealingPerTick -= t.affectedQuantity;
                    if (popHealingPerTick < 0) popHealingPerTick = 0;
                    break;
                case ResourceAndBuildingRecords.InjuredPopulation:
                    injuredPop -= t.affectedQuantity;
                    if (injuredPop < 0) injuredPop = 0;
                    if (injuredPop > currentPop) injuredPop = currentPop;
                    break;
            }
        }
        UpdateText();
    }

    public void DeductMoney(int _deductedAmount)
    {
        Debug.Log("DeductMoneyInvoked");
        currentMoney -= _deductedAmount;
        if(currentMoney < 0)
        {
            currentMoney = 0;
        }
        UpdateText();
    }

    private void UpdateText()
    {
        popText.text = "Pop: " + Mathf.RoundToInt(currentPop).ToString() + "(" + Mathf.RoundToInt(injuredPop).ToString() + ")";
        moneyText.text = "Money: $" + Mathf.FloorToInt(currentMoney).ToString();
        horrorText.text = "Horror: " + horrorLevel.ToString();
        housingText.text = "Available Housing: " + availableHousing.ToString();
    }

    public void InsertTileRecord(Tile _tile, Transform _transform)
    {
        ResourceAndBuildingRecords buildingRecords = _tile.tileType;
        if(!tileRecords.ContainsKey(buildingRecords)) 
        {
            tileRecords.Add(buildingRecords, new List<Transform>());
        }
        tileRecords[buildingRecords].Add(_transform);
    }
    public void RemoveTileRecord(Tile _tile, Transform _transform)
    {
        ResourceAndBuildingRecords buildingRecords = _tile.tileType;
        if (!tileRecords.ContainsKey(buildingRecords))
        {
            Debug.LogError("List of Transform for: " + buildingRecords.ToString());
        }
        tileRecords[buildingRecords].Remove(_transform);
    }

    public bool CheckRequirements(List<ResourceRequirements> _reqs)
    {
        if (_reqs.Count == 0)
        {
            Debug.Log("Reqs is 0 but it's: " + _reqs.Count);
            return true;
        }
        bool status = true;
        foreach(ResourceRequirements r in _reqs)
        {
            switch(r.resourceRequired)
            {
                #region Resources
                case ResourceAndBuildingRecords.AvailableHousing:
                    if (availableHousing < r.quantityNeeded) status = false;
                    break;
                case ResourceAndBuildingRecords.Money:
                    if (currentMoney < r.quantityNeeded) status = false;
                    break;
                case ResourceAndBuildingRecords.Population:
                    if (currentPop < r.quantityNeeded)
                    {
                        Debug.Log("CurrentPop: " + currentPop + "/ Needed Pop: " + r.quantityNeeded);
                        status = false;
                    }
                    break;
                case ResourceAndBuildingRecords.Horror:
                    if (horrorLevel < r.quantityNeeded) status = false;
                    break;
                case ResourceAndBuildingRecords.InjuredPopulation:
                    if ((injuredPop + r.quantityNeeded) > currentPop) status = false;
                    break;
                #endregion
                #region Environmental & Building Tile
                default:
                    if (tileRecords[r.resourceRequired].Count < r.quantityNeeded) status = false;
                    break;
                #endregion
            }
        }
        return status;
    }
}
