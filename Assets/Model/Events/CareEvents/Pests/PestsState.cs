using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PestsState : CareState
{
    public int wormNumberLeft;
    public List<Worm> worms = new List<Worm>();

    private PestsScriptableStateInfo pestsStateInfo => (PestsScriptableStateInfo)StateInfo;


    public PestsState(PestsScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant plant)
    {
        worms = new List<Worm>();
        wormNumberLeft = Random.Range(pestsStateInfo.minWormNumber, pestsStateInfo.maxWormNumber);

        if (plant.gameObject.activeInHierarchy)
        {
            GeneratePests(plant);
        }
        else
        {
            plant.OnEnableOnes += () => GeneratePests(plant);
        }
    }

    public override void Complete(PotWithPlant plant)
    {
        foreach (var worm in worms)
        {
            if (worm != null)
                GameObject.Destroy(worm.gameObject);
        }
        worms.Clear();
    }

    public Vector3 GetGroupPosition()
    {
        worms.RemoveAll(x => x == null);
        if (worms == null || !worms.Any())
        {
            return Vector3.zero;
        }

        var sumPosition = Vector3.zero;
        foreach (var obj in worms)
        {
            sumPosition += obj.transform.position;
        }

        return sumPosition / worms.Count;
    }

    public void PullOutWorm()
    {
        wormNumberLeft--;
        worms.RemoveAll(x => x == null);
    }

    private void GeneratePests(PotWithPlant plant)
    {
        for (var i = 0; i < wormNumberLeft; i++)
        {
            var createdWorm = GameObject.Instantiate(pestsStateInfo.wormPrefab, plant.dirtCollider.transform);
            createdWorm.SetUp(
                Random.Range(pestsStateInfo.minAppearanceRate, pestsStateInfo.maxAppearanceRate),
                Random.Range(pestsStateInfo.minAppearanceTime, pestsStateInfo.maxAppearanceTime));
            createdWorm.transform.localPosition = plant.dirtCollider.GetRandomPointInCollider();
            worms.Add(createdWorm);
        }
    }
}
