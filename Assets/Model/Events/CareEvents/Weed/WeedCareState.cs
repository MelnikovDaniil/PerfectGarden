using System.Collections.Generic;
using UnityEngine;

public class WeedCareState : CareState
{
    public int weedNumberLeft;
    public List<Weed> weeds;

    private WeedScriptableStateInfo weedStateInfo => (WeedScriptableStateInfo)StateInfo;

    public WeedCareState(WeedScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant plant)
    {
        weeds = new List<Weed>();
        weedNumberLeft = Random.Range(weedStateInfo.minWeedNumber, weedStateInfo.maxWeedNumber);

        if (plant.gameObject.activeInHierarchy)
        {
            GenerateWeeds(plant);
        }
        else
        {
            plant.OnEnableOnes += () => GenerateWeeds(plant);
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

    private void GenerateWeeds(PotWithPlant plant)
    {
        for (var i = 0; i < weedNumberLeft; i++)
        {
            var createdWeed = GameObject.Instantiate(weedStateInfo.weedPrefab, plant.dirtCollider.transform);
            createdWeed.renderer.sprite = weedStateInfo.weedSprites.GetRandom();
            createdWeed.transform.localPosition = plant.dirtCollider.GetRandomPointInCollider();
            weeds.Add(createdWeed);
        }
    }
}
