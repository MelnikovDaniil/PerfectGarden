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
    public List<BuffButton> buffButtons;
    public List<CareButton> careButtons;
    public Button purchaseButton;
    public Button trashButton;
    public TextMeshProUGUI priceText;
    public GameObject canvas;
    public Button BackButton;

    private bool isLocked;

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

    public void ShowMenu(PotWithPlant plant, bool unlock = false)
    {
        if (!isLocked || unlock)
        {
            isLocked = false;
            canvas.SetActive(true);
            currencyText.text = MoneyMapper.Money + "$";
            BackButton.gameObject.SetActive(true);
            HideAllButtons();

            if (plant.IsShouldBeRotted)
            {
                trashButton.gameObject.SetActive(true);
                trashButton.onClick.RemoveAllListeners();
                trashButton.onClick.AddListener(async () =>
                {
                    HideAllButtons();
                    await CareManager.Instance.MoveToTrash();
                });
            }
            else if (!plant.IsLastStage)
            {

                if (plant.waitingCareEvents.Any())
                {
                    var careButtonsToEnable = careButtons.Where(x => plant.waitingCareEvents.Contains(x.eventName));
                    foreach (var careButton in careButtonsToEnable)
                    {
                        careButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    var activeBuffTypes = plant.GetAllBuffStates().Select(x => x.buffType);
                    var buffButtonsToEnable = buffButtons.Where(x => !activeBuffTypes.Contains(x.buffType));
                    foreach (var buffButton in buffButtonsToEnable)
                    {
                        buffButton.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                purchaseButton.gameObject.SetActive(true);
                priceText.text = plant.plantInfo.plantPrice.ToString();
                purchaseButton.onClick.RemoveAllListeners();
                purchaseButton.onClick.AddListener(async () =>
                {
                    HideAllButtons();
                    await CareManager.Instance.PurchaseAsync();
                });
            }

        }
    }

    public void HideMenu()
    {
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.gameObject.SetActive(false);
        canvas.SetActive(false);
        BackButton.gameObject.SetActive(false);
    }

    public void HideAllButtons(bool locked = false)
    {
        isLocked = locked;
        purchaseButton.onClick.RemoveAllListeners();
        trashButton.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(false);
        careButtons.ForEach(x => x.gameObject.SetActive(false));
        buffButtons.ForEach(x => x.gameObject.SetActive(false));
    }
}