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
            createdWeed.transform.localPosition = GetRandomPointInCollider(plant.dirtCollider);
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

    private Vector3 GetRandomPointInCollider(Collider collider)
    {
        var bounds = collider.bounds;

        for (var i = 0; i < 100; i++)
        {
            var localRandomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            var randomPoint = collider.transform.TransformPoint(localRandomPoint);
            Debug.Log($"Random weed point generated: {randomPoint}");
            // Check if the random point is inside the collider using Collider.ClosestPoint
            var closestPoint = collider.ClosestPoint(randomPoint);
            if (closestPoint == randomPoint) // If it's the same, it means the point is inside
            {
                return localRandomPoint;
            }
        }

        return Vector3.zero;
    }
}
