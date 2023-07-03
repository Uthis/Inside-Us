using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    bool isChanging;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        int id = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(id);
    }

    public void ChangeLocale(int id)
    {
        if (isChanging) return;
        StartCoroutine(SetLocale(id));
    }

    IEnumerator SetLocale(int id)
    {
        isChanging = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        PlayerPrefs.SetInt("LocaleKey", id);
        isChanging = false;
    }
}