using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class OrderManagerUI : MonoBehaviour
{
    public TMP_Dropdown ordersDropdown;
    public Button careButton;

    private List<Order> currentOrders = new List<Order>();
    private bool isDropdownUpdateInProgress = false;

    private void Awake()
    {
        CareManager.OnCareMenuOpen += () =>
        {
            ordersDropdown.gameObject.SetActive(false);
            careButton.gameObject.SetActive(false);
        };
        CareManager.OnCareMenuClosed += () =>
        {
            ordersDropdown.gameObject.SetActive(true);
            careButton.gameObject.SetActive(true);
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
        UpdateDropdownOptions();
    }
    private void OnDropdownValueChanged(int index)
    {
        if (isDropdownUpdateInProgress || index < 0 || index >= currentOrders.Count)
            return;

        var selectedOrder = currentOrders[index];

        OrderManager.Instance.PlaceOrder(selectedOrder);

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
