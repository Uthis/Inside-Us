using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public List<DialogAsset> dialogs;

    Dialogue currDialog;
    public bool isShowingDialog;

    public void StartDialog(string dialogName, string[] dialog)
    {
        DialogAsset dialogAsset = dialogs.First(x => x.nama == dialogName);
        currDialog = dialogAsset.dialog;
        isShowingDialog = true;
        dialogAsset.dialog.StartDialogue(dialog);
    }

    public bool Next()
    {
        bool isDone = currDialog.Next();
        if (isDone)
        {
            isShowingDialog = false;
            currDialog = null;
        }
        return isDone;
    }
}

[Serializable]
public class DialogAsset
{
    public string nama;
    public Dialogue dialog;
}
