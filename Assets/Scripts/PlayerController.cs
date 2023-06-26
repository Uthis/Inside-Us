using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] SceneDialogue cutscene;
    [SerializeField] CinemachineConfiner confiner;

    PlayerInput input;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Dialogue dialog;
    Animator anim;

    Interactables currInteractable;

    bool isControl,isAuto,isInteracting;
    Transform target;

    private void Awake()
    {
        input = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        sr = transform.Find("Animation").GetComponent<SpriteRenderer>();
        dialog = GetComponentInChildren<Dialogue>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        input.Control.Enable();
    }

    private void OnDisable()
    {
        input.Control.Disable();
    }

    public void SetControl(bool isActive)
    {
        isControl = isActive;
        SetMovement(isActive);
    }

    public void SetMovement(bool isActive)
    {
        if (isActive)
            input.Control.Movement.Enable();
        else
            input.Control.Movement.Disable();
    }

    public void SetInteract(bool isActive)
    {
        if (isActive)
            input.Control.Interact.Enable();
        else
            input.Control.Interact.Disable();
    }

    public void Transport(Room room, Transform point)
    {
        SetMovement(false);
        transform.position = point.position;
        confiner.GetComponent<CinemachineVirtualCamera>().ForceCameraPosition(point.position, Quaternion.identity);
        confiner.m_BoundingShape2D = room.confiner;
        SetMovement(true);
    }

    public void MoveToPoint(Transform point)
    {
        target = point;
        isAuto = true;
    }

    private void Start()
    {
        input.Control.Interact.performed += OnInteract;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAuto)
        {
            float distance = target.position.x - transform.position.x;
            rb.velocity = new Vector2(distance / Mathf.Abs(distance) * speed, 0);
            if((target.position - transform.position).x < 0.1f)
            {
                transform.position = target.position;
                isAuto = false;
                target = null;
                rb.velocity = Vector2.zero;
                if (isInteracting)
                {
                    sr.flipX = currInteractable.isFaceRight;
                    Interact();
                }
                else
                    GameManager.instance.reader.Continue();
                isInteracting = false;
            }
        }
        else
        {
            Vector2 movement = input.Control.Movement.ReadValue<Vector2>();
            rb.velocity = movement * speed;
        }

        anim.SetBool("isMoving", rb.velocity != Vector2.zero);
        sr.flipX = rb.velocity != Vector2.zero? rb.velocity.x > 0:sr.flipX;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.instance.dialogManager.isShowingDialog)
        {
            bool isDone = GameManager.instance.dialogManager.Next();
            if (isDone)
                if (isControl)
                    SetMovement(true);
                else
                    GameManager.instance.reader.Continue();
        }
        else if (cutscene.isShowing)
        {
            bool isDone = cutscene.Next();
            if (isDone)
                if(isControl)
                    SetMovement(true);
                else
                    GameManager.instance.reader.Continue();
        }
        else if (currInteractable != null)
        {
            if (currInteractable.onStandPoint)
            {
                target = currInteractable.standPoint;
                isAuto = true;
                isInteracting = true;
            }
            else
                Interact();
        }
    }

    private void Interact()
    {
        Interact currInteract = currInteractable.Interact();
        if (currInteract.type == InteractType.Dialog && dialog.isShowing) return;
        if (currInteract.type == InteractType.Scene && cutscene.isShowing) return;
        switch (currInteract.type)
        {
            case InteractType.Dialog:
                SetMovement(false);
                GameManager.instance.dialogManager.StartDialog("Player", currInteract.dialogTexts);
                break;
            case InteractType.Scene:
                SetMovement(false);
                cutscene.StartScene(currInteract.Cutscene);
                break;
            case InteractType.Animation:
                //storyManager.NextScene();
                break;
            case InteractType.Transport:
                GameManager.instance.roomManager.Tele(currInteract.targetRoom, currInteract.targetPos);
                break;
            case InteractType.Trigger:
                GameManager.instance.reader.SetTrigger(currInteract.name);
                break;
            case InteractType.AutoTrigger:
                GameManager.instance.reader.SetTrigger(currInteract.name);
                break;
            case InteractType.TriggerWithDialog:
                SetMovement(false);
                GameManager.instance.dialogManager.StartDialog("Player", currInteract.dialogTexts);
                GameManager.instance.reader.SetTrigger(currInteract.name);
                break;
            default:
                //SetMovement(true);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Interactable"))
        {
            currInteractable = collision.GetComponentInParent<Interactables>();
            InteractType type = currInteractable.GetType();
            if (type != InteractType.Off && type != InteractType.AutoTrigger)
                currInteractable.ShowInteract(true);
            else if(type == InteractType.AutoTrigger)
                if (currInteractable.onStandPoint)
                {
                    target = currInteractable.standPoint;
                    isAuto = true;
                    isInteracting = true;
                }
                else
                    Interact();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            collision.GetComponentInParent<Interactables>().ShowInteract(false);
            currInteractable = null;
        }
    }
}