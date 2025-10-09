using Assets.Scripts.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SeedSelectionEventHandler : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.SeedSelection;
    public float unlockPercentage = 0.5f;
    public SelectionMenu selectionMenu;

    private List<ProductInfo> plantProducts;
    private PlantInfo selectedSeed;

    private void Start()
    {
        MoneyMapper.Money = 6; // 10000;
        plantProducts = Resources.LoadAll<PlantInfo>("Plants")
            .OrderBy(x => x.seedPrice)
            .Select(plant =>
                new ProductInfo(
                    plant, plant.name, plant.shopSprite, plant.seedPrice,
                    ProductMapper.GetAvaliableProducts(plant.name),
                    plant.seedPrice * unlockPercentage < MoneyMapper.MaxMoneyReached,
                    plant.seedPrice < MoneyMapper.MaxMoneyReached)).ToList();
        var fistInvisibleProduct = plantProducts.FirstOrDefault(x => !x.IsPartiallyVisible);
        if (fistInvisibleProduct != null)
        {
            fistInvisibleProduct.IsPartiallyVisible = true;
        };
    }

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        selectedSeed = null;
        RefreshProduct();
        selectionMenu.GenerateCards(plantProducts);
        selectionMenu.OnProductSelection = (product) =>
        {
            selectedSeed = product.GetProduct<PlantInfo>();
            var productName = selectedSeed.name;
            Context.OnPlantingFinished += () =>
            {
                ProductMapper.Remove(productName);
                product.ItemsAvaliable--;
            };
        };
        await Task.Yield();
        await base.PrepareHandlingAsync();
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        selectionMenu.OpenCardsView();
        await Task.Yield();
        if (!GuideMapper.IsGuideComplete(GuideStep.SeedSelection))
        {
            var purchaseButton = selectionMenu.GetComponentsInChildren<ProductMiniCard>().First().GetComponentsInChildren<Button>().Last();
            await TutorialManager.Instance.SetTap(purchaseButton.gameObject, true, token);
            GuideMapper.Complete(GuideStep.SeedSelection);
        }
        while (selectedSeed == null)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            await Task.Yield();
        }
        Context.plantInfo = selectedSeed;
    }

    public override void Clear()
    {
        selectionMenu.Hide();
        selectedSeed = null;
        base.Clear();
    }

    private void RefreshProduct()
    {
        plantProducts.ForEach(product =>
        {
            product.IsUnlocked = product.Price * unlockPercentage < MoneyMapper.MaxMoneyReached;
            product.IsPartiallyVisible = product.Price < MoneyMapper.MaxMoneyReached;
        });

        var fistInvisibleProduct = plantProducts.FirstOrDefault(x => !x.IsPartiallyVisible);
        if (fistInvisibleProduct != null)
        {
            fistInvisibleProduct.IsPartiallyVisible = true;
        }
    }

}
