using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] List<LanguageAsset> texts;
    [SerializeField] TextAsset UIText;
    [SerializeField] string defaultLanguage = "en";

    string currLang;
    Dictionary<string, string> words,UIWords;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setLanguage(string key)
    {
        if (currLang == key) return;
        LanguageAsset text = texts.FirstOrDefault(x => x.nama == key);
        if (text != null)
        {
            currLang = key;
            ReadText(text.text);
        }
        else
        {
            Debug.Log("Failed to get language: " + key);
        }
    }

    void ReadText(TextAsset text)
    {

    }

    public string GetString(string key)
    {
        string res = "";
        if (!words.TryGetValue(key, out res))
            Debug.Log("Failed to get word: " + key);
        return res;
    }

    public string GetUIText(string key)
    {
        string res = "";
        if (!UIWords.TryGetValue(key, out res))
            Debug.Log("Failed to get word: " + key);
        return res;
    }

}

[Serializable]
public class LanguageAsset
{
    public string nama;
    public TextAsset text;
}