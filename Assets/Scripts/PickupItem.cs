using UnityEngine;

public class Pickupltem : MonoBehaviour
{
    [Header("아이템 정보")]
    [SerializeField] private string itemName = "OfOl!";
    [SerializeField] private int amount = 1;

    [SerializeField] QuestManager questManager;

    [Header("화면 연출")]
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float floatingHeight = 0.15f;
    [SerializeField] private float floatingSpeed = 2f;

    private Vector3 startPosition;

    private SimpleInventory inventory;

    public void Awake()
    {
        questManager = FindFirstObjectByType<QuestManager>();
        inventory = FindFirstObjectByType<SimpleInventory>();
    }


    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        float y = Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
        transform.position = startPosition + Vector3.up * y;
    }

    public void Collect()
    {
        if (inventory == null)
        {
            Debug.Log("Simplelnventory를 찾을 수 없습니다.");
            return;
        }

        if (inventory.Addltem(itemName, amount) == false)
        {
            return;
        }

        Debug.Log(itemName + " " + amount + "개 획득");
        if (questManager != null)
        {
            questManager.AddQuestItem(itemName, amount);
        }
        Destroy(gameObject);
    }

    public string GetltemName()
    {
        return itemName;
    }
}
