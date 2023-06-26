using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
