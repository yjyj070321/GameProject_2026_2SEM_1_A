using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionUI interactionUI;

    [Header("아이템 탐색")]
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;


    private Pickupltem targetltem;
    private Door targetDoor;
    private NPC targetNPC;

    private bool isPickingUp;

    void Start()
    {
        
    }

    void Update()
    {
        if (isPickingUp) return;

        targetltem = FindClosestItem();
        targetDoor = FindClosestDoor();
        targetNPC = FindClosestNPC();

        ShowGuide();

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (targetltem == null && targetDoor == null && targetNPC == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private Pickupltem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        Pickupltem closestltem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders)
        {
            Pickupltem item = itemCollider.GetComponentInParent<Pickupltem>();
            if (item == null) continue;
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestltem = item;
            }
        }
        return closestltem;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        interactionUI.HideGuide();

        Pickupltem itemToCollect = targetltem;

        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y = 0;

        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        playerController.ChangeState(PlayerState.Pickup);

        animator.CrossFade("PickUp01", 0.1f);

        yield return new WaitForSeconds(pickupMoment);

        if (itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        animator.CrossFade("Blend Tree", 0.1f);

        playerController.ChangeState(PlayerState.Normal);

        targetltem = null;
        targetDoor = null;
        targetNPC = null;
        isPickingUp = false;

    }

    private Door FindClosestDoor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders)
        {
            Door door = targetCollider.GetComponentInParent<Door>();
            if (door == null) continue;
            float distance = Vector3.Distance(transform.position, door.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDoor = door;
            }
        }
        return closestDoor;
    }

    private NPC FindClosestNPC()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders)
        {
            NPC npc = targetCollider.GetComponentInParent<NPC>();
            if (npc == null) continue;
            float distance = Vector3.Distance(transform.position, npc.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }
        return closestNPC;
    }

    private void ShowGuide()
    {
        if (targetltem != null)
        {
            interactionUI.ShowGuide("[E] " + targetltem.GetltemName() + "줍기");
        }
        else if (targetDoor != null)
        {
            interactionUI.ShowGuide(targetDoor.GetGuideText());
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowGuide(targetNPC.GetGuideText());
        }
        else
        {
            interactionUI.HideGuide();
        }
    }

    private void Interact()
    {
        if (targetltem != null)
        {
            StartCoroutine(PickupAnimation());
        }
        else if (targetDoor != null)
        {
            targetDoor.Interact();
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowDialogue(targetNPC.GetNPCName(), targetNPC.GetMessage());
        }


    }
}
