using UnityEngine;

[CreateAssetMenu(fileName = "SpeedGroBuffStateInfo", menuName = "Buffs/SpeedGro")]
public class SpeedGroScriptableBuffStateInfo : ScriptableBuffStateInfo
{
    public override BuffType BuffType => BuffType.SpeedGro;
    public ParticleSystem particles;
    public Vector3 particlesOffset = new Vector3(0, 0.2f, 0);
    public float timeReductionCoef = 0.5f;

    public override BuffState CreateState()
    {
        return new SpeedGroBuffState(this);
    }

    public override BuffState CreateStateFromSave(BuffSaveInfo buffStateInfo)
    {
        var speedGroBuffStateInfo = (SpeedGroBuffSaveInfo) buffStateInfo;
        return new SpeedGroBuffState(this, speedGroBuffStateInfo);
    }
}