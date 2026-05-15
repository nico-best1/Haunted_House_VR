
public abstract class APersistence
{
    protected const int MaxBuffer = 50;
    protected int index = 0;
    protected TrackerEvent[] events = new TrackerEvent[MaxBuffer];
    protected ISerializer serializer;
    protected int eventSize = 0;

    public void setSerializer(ISerializer s)
    {
        serializer = s;
    }

    public void Send(TrackerEvent e)
    {

        if(eventSize<MaxBuffer)
            eventSize++;

        events[index] = e;

        if (index < MaxBuffer - 1)
            index++;
        else
            index = 0;
    }

    public abstract void Flush();
}
