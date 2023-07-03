using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] SceneDialogue dialog;
    [SerializeField] List<CutsceneAsset> cutsceneList;

    public void Play(string name)
    {
        CutsceneAsset cutscene = cutsceneList.First(x => x.nama.ToLower().Equals(name.ToLower()));
        dialog.StartScene(cutscene);
    }
}

[Serializable]
public class CutsceneAsset
{
    public string nama;
    public List<CutsceneAction> actions;
}

[Serializable]
public class CutsceneAction
{
    public Sprite image;
    public List<LocalizedString> text;
}