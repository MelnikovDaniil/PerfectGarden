using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class OrderManagerUI : MonoBehaviour
{
    public static OrderManagerUI Instance;
    public TMP_Dropdown ordersDropdown;
    public Button careButton;
    public Animator dialogMenu;
    public TMP_Text dialogueText;
    public float typingSpeed = 0.05f;
    public float minDialogueTime = 1f;

    private Order currentSelectedOrder;
    private List<Order> currentOrders = new List<Order>();
    private bool isDropdownUpdateInProgress = false;

    private void Awake()
    {
        Instance = this;
        CareManager.OnCareMenuOpen += () =>
        {
            ordersDropdown.gameObject.SetActive(false);
            careButton.gameObject.SetActive(false);
        };
        CareManager.OnCareMenuClosed += () =>
        {
            if (currentOrders.Any())
            {
                ordersDropdown.gameObject.SetActive(true);
                careButton.gameObject.SetActive(true);
            }
        };
        OrderManager.OnOrderAdded += AddOrderToDropdown;
        OrderManager.OnOrderAdded += (order) => ordersDropdown.gameObject.SetActive(true);
        OrderManager.OnOrderСomplete += RemoveOrderFromDropdown;

        OrderManager.OnOrderPlaced += (order) =>
        {
            careButton.gameObject.SetActive(true);
        };
        OrderManager.OnAllOrdersComplete += () =>
        {
            ordersDropdown.gameObject.SetActive(false);
            careButton.gameObject.SetActive(false);
        };

        careButton.onClick.AddListener(async () =>
        {
            careButton.gameObject.SetActive(false);
            await OrderManager.Instance.StartOrderAsync();
        });
        ordersDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Start()
    {
        dialogMenu.gameObject.SetActive(false);
        UpdateDropdownOptions();
        ordersDropdown.RefreshShownValue();
    }

    public async Task ShowDialogueAsync(string text, bool isThankYou = false, CancellationToken token = default)
    {
        dialogueText.text = "";
        dialogMenu.gameObject.SetActive(true);
        careButton.gameObject.SetActive(!isThankYou);
        var dialogueTime = Task.Delay((int)(minDialogueTime * 1000));
        dialogMenu.Play("DialogueMenu_Appearence", 0, 0);

        foreach (char letter in text.ToCharArray())
        {
            if (token.IsCancellationRequested)
                return;

            dialogueText.text += letter;
            await Task.Delay((int)(typingSpeed * 1000));
        }

        careButton.gameObject.SetActive(true);
        await dialogueTime;
    }

    private async void OnDropdownValueChanged(int index)
    {
        dialogMenu.gameObject.SetActive(false);
        if (isDropdownUpdateInProgress || index < 0 || index >= currentOrders.Count)
            return;

        currentSelectedOrder = currentOrders[index];

        await OrderManager.Instance.PlaceOrderAsync(currentSelectedOrder);

        ordersDropdown.SetValueWithoutNotify(index);
    }

    private void AddOrderToDropdown(Order order)
    {
        if (order == null) return;

        currentOrders.Add(order);

        UpdateDropdownOptions();
    }

    private void UpdateDropdownOptions()
    {
        isDropdownUpdateInProgress = true;

        var currentSelection = ordersDropdown.value;

        ordersDropdown.ClearOptions();

        var options = new List<string>();
        foreach (var order in currentOrders)
        {
            options.Add($"{order.Name} - {order.Reward}G");
        }

        ordersDropdown.AddOptions(options);

        if (currentSelection >= 0 && currentSelection < options.Count)
        {
            ordersDropdown.SetValueWithoutNotify(currentSelection);
        }
        else
        {
            ordersDropdown.SetValueWithoutNotify(-1);
        }

        isDropdownUpdateInProgress = false;
    }

    private void RemoveOrderFromDropdown(Order order)
    {
        if (order == null) return;

        int index = currentOrders.IndexOf(order);
        if (index >= 0)
        {
            currentOrders.RemoveAt(index);
            UpdateDropdownOptions();
        }
    }
}
