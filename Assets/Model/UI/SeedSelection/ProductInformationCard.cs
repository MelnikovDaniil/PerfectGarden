using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantInformationCard : MonoBehaviour
{
    public Action OnTake;
    public Action OnPurchase;

    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI itemsAvaliableText;
    [SerializeField] private Button takeButton;
    [SerializeField] private TextMeshProUGUI takeButtonText;

    public void ShowCard(ProductInfo productInfo)
    {
        nameText.text = productInfo.Name;

        previewImage.sprite = productInfo.Preview;

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
