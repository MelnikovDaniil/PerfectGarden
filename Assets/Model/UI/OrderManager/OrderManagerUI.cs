using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class OrderManagerUI : MonoBehaviour
{
    public Button careButton;

    private void Awake()
    {
        OrderManager.OnOrderPlaced += (order) => 
        {
            careButton.gameObject.SetActive(true);
        };
        OrderManager.OnAllOrdersComplete += () =>
        {
            careButton.gameObject.SetActive(false);
        };

        careButton.onClick.AddListener(async () =>
        {
            careButton.gameObject.SetActive(false);
            await OrderManager.Instance.StartOrderAsync();
        });
    }
}
