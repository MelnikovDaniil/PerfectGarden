using System.Collections.Generic;
using UnityEngine;

public class WeedCareState : CareState
{
    public int weedNumberLeft;
    private WeedScriptableStateInfo weedStateInfo => (WeedScriptableStateInfo)StateInfo;
    private List<Weed> weeds;

    public WeedCareState(WeedScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant plant)
    {
        weeds = new List<Weed>();
        weedNumberLeft = Random.Range(weedStateInfo.minWeedNumber, weedStateInfo.maxWeedNumber);

        for (var i = 0; i < weedNumberLeft; i++)
        {
            var createdWeed = GameObject.Instantiate(weedStateInfo.weedPrefab, plant.dirtCollider.transform);
            createdWeed.renderer.sprite = weedStateInfo.weedSprites.GetRandom();
            createdWeed.transform.localPosition = plant.dirtCollider.GetRandomPointInCollider();
            weeds.Add(createdWeed);
        }
    }

    public override void Complete(PotWithPlant plant)
    {
        foreach (var weed in weeds)
        {
            if (weed != null)
                GameObject.Destroy(weed.gameObject);
        }
        weeds.Clear();
    }

    public void PullOutWeed()
    {
        weedNumberLeft--;
    }
}
