using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class RandomEventHandler : MonoBehaviour
{
    public enum EventType
    {
        Daily,
        Monthly,
        Yearly,
    }
    public DayNightCycle dayNightCycle;
    public ResourceSystem resourceSystem;

    public List<GameEvent> dailyEvents;
    public List<GameEvent> monthlyEvents;
    public List<GameEvent> yearlyEvents;

    UnityEvent dayTrigger;
    UnityEvent monthTrigger;
    UnityEvent yearlyTrigger;

    public GameObject decisionBtnPrefab;
    public GameObject eventPanel;
    public TextMeshProUGUI eventTitleText;
    public TextMeshProUGUI eventNarrativeText;
    public GameObject decisionBtnParent;

    private int chosenDailyGameEventIndex = -1;
    private int chosenMonthlyGameEventIndex = -1;
    private int chosenYearlyGameEventIndex = -1;

    UnityEvent changeDailyEvent;
    UnityEvent changeMonthlyEvent;
    UnityEvent changeYearlyEvent;

    private void Awake()
    {
        dayTrigger = dayNightCycle.dayChange;
        monthTrigger = dayNightCycle.monthChange;
        yearlyTrigger = dayNightCycle.yearChange;

        changeDailyEvent = new UnityEvent();
        changeMonthlyEvent = new UnityEvent();
        changeYearlyEvent = new UnityEvent();
        if (dailyEvents.Count != 0)
        {
            changeDailyEvent.AddListener(delegate { ChooseWhichEventHappens(EventType.Daily); });
            changeDailyEvent.Invoke();
        }
        if (monthlyEvents.Count != 0)
        {
            changeMonthlyEvent.AddListener(delegate { ChooseWhichEventHappens(EventType.Monthly); });
            changeMonthlyEvent.Invoke();
        }
        if (yearlyEvents.Count != 0)
        {
            changeYearlyEvent.AddListener(delegate { ChooseWhichEventHappens(EventType.Yearly); });
            changeYearlyEvent.Invoke();
        }
        
    }
    private void Start()
    {
        if (dailyEvents.Count != 0)
        {
            dayTrigger.AddListener(delegate { TriggerEvent(dailyEvents[chosenDailyGameEventIndex]); });
            dayTrigger.AddListener(delegate { DailyEventTriggered(); });
            dayTrigger.AddListener(delegate { changeDailyEvent.Invoke(); });
        }
        if (monthlyEvents.Count != 0)
        {
            monthTrigger.AddListener(delegate { TriggerEvent(monthlyEvents[chosenMonthlyGameEventIndex]); });
            monthTrigger.AddListener(delegate { MonthlyEventTriggered(); });
            monthTrigger.AddListener(delegate { changeMonthlyEvent.Invoke(); });
        }
        if (yearlyEvents.Count != 0)
        {
            yearlyTrigger.AddListener(delegate { TriggerEvent(yearlyEvents[chosenYearlyGameEventIndex]); });
            yearlyTrigger.AddListener(delegate { YearlyEventTriggered(); });
            yearlyTrigger.AddListener(delegate { changeYearlyEvent.Invoke(); });
        }
    }

    public void TriggerEvent(GameEvent e)
    {
        Debug.Log("Req count is: " + e.eventRequirements.Count);
        if (!resourceSystem.CheckRequirements(e.eventRequirements)) return;
        eventTitleText.text = e.title;
        eventNarrativeText.text = e.narrative;
        dayNightCycle.PauseGame();

        foreach(EventDecision _eventDecision in e.decisions)
        {
            GameObject btnObject = Instantiate(decisionBtnPrefab, decisionBtnParent.transform);
            TextMeshProUGUI[] allTextInBtn = btnObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(TextMeshProUGUI t in allTextInBtn)
            {
                if (t.name.Contains("DecisionNarrative"))
                {
                    t.text = _eventDecision.decisionNarrative;
                }
                if(t.name.Contains("DecisionEffects"))
                {
                    string tmp = "Effects:\n";
                    foreach(EffectsToResources _effects in _eventDecision.decisionEffects)
                    {
                        tmp += "- " + _effects.affectedResourceType.ToString() + " (" + _effects.affectedQuantity + ")\n"; 
                    }
                    t.text = tmp;
                }
            }

            Button btn = btnObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate { resourceSystem.ApplyResourceEffects(_eventDecision.decisionEffects); });
            btn.onClick.AddListener(delegate { eventPanel.SetActive(false); });
            btn.onClick.AddListener(delegate { dayNightCycle.UnPauseGame(); });
            btn.onClick.AddListener(delegate { DestroyDecisionButtons(); });
            if(!resourceSystem.CheckRequirements(_eventDecision.decisionRequirements))
            {
                btn.interactable = false;
                btnObject.GetComponent<Image>().color = Color.red;
            }
        }
        eventPanel.SetActive(true);
    }
    private void DestroyDecisionButtons()
    {
        for(int i = 0; i < decisionBtnParent.transform.childCount; i++)
        {
            Destroy(decisionBtnParent.transform.GetChild(i).gameObject);
        }
    }
    public void ChooseWhichEventHappens(EventType e)
    {
        switch(e)
        {
            case EventType.Daily:
                if (chosenDailyGameEventIndex == -1)
                {
                    List<int> unusedIndexes = new List<int>();
                    for (int i = 0; i < dailyEvents.Count; i++)
                    {
                        if (i != chosenDailyGameEventIndex)
                        {
                            unusedIndexes.Add(i);
                        }
                    }

                    chosenDailyGameEventIndex = unusedIndexes[UnityEngine.Random.Range(0, unusedIndexes.Count)];
                    Debug.Log("Daily Index: " + chosenDailyGameEventIndex);
                }
            break;
            case EventType.Monthly:
                if (chosenMonthlyGameEventIndex == -1)
                {
                    List<int> unusedIndexes = new List<int>();
                    for (int i = 0; i < monthlyEvents.Count; i++)
                    {
                        if (i != chosenMonthlyGameEventIndex)
                        {
                            unusedIndexes.Add(i);
                            Debug.Log("Index: " + i + " has not been used in monthly event");
                        }
                    }
                    
                    chosenMonthlyGameEventIndex = unusedIndexes[UnityEngine.Random.Range(0, unusedIndexes.Count)];
                    Debug.Log("Chosen Monthly Index: " + chosenMonthlyGameEventIndex);
                }
            break;
            case EventType.Yearly:
                if (chosenYearlyGameEventIndex == -1)
                {
                    List<int> unusedIndexes = new List<int>();
                    for (int i = 0; i < yearlyEvents.Count; i++)
                    {
                        if (i != chosenYearlyGameEventIndex)
                        {
                            unusedIndexes.Add(i);
                        }
                    }

                    chosenYearlyGameEventIndex = unusedIndexes[UnityEngine.Random.Range(0, unusedIndexes.Count)];
                    Debug.Log(chosenYearlyGameEventIndex);
                }
            break;
        }
    }
    private void DailyEventTriggered()
    {
        chosenDailyGameEventIndex = -1;
    }
    private void MonthlyEventTriggered()
    {
        chosenMonthlyGameEventIndex = -1;
    }
    private void YearlyEventTriggered()
    {
        chosenYearlyGameEventIndex = -1;
    }
}
