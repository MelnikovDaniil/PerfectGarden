using Assets.Scripts.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        selectedPot = null;
        selectionMenu.GenerateCards(potsProduct);
        selectionMenu.OnProductSelection = (product) => { selectedPot = product.GetProduct<PotInfo>(); };
        await base.PrepareHandlingAsync();
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        selectionMenu.OpenCardsView();
        await Task.Yield();
        if (!GuideMapper.IsGuideComplete(GuideStep.PotSelection))
        {
            var purchaseButton = selectionMenu.GetComponentInChildren<ProductMiniCard>().GetComponentsInChildren<Button>().Last();
            await TutorialManager.Instance.SetTap(purchaseButton.gameObject, true, token);
            GuideMapper.Complete(GuideStep.PotSelection);
        }
        while (selectedPot == null)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await Task.Yield();
        }
        var generatedPot = Instantiate(selectedPot.potPrefab);
        generatedPot.potInfo = selectedPot;
        Context.PotWithPlant = generatedPot;
    }

    public override void Clear()
    {
        Context?.PotWithPlant?.gameObject?.SetActive(false);
        selectionMenu.Hide();
        selectedPot = null;
        base.Clear();
    }
}
