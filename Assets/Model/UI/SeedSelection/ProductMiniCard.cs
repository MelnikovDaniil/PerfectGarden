using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductMiniCard : MonoBehaviour
{
    public event Action OnDetailedView;
    public event Action OnTake;
    public event Action OnPurchase;

    [SerializeField] private Button cardButton;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI itemsAvaliableText;
    [SerializeField] private Button takeButton;
    [SerializeField] private TextMeshProUGUI takeButtonText;

    public void CreateCard(ProductInfo productInfo)
    {
        nameText.text = productInfo.Name;
        itemsAvaliableText.enabled = false;
        takeButton.interactable = true;

        previewImage.sprite = productInfo.Preview;
        previewImage.color = Color.white;
        cardButton.onClick.AddListener(() => OnDetailedView?.Invoke());

        if (productInfo.ItemsAvaliable > 0)
        {
            itemsAvaliableText.enabled = true;
            takeButtonText.text = "Take";
            itemsAvaliableText.text = productInfo.ItemsAvaliable.ToString();
            takeButton.onClick.AddListener(() => OnTake?.Invoke());
        }
        else if (productInfo.IsUnlocked || productInfo.IsPartiallyVisible)
        {
            takeButtonText.text = productInfo.Price.ToString();
            takeButton.onClick.AddListener(() => OnPurchase?.Invoke());
            if (!productInfo.IsUnlocked)
            {
                previewImage.color= Color.black;
                nameText.text = "???";
                takeButton.interactable = false;
            }
        }
    }
}
