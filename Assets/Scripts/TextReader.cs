using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class TextReader : MonoBehaviour
{
    public delegate bool StorylineMethod(params object[] args);
    [SerializeField] LocalizedAsset<TextAsset> storyAsset;
    int indexLine = 0;
    List<KeyValuePair<StorylineMethod, object[]>> lines;
    Dictionary<string, bool> Triggers;
    bool isRead = false;

    private void Awake()
    {
        Triggers = new Dictionary<string, bool>();
    }

    private void Start()
    {
        indexLine = 0;
        lines = ReadFile();
        isRead = true;
    }

    private void FixedUpdate()
    {
        while (isRead)
        {
            ExecuteMethod();
            indexLine++;
        }
    }

    void ExecuteMethod()
    {
        bool res = lines[indexLine].Key.Invoke(lines[indexLine].Value);
    }

    public List<KeyValuePair<StorylineMethod, object[]>> ReadFile()
    {
        List<KeyValuePair<StorylineMethod, object[]>> storylines = new List<KeyValuePair<StorylineMethod, object[]>>();
        List<KeyValuePair<StorylineMethod, object[]>> togetherLines = new List<KeyValuePair<StorylineMethod, object[]>>();
        bool isTogether = false;

        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitLine = new char[] { ',' };
        TextAsset storyline = storyAsset.LoadAsset();
        var Lines = storyline.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Lines.Length; i++)
        {
            var line = Lines[i];
            if (line.StartsWith("["))
            {
                string command = line.Substring(1,line.IndexOf("]") - 1).ToLower();
                object[] content = new object[] { };
                StorylineMethod method = null;

                content = line.Length > line.IndexOf("]") + 2? line.Substring(line.IndexOf("]") + 2).Split(splitLine, System.StringSplitOptions.RemoveEmptyEntries):new object[] { };
                switch (command)
                {
                    case "together":
                        isTogether = true;
                        togetherLines = new List<KeyValuePair<StorylineMethod, object[]>>();
                        break;
                    case "end together":
                        isTogether = false;
                        method = Together;
                        content = new object[] { togetherLines.ToList() };
                        break;
                    case "lock control":
                        method = LockControl;
                        break;
                    case "full control":
                        method = FullControl;
                        break;
                    case "anim":
                        method = Anim;
                        break;
                    case "anim2":
                        method = AnimObject;
                        break;
                    case "vfx":
                        method = VFX;
                        break;
                    case "sfx":
                        method = SFX;
                        break;
                    case "dialog":
                        method = Dialog;
                        break;
                    case "state":
                        method = State;
                        break;
                    case "trigger":
                        method = Trigger;
                        break;
                    case "go":
                        method = Go;
                        break;
                    case "cutscene":
                        method = Cutscene;
                        break;
                    case "camera":
                        method = Camera;
                        break;
                    case "tele":
                        method = Tele;
                        break;
                    case "room":
                        method = Room;
                        break;
                    case "move":
                        method = Move;
                        break;
                    case "end":
                        method = End;
                        break;
                    default:
                        break;
                }
                if (method == null) continue;
                if (isTogether)
                    togetherLines.Add(new KeyValuePair<StorylineMethod, object[]> (method,content));
                else
                    storylines.Add(new KeyValuePair<StorylineMethod, object[]>(method, content));
            }
        }
        return storylines;
    }

    bool Together(params object[] args)
    {
        isRead = false;
        List<KeyValuePair<StorylineMethod, object[]>> togetherLines = (List<KeyValuePair<StorylineMethod, object[]>>) args[0];
        foreach (KeyValuePair<StorylineMethod, object[]> line in togetherLines)
        {
            line.Key.Invoke(line.Value);
        }
        isRead = true;
        return true;
    }

    bool LockControl(params object[] args)
    {
        MainManager.instance.playerController.SetControl(false);
        Debug.Log("Lock Control");
        return true;
    }

    bool FullControl(params object[] args)
    {
        AnimAsset player = MainManager.instance.anims.First(x => x.nama == "Player");
        player.anim.Play("Idle");
        MainManager.instance.playerController.SetControl(true);
        Debug.Log("Full Control");
        return true;
    }

    bool Anim(params object[] args)
    {
        AnimAsset animAsset = MainManager.instance.anims.First(x => x.nama.ToLower() == args[0].ToString().ToLower());
        animAsset.anim.Play(args[1].ToString());
        if (!args.Contains("NoWait") && isRead)
            StartCoroutine(DelayAnim(animAsset.anim));
        Debug.Log("Animation : " + args[0] + "," + args[1]);
        return true;
    }
    bool AnimObject(params object[] args)
    {
        Interactables animAsset = MainManager.instance.interactablesList.First(x => x.nama.ToLower() == args[0].ToString().ToLower()).interactables;
        animAsset.PlayAnim(args[1].ToString());
        if (!args.Contains("NoWait") && isRead)
            StartCoroutine(DelayAnim(animAsset.anim));
        if (!args.Contains("NoHide"))
        {
            AnimAsset player = MainManager.instance.anims.First(x => x.nama == "Player");
            player.anim.Play("Hide");
        }
        Debug.Log("Object Animation : " + args[0] + "," + args[1]);
        return true;
    }

    bool VFX(params object[] args)
    {
        MainManager.instance.vfx.Play(args[0].ToString());
        if (!args.Contains("NoWait") && isRead)
            StartCoroutine(DelayAnim(MainManager.instance.vfx));
        Debug.Log("VFX : " +args[0]);
        return true;
    }

    bool SFX(params object[] args)
    {
        MainManager.instance.sfx.PlayOneShot(MainManager.instance.sfxList.First(x =>x.nama == args[0].ToString()).clip);
        Debug.Log("SFX : " + args[0]);
        return true;
    }

    public bool Dialog(params object[] args)
    {
        MainManager.instance.dialogManager.StartDialog(args[0].ToString(), new string[] { args[1].ToString() });
        isRead = false;
        Debug.Log("Dialog : " + args[0] + "," + args[1]);
        return true;
    }

    bool State(params object[] args)
    {
        InteractablesAsset interactablesAsset = MainManager.instance.interactablesList.First(x => x.nama == args[0].ToString());
        interactablesAsset.interactables.SetInteract(int.Parse(args[1].ToString()));
        Debug.Log("State : " + args[0] + "," + args[1]);
        return true;
    }

    bool Go(params object[] args)
    {
        MainManager.instance.roomManager.Move(args[0].ToString());
        isRead = false;
        Debug.Log("GO : " + args[0]);
        return true;
    }

    bool Move(params object[] args)
    {
        Transform entity = MainManager.instance.entityList.First(x => x.name.ToLower().Equals(args[0].ToString().ToLower())).entity;
        Transform pos = MainManager.instance.roomManager.GetPoint(args[1].ToString(), args[2].ToString());
        entity.position = pos.position;
        Debug.Log("Move : " + args[0] + "," + args[1] + "," + args[2]);
        return true;
    }

    bool Cutscene(params object[] args)
    {
        MainManager.instance.cutsceneManager.Play(args[0].ToString());
        isRead = false;
        Debug.Log("Cutscene : " + args[0]);
        return true;
    }

    bool Tele(params object[] args)
    {
        MainManager.instance.roomManager.Tele(args[0].ToString(), args[1].ToString());
        MainManager.instance.playerController.SetControl(false);
        Debug.Log("Tele : " + args[0] + "," + args[1]);
        return true;
    }

    bool Camera(params object[] args)
    {
        MainManager.instance.cfx.Play(args[0].ToString());
        if (isRead)
        {
            float duration = MainManager.instance.cfx.GetCurrentAnimatorClipInfo(0).Length;
            StartCoroutine(DelayRead(duration));
        }
        Debug.Log("Camera : " + args[0]);
        return true;
    }

    bool Room(params object[] args)
    {
        MainManager.instance.roomManager.SetRoom(args[0].ToString(), int.Parse(args[1].ToString()));
        Debug.Log("Room : " + args[0] + "," + args[1]);
        return true;
    }

    bool Trigger(params object[] args)
    {
        isRead = false;
        for (int i = 0; i < args.Length; i++)
        {
            Triggers.Add(args[i].ToString(), false);
        }
        Debug.Log("Trigger : " + args[0]);
        return true;
    }

    public void SetTrigger(string name)
    {
        if (Triggers.ContainsKey(name))
        {
            Triggers[name] = true;
            Debug.Log("Set Trigger " + name);
            if (Triggers.Values.All(x => x))
            {
                Debug.Log("All trigger set");
                Triggers.Clear();
                Continue();
            }
        }
        else
        {
            Debug.Log("Trigger "+name+" tidak ada");
        }
    }

    public void Continue()
    {
        isRead = true;
        MainManager.instance.playerController.SetControl(false);
        Debug.Log("Continue");
    }

    IEnumerator DelayRead(float time)
    {
        isRead = false;
        yield return new WaitForSeconds(time);
        isRead = true;
    }

    IEnumerator DelayAnim(Animator anim)
    {
        isRead = false;
        while (anim.GetCurrentAnimatorClipInfo(0).Length == 0)
            yield return null;
        AnimationClip clip = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
        Debug.Log(String.Format("Delay : {0},{1},{2}", anim.name, clip.name, clip.length));
        yield return new WaitForSeconds(clip.length);
        isRead = true;
    }

    bool End(params object[] args)
    {
        isRead = false;
        MainManager.instance.gameOver.SetActive(true);
        return true;
    }
}
