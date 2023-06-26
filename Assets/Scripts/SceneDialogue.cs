using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneDialogue : MonoBehaviour
{
    [SerializeField] Vector2 padding;
    [Range(1f, 5f)]
    [SerializeField] float textSpeed = 1f;

    public bool isShowing;

    TextMeshProUGUI tmp;
    Image img,bg;
    CutsceneAsset cutscene;

    string[] textToWrite;
    string currText;
    int charIndex, textIndex,cutsceneIndex;
    float timePerChar, timer;
    bool isWriting;

    private void Awake()
    {
        tmp = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        img = GetComponent<Image>();
        bg = transform.Find("BG").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWriting) return;

        timer -= Time.deltaTime;
        while (timer <= 0)
        {
            timer += timePerChar;
            charIndex++;
            string text = currText.Substring(0, charIndex);
            text += "<color=#00000000>" + currText.Substring(charIndex) + "</color>";
            tmp.text = text;

            if (charIndex >= currText.Length)
            {
                isWriting = false;
                return;
            }
        }
    }

    public void StartScene(CutsceneAsset scene)
    {
        cutscene = scene;
        textToWrite = new string[]{ };
        isShowing = true;
        cutsceneIndex = -1;
        Next();
    }

    void SetText()
    {
        currText = textToWrite[textIndex];
        tmp.SetText("<color=#00000000>" + currText + "</color>");
        tmp.ForceMeshUpdate();
        bg.enabled = true;
        tmp.enabled = true;
        timePerChar = 1 / (textSpeed * 10);
        charIndex = 0;
        isWriting = true;
    }

    void SetScene()
    {
        CutsceneAction currAction = cutscene.actions[cutsceneIndex];
        img.sprite = currAction.image;
        img.enabled = true;
        textToWrite = currAction.text.ToArray();
        textIndex = 0;
        SetText();
    }

    public bool Next()
    {
        if (isWriting)
            timePerChar = 0;
        else if (textIndex < textToWrite.Length - 1)
        {
            textIndex++;
            SetText();
        }
        else if (cutsceneIndex < cutscene.actions.Count - 1)
        {
            cutsceneIndex++;
            SetScene();
        }
        else
        {
            img.enabled = false;
            bg.enabled = false;
            tmp.enabled = false;
            textToWrite = null;
            isShowing = false;
            return true;
        }
        return false;
    }
}
