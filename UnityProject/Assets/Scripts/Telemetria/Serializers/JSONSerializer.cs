
public class JSONSerializer : ISerializer
{
    public string serialize(TrackerEvent e) {  return e.ToJSON(); }
}
