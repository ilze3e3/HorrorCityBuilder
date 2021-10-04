using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject panel;
    public DayNightCycle dayNightCycle;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!panel.activeSelf)
            {
                panel.SetActive(true);
                dayNightCycle.PauseGame();
            }
            else
            {
                panel.SetActive(false);
                dayNightCycle.UnPauseGame();
            }
        }
    }

    public void TurnOffPauseMenu()
    {
        panel.SetActive(false);
        dayNightCycle.UnPauseGame();
    }

    public void RestartGame()
    {
        panel.SetActive(false);
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
