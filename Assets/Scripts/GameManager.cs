using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController playerController;
    public CutsceneManager cutsceneManager;
    public DialogManager dialogManager;
    public RoomManager roomManager;
    public Animator vfx,cfx;
    public AudioSource sfx;
    public TextReader reader;
    public GameObject gameOver;

    public List<AnimAsset> anims;
    public List<AudioAsset> sfxList;
    public List<InteractablesAsset> interactablesList;
    public List<EntityAsset> entityList;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
[Serializable]
public class AnimAsset
{
    public string nama;
    public Animator anim;
}

[Serializable]
public class AudioAsset
{
    public string nama;
    public AudioClip clip;
}

[Serializable]
public class InteractablesAsset
{
    public string nama;
    public Interactables interactables;
}

[Serializable]
public class PointAsset
{
    public string nama;
    public Transform point;
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
    public List<string> text;
}

[Serializable]
public class EntityAsset
{
    public string name;
    public Transform entity;
}