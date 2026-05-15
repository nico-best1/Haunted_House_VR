using System.IO;

public class FilePersistence : APersistence
{
    string path;

    public void setPath(string p) { path = p; }

    public override void Flush() {
        string data = "";
        for(int i = MaxBuffer+index-eventSize; i < MaxBuffer+index; i++)
        {
            data += this.serializer.serialize(events[i % MaxBuffer]);
            data += "\n";
        }

        File.AppendAllText(path, data);

        eventSize = 0;
    }
}
