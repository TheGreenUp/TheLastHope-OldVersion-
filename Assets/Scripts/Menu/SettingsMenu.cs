using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SettingsMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject settingsMenu;
    public void EditForestSound()
    {

    }
    public void EditFootStepsSound()
    {
        
    }
    public void ExitToMainMenu()
    {
        settingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }
}

