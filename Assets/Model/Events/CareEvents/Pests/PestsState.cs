using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PestsState : CareState
{
    public int wormNumberLeft;

    private PestsScriptableStateInfo pestsStateInfo => (PestsScriptableStateInfo)StateInfo;

    private List<Worm> worms = new List<Worm>();

    public PestsState(PestsScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant plant)
    {
        worms = new List<Worm>();
        wormNumberLeft = Random.Range(pestsStateInfo.minWormNumber, pestsStateInfo.maxWormNumber);
        var wormGroupPosition = plant.dirtCollider.GetRandomPointInCollider();

        for (var i = 0; i < wormNumberLeft; i++)
        {
            var createdWorm = GameObject.Instantiate(pestsStateInfo.wormPrefab, plant.dirtCollider.transform);
            createdWorm.SetUp(
                Random.Range(pestsStateInfo.minAppearanceRate, pestsStateInfo.maxAppearanceRate),
                Random.Range(pestsStateInfo.minAppearanceTime, pestsStateInfo.maxAppearanceTime));
            createdWorm.transform.localPosition = GetRandomPointAround(plant.dirtCollider, wormGroupPosition, pestsStateInfo.wormGroupRadius);
            worms.Add(createdWorm);
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
    }

    private Vector3 GetRandomPointAround(Collider collider, Vector3 center, float radius)
    {
        var bounds = collider.bounds;

        for (var i = 0; i < 100; i++)
        {
            var localRandomPoint = Random.insideUnitCircle;
            var randomPointInGroup = center + new Vector3(localRandomPoint.x, 0, localRandomPoint.y) * radius;

            var randomPoint = collider.transform.TransformPoint(randomPointInGroup);

            var closestPoint = collider.ClosestPoint(randomPoint);
            if (closestPoint == randomPoint) // If it's the same, it means the point is inside
            {
                return randomPointInGroup;
            }
        }

        return Vector3.zero;
    }
}
