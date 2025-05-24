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

    public SelectionMenu selectionMenu;

    private List<ProductInfo> plantProducts;
    private PlantInfo selectedSeed;

    private void Start()
    {
        MoneyMapper.Money = 10000;
        plantProducts = Resources.LoadAll<PlantInfo>("Plants")
            .Select(plant => new ProductInfo(plant, plant.name, plant.shopSprite, plant.seedPrice, ProductMapper.GetAvaliableProducts(plant.name))).ToList();
    }

    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        selectedSeed = null;
        selectionMenu.GenerateCards(plantProducts);
        selectionMenu.OnProductSelection = (product) => { selectedSeed = product.GetProduct<PlantInfo>(); };
        return base.PrepareHandlingAsync();
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        selectionMenu.OpenCardsView();
        if (!GuideMapper.IsGuideComplete(GuideStep.SeedSelection))
        {
            var purchaseButton = selectionMenu.GetComponentInChildren<ProductMiniCard>().GetComponentsInChildren<Button>().Last();
            await TutorialManager.Instance.SetTap(purchaseButton.gameObject, true, token);
            GuideMapper.Complete(GuideStep.SeedSelection);
        }
        while (selectedSeed == null)
        {
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

}
