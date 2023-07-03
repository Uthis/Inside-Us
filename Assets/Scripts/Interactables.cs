using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public enum InteractType
{
    Off,
    Dialog,
    Scene,
    Animation,
    Transport,
    AutoTrigger,
    Trigger,
    TriggerWithDialog,
}

[Serializable]
public class Interact
{
    public string name;
    public Sprite sprite;
    public InteractType type;
    public LocalizedString[] dialogTexts;
    public CutsceneAsset Cutscene;
    public string targetRoom,targetPos;

    public string[] GetDialogs()
    {
        return dialogTexts.Select(x => x.GetLocalizedString()).ToArray();
    }
}

public class Interactables : MonoBehaviour
{
    [SerializeField] GameObject btnInteract;
    [SerializeField] List<Interact> interactions;
    [SerializeField] GameObject triggerCol;
    [HideInInspector]
    public Interact currInteract;
    public Animator anim;
    public Transform standPoint;
    public bool isFaceRight,onStandPoint;

    private void Start()
    {
        SetInteract(0);
    }

    public void ShowInteract(bool isActive)
    {
        btnInteract.SetActive(isActive);
    }

    public void SetInteract(int index)
    {
        currInteract = interactions[index];
        if (currInteract.sprite != null) anim.GetComponent<SpriteRenderer>().sprite = currInteract.sprite;
        SetCollider(currInteract.type != InteractType.Off);
    }

    public void SetCollider(bool isActive)
    {
        triggerCol.gameObject.SetActive(isActive);
    }

    public InteractType GetInteractType()
    {
        return currInteract.type;
    }

    public Interact Interact()
    {
        if(currInteract.type == InteractType.Transport) anim.Play("OnInteract");
        return currInteract;
    }

    public void PlayAnim(string name)
    {
        anim.Play(name);
    }
}
