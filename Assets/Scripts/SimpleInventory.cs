using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleInventory : MonoBehaviour
{
    [Header("Ul 연결")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;

    private List<Inventoryltem> items = new List<Inventoryltem>();
    private PlayerController playerController;
    private PlayerPickup playerPickup;
    private ThirdPersonCamera thirdPersonCamera;
    private bool isOpen;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }
        inventoryPanel.SetActive(false);
    }

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (isOpen == false && playerController.IsNormalState() == false)
        {
            return;
        }
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerController.ChangeState(PlayerState.inventory);
            playerPickup.enabled = false;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerController.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slotText.Length; i++)
        {
            if (i < items.Count)
            {
                slotText[i].text = items[i].itemName + "\n" + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        }
    }

    public bool Addltem(string itemName, int amount)
    {
        foreach (Inventoryltem item in items)
        {
            if (item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }

        if (items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        Inventoryltem newItem = new Inventoryltem();
        newItem.itemName = itemName;
        newItem.amount = amount;

        items.Add(newItem);
        UpdateUI();

        return true;
    }

}
