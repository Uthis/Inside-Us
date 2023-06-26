using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField] Vector2 padding;
    [Range(1f,5f)]
    [SerializeField] float textSpeed = 1f;

    public bool isShowing;

    SpriteRenderer background;
    TextMeshPro tmp;

    string[] textToWrite;
    string currText;
    int charIndex,textIndex;
    float timePerChar, timer;
    bool isWriting;

    private void Awake()
    {
        background = transform.Find("BG").GetComponent<SpriteRenderer>();
        tmp = transform.Find("Text").GetComponent<TextMeshPro>();
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

    public void StartDialogue(string[] text)
    {
        background.enabled = true;
        tmp.enabled = true;
        textToWrite = text;
        isShowing = true;
        textIndex = -1;
        Next();
    }

    void SetText()
    {
        currText = textToWrite[textIndex];
        tmp.SetText("<color=#00000000>"+currText + "</color>");
        tmp.ForceMeshUpdate();
        Vector2 textSize = tmp.GetRenderedValues(false);
        background.size = textSize + padding;
        timePerChar = 1/(textSpeed * 10);
        charIndex = 0;
        isWriting = true;
    }

    public bool Next()
    {
        if (isWriting)
            timePerChar = 0;
        else if(textIndex < textToWrite.Length - 1)
        {
            textIndex++;
            SetText();
        }
        else
        {
            background.enabled = false;
            tmp.enabled = false;
            textToWrite = null;
            isShowing = false;
            return true;
        }
        return false;
    }
}
