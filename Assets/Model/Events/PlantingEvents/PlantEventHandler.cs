public abstract class PlantEventHandler : EventHandler
{
    public abstract PlantingEvent EventName { get; }

    protected PlantContext Context { get; private set; }

    public void Setup(PlantContext context)
    {
        Status = HandlingStatus.Scheduled;
        Context = context;
    }
}
