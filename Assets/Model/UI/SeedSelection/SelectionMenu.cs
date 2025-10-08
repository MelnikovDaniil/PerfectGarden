using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMenu : MonoBehaviour
{
    public Action<ProductInfo> OnProductSelection;
    public Transform cardPlace;

    [SerializeField] private Canvas seedSlectionCanvas;
    [SerializeField] private PlantInformationCard informationCard;
    [SerializeField] private Canvas moreInforCanvas;
    [SerializeField] private Canvas errorMessage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private ProductMiniCard miniCardPrefab;
    [SerializeField] private List<TextMeshProUGUI> moneyTests;

    private void Awake()
    {
        cancelButton.onClick.AddListener(() => PlantingManager.Instance.CancelPlanting());
    }

    public void GenerateCards(List<ProductInfo> productInfos)
    {
        ClearCards();
        UpdateMoney();
        foreach (var productInfo in productInfos)
        {
            var createdCard = Instantiate(miniCardPrefab, cardPlace);
            createdCard.OnDetailedView += () => OpenDetailedView(productInfo);
            createdCard.OnTake += () => OnProductSelection(productInfo);
            createdCard.OnPurchase += () => Purchase(productInfo);
            createdCard.CreateCard(productInfo);
        }
    }

    public void OpenCardsView()
    {
        seedSlectionCanvas.gameObject.SetActive(true);
        moreInforCanvas.gameObject.SetActive(false);
    }

    public void OpenDetailedView(ProductInfo productInfo)
    {
        seedSlectionCanvas.gameObject.SetActive(false);
        moreInforCanvas.gameObject.SetActive(true);
        informationCard.OnTake = () => OnProductSelection(productInfo);
        informationCard.OnPurchase += () => Purchase(productInfo);
        informationCard.ShowCard(productInfo);
    }

    public void Hide()
    {
        seedSlectionCanvas.gameObject.SetActive(false);
        moreInforCanvas.gameObject.SetActive(false);
    }

    private void UpdateMoney()
    {
        var money = MoneyMapper.Money;
        foreach (var text in moneyTests)
        {
            text.text = money.ToString();
        }
    }

    private void Purchase(ProductInfo productInfo)
    {
        if (MoneyMapper.Money > productInfo.Price)
        {
            MoneyMapper.Money -= productInfo.Price;
            ProductMapper.Add(productInfo.Name);
            productInfo.ItemsAvaliable++;
            OnProductSelection(productInfo);
            UpdateMoney();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        errorMessage.gameObject.SetActive(true);
    }

    private void ClearCards()
    {
        foreach (Transform card in cardPlace)
        {
            Destroy(card.gameObject);
        }
    }
}
