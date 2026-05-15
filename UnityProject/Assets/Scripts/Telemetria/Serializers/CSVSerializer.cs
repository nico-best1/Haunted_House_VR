
public class CSVSerializer : ISerializer
{
    public string serialize(TrackerEvent e) {  return e.ToCSV(); }
}
