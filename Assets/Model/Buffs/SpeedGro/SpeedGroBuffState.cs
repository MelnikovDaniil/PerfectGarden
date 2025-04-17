using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpeedGroBuffState : BuffState
{
    public DateTime endDate;
    private ParticleSystem particlesInstance;

    private SpeedGroScriptableBuffStateInfo speedGroStateInfo => (SpeedGroScriptableBuffStateInfo)StateInfo;
    private SpeedGroBuffSaveInfo speedGroSaveInfo => (SpeedGroBuffSaveInfo)SaveInfo;

    public SpeedGroBuffState(SpeedGroScriptableBuffStateInfo stateInfo) : base(stateInfo) 
    {
    }

    public SpeedGroBuffState(SpeedGroScriptableBuffStateInfo stateInfo, SpeedGroBuffSaveInfo buffSaveInfo) : base(stateInfo, buffSaveInfo)
    {
        endDate = new DateTime(buffSaveInfo.endDateUtc);
    }

    public override void Apply(PotWithPlant plant)
    {
        if (speedGroSaveInfo == null)
        {
            endDate = plant.lastStageChangeTime + (DateTime.UtcNow - (plant.lastStageChangeTime + plant.plantInfo.timeBetweenStages)) / speedGroStateInfo.timeReductionCoef;
        }
        particlesInstance = GameObject.Instantiate(speedGroStateInfo.particles, plant.transform);
        particlesInstance.transform.localPosition = speedGroStateInfo.particlesOffset;

        plant.OnStageChange += () => plant.CompleteBuffState(buffType);
    }

    public override void Complete(PotWithPlant plant)
    {
        GameObject.Destroy(particlesInstance.gameObject);
    }

    public override BuffSaveInfo GetSaveInfo()
    {
        var stateInfo = new SpeedGroBuffSaveInfo { buffType = BuffType.SpeedGro, endDateUtc = endDate.Ticks };
        stateInfo.Serialize();
        return stateInfo;

    }
}
