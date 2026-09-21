using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    [Header("아이템 탐색")]
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;

    private Pickupltem targetltem;
    private bool isPickingUp;

    void Start()
    {
        
    }

    void Update()
    {
        if (isPickingUp) return;

        targetltem = FindClosestItem();

        if (targetltem == null) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupAnimation());
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
        isPickingUp = false;

    }



}
