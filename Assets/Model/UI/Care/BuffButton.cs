using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class BuffButton : MonoBehaviour
{
    public BuffType buffType;
    public int buffCost;
    public TMP_Text numberOfBuffText;

    private int numberOfBuffs;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(async () => await StartBuffAsync());
    }

    private void UpdateStatus()
    {
        numberOfBuffs = ProductMapper.GetAvaliableProducts(buffType.ToString());
        numberOfBuffText.gameObject.SetActive(false);
        if (numberOfBuffs > 0)
        {
            numberOfBuffText.gameObject.SetActive(true);
            numberOfBuffText.text = numberOfBuffs.ToString();
        }
    }

    private void OnEnable()
    {
        UpdateStatus();
    }

    private async Task StartBuffAsync()
    {
        if (numberOfBuffs > 0)
        {
            if (await CareManager.Instance.StartBuffAsync(buffType))
            {
                ProductMapper.Remove(buffType.ToString());
            }
        }
        else if (buffCost > MoneyMapper.Money)
        {
            PopupManagerUI.Instance.ShowAddMoneyPopup((success) =>
            {
                if (success)
                {
                    RewardManager.Instance.GenerateCareLargeReward(100);
                }
            });
        }
        else
        {
            ProductMapper.Add(buffType.ToString());
            if (await CareManager.Instance.StartBuffAsync(buffType))
            {
                ProductMapper.Remove(buffType.ToString());
            }
        }
    }
}
