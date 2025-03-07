using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CareMenu : MonoBehaviour
{
    public Action OnBackPressed;

    public TMP_Text currencyText;
    public List<CareButton> careButtons;
    public Button purchaseButton;
    public Button trashButton;
    public TextMeshProUGUI priceText;
    public GameObject canvas;
    public Button BackButton;

    private void Awake()
    {
        canvas.SetActive(false);
        careButtons.ForEach(x => x.gameObject.SetActive(false));
        RewardManager.Instance.OnTextUpdate += () => { currencyText.text = MoneyMapper.Money + "$"; };
    }

    private void Start()
    {
        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(() => OnBackPressed?.Invoke());
    }

    public void ShowMenu(PotWithPlant plant)
    {
        canvas.SetActive(true);
        currencyText.text = MoneyMapper.Money + "$";
        BackButton.gameObject.SetActive(true);
        HideCareButtons();

        if (plant.IsShouldBeRotted)
        {
            trashButton.gameObject.SetActive(true);
            trashButton.onClick.AddListener(async () =>
            {
                HideCareButtons();
                await CareManager.Instance.MoveToTrash();
            });
        }
        else if (!plant.IsLastStage)
        {
            var buttonsToEnable = careButtons.Where(x => plant.waitingCareEvents.Contains(x.eventName));
            foreach (var careButton in buttonsToEnable)
            {
                careButton.gameObject.SetActive(true);
            }
        }
        else
        {
            purchaseButton.gameObject.SetActive(true);
            priceText.text = plant.plantInfo.plantPrice.ToString();
            purchaseButton.onClick.AddListener(async () =>
            {
                HideCareButtons();
                await CareManager.Instance.PurchaseAsync();
            });
        }

    }

    public void HideMenu()
    {
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.gameObject.SetActive(false);
        canvas.SetActive(false);
        BackButton.gameObject.SetActive(false);
    }

    public void HideCareButtons()
    {
        purchaseButton.onClick.RemoveAllListeners();
        trashButton.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(false);
        careButtons.ForEach(x => x.gameObject.SetActive(false));
    }
}