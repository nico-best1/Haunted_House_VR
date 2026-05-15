
using System;
using System.Diagnostics;
using System.IO;

// definicion de los formatos posibles para guardar los datos
public enum formatType
{
    JSON, CSV
}

// clase principal encargada de registrar eventos
public class Tracker
{
    static Tracker instance = null;

    // espacio minimo requerido en disco (2GB)
    const long MINIMUM_SPACE_DISK = 2L * 1024 * 1024 * 1024;

    // objeto encargado de guardar los datos
    APersistence persistenceObject;

    // identificador de la sesion
    string sessionId;

    // contador de eventos enviados
    int eventCount;

    // contador de archivos creados
    int fileCount = 0;

    // metodo de inicializacion del tracker
    public static string Init(string sessionId, int timeStamp, string path, bool filePersistence = true, formatType format = formatType.JSON)
    {

        // se crea la instancia del tracker
        instance = new Tracker();
        instance.sessionId = sessionId;

        // se obtiene informacion del disco donde se guardaran los archivos
        DriveInfo drive = new DriveInfo(Path.GetPathRoot(path));
        long freeSpace = drive.AvailableFreeSpace;

        // se comprueba si se puede guardar en fichero y hay espacio suficiente
        if (filePersistence && freeSpace > MINIMUM_SPACE_DISK)
        {
            FilePersistence per = new FilePersistence();
            string filePath = "";

            // si el formato es json
            if (format == formatType.JSON)
            {
                // se genera un nombre de archivo unico
                filePath = path + "/telemetry_" + instance.fileCount + ".json";
                while (File.Exists(filePath))
                {
                    instance.fileCount++;
                    filePath = path + "/telemetry_" + instance.fileCount + ".json";
                }
            }
            // si el formato es csv
            else if (format == formatType.CSV)
            {
                UnityEngine.Debug.Log("CSV");

                // se genera un nombre de archivo unico
                filePath = path + "/telemetry_" + instance.fileCount + ".csv";
                while (File.Exists(filePath))
                {
                    instance.fileCount++;
                    filePath = path + "/telemetry_" + instance.fileCount + ".csv";
                }
            }

            // se configura la ruta donde se guardaran los datos
            per.setPath(filePath);

            // se asigna el sistema de persistencia
            instance.persistenceObject = per;
        }
        else
        {
            // si no hay persistencia o espacio suficiente, no se guarda nada
            instance.persistenceObject = null;

            if (!filePersistence)
                return "persistencia en local desactivado";
            else
                return "no hay suficiente espacio en el disco duro (2gb)";
        }

        // se configura el tipo de serializador segun el formato elegido
        switch (format)
        {
            case formatType.JSON:
                instance.persistenceObject.setSerializer(new JSONSerializer());
                break;

            case formatType.CSV:
                instance.persistenceObject.setSerializer(new CSVSerializer());
                break;

            default:
                instance.persistenceObject = null;
                return "formato no reconocible";
        }

        // se registra el evento de inicio de sesion
        instance.TrackEvent(new TrackerEvent("Session_Start", timeStamp));

        return null;
    }

    // metodo para finalizar la sesion
    public static void End(int timeStamp, bool flush = true)
    {

        // se registra el evento de fin de sesion
        instance.TrackEvent(new TrackerEvent("Session_End", timeStamp));

        // se fuerzan a guardar los datos pendientes
        if (flush)
            instance.flush();

        // se elimina la instancia
        instance = null;
    }

    public static Tracker Instance
    {
        get { return instance; }
    }

    // metodo para registrar un evento
    public void TrackEvent(TrackerEvent e)
    {

        // si no hay sistema de persistencia, no hace nada
        if (persistenceObject == null)
            return;

        // se genera un id unico para el evento
        string eventId = "event_" + eventCount;

        // se asignan datos al evento
        e.setSessionId(sessionId);
        e.setEventId(eventId);

        // se envia el evento al sistema de guardado
        persistenceObject.Send(e);

        // se incrementa el contador de eventos
        eventCount++;
    }

    // metodo para forzar el guardado de datos
    public void flush()
    {
        if (persistenceObject != null)
            persistenceObject.Flush();
    }
}
