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
        nameText.text = name;
        itemsAvaliableText.enabled = false;

        previewImage.sprite = productInfo.Preview;
        cardButton.onClick.AddListener(() => OnDetailedView?.Invoke());

        if (productInfo.ItemsAvaliable > 0)
        {
            itemsAvaliableText.enabled = true;
            takeButtonText.text = "Take";
            itemsAvaliableText.text = productInfo.ItemsAvaliable.ToString();
            takeButton.onClick.AddListener(() => OnTake?.Invoke());
        }
        else
        {
            takeButtonText.text = productInfo.Price.ToString();
            takeButton.onClick.AddListener(() => OnPurchase?.Invoke());
        }
    }

}
