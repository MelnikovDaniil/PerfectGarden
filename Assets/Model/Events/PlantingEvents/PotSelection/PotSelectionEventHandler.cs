using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PotSelectionEventHandler : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.PotSelection;

    public SelectionMenu selectionMenu;

    private List<ProductInfo> potsProduct;
    private PotInfo selectedPot;

    private void Start()
    {
        potsProduct = Resources.LoadAll<PotInfo>("Pots")
            .Select(pot => new ProductInfo(pot, pot.name, pot.shopSprite, pot.price, ProductMapper.GetAvaliableProducts(pot.name))).ToList();
    }
    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        selectedPot = null;
        selectionMenu.GenerateCards(potsProduct);
        selectionMenu.OnProductSelection = (product) => { selectedPot = product.GetProduct<PotInfo>(); };
        return base.PrepareHandlingAsync();
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        selectionMenu.OpenCardsView();
        while (selectedPot == null)
        {
            await Task.Yield();
        }
        var generatedPot = Instantiate(selectedPot.potPrefab);
        generatedPot.potInfo = selectedPot;
        Context.PotWithPlant = generatedPot;
    }

    public override void Clear()
    {
        Context.PotWithPlant.gameObject.SetActive(false);
        selectionMenu.Hide();
        selectedPot = null;
        base.Clear();
    }
}
