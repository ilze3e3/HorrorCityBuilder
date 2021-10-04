using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public float defaultLengthOfDayInRealTime; // Normal time
    public float modifiedLengthOfDayInRealTime; // Fast forwarded time
    public float durationOfDayRealtimeSeconds;
    public int daysInAMonth;
    public int monthsInAYear;
    [Tooltip("Enter starting date in the form of [dd][mm][yyyy]")] public Vector3Int startingDate;
    [Tooltip("Current date starting from starting date")] public Vector3Int currentDate;
    public UnityEvent dayChange;
    public UnityEvent monthChange;
    public UnityEvent yearChange;
    public bool pause;
    public bool fastForward;
    float timeStart;

    public TextMeshProUGUI dateText;

    IEnumerator TickTock()
    {

        while(true)
        {
            if(pause)
            {
                timeStart = Time.time;
            }
            
            if ((Time.time - timeStart) >= durationOfDayRealtimeSeconds)
            {
                Debug.Log("Day Change");
                timeStart = Time.time;
                // Change the day and the date
                ChangeDate();
            }
            yield return null;
        }
    }

    private void Awake()
    {
        currentDate = startingDate;
        if (dayChange == null) dayChange = new UnityEvent();
        if (monthChange == null) monthChange = new UnityEvent();
        if (yearChange == null) yearChange = new UnityEvent();
    }
    private void Start()
    {
        StartCoroutine("TickTock");
    }
    private void Update()
    {
        /// If button press to fast forward
        /// lengthOfDayInRealTime /= 2 (Divide 2 to make it run twice quicker)
        /// lengthOfDay = modifiedLengthOfDayInRealtime

        /// If button press to play
        /// lengthOfDayInRealTime goes back to normal time. 
        /// lengthOfDay = defaultLengthOfDayInRealtime
        
        if(pause)
        {
            Time.timeScale = 0;
        }
        if(!pause)
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Space)) pause = true;
    }

    private void ChangeDate()
    {
        currentDate.x++;
        if(currentDate.x > daysInAMonth)
        {
            currentDate.x = 1;
            currentDate.y++;
            if(currentDate.y > monthsInAYear)
            {
                currentDate.y = 1;
                currentDate.z++;
                yearChange.Invoke();
            }
            else
            {
                monthChange.Invoke();
            }
        }
        else
        {
            dayChange.Invoke();
        }

        UpdateText();
    }

    private void UpdateText()
    {
        dateText.text = currentDate.x + "/" + currentDate.y + "/" + currentDate.z; 
    }

    public void PauseGame() => pause = true;
    public void UnPauseGame() => pause = false;



}
