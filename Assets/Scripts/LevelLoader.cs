﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    GameObject menu_Canvas;
    GameObject settings_Canvas;

    void Start()
    {

        menu_Canvas = GameObject.Find("MainMenuCanvas");
        settings_Canvas = GameObject.Find("SettingsCanvas");

        settings_Canvas.SetActive(false);
        menu_Canvas.SetActive(true);
    }

    public void LoadScene(int a)
    {

        SceneManager.LoadScene(a);
    }

    public void Quit()
    {

        Application.Quit();
    }

    public void loadMenu()
    {

        menu_Canvas.SetActive(true);
        settings_Canvas.SetActive(false);
    }

    public void loadSettings()
    {

        menu_Canvas.SetActive(false);
        settings_Canvas.SetActive(true);
    }
}