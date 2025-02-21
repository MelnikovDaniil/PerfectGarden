using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
