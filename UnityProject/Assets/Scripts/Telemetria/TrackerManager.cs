using UnityEngine;

// clase que gestiona el tracker dentro de unity
public class TrackerManager : MonoBehaviour
{
    Tracker tracker;

    // indica si se guardaran datos en fichero
    [SerializeField]
    bool filePersistence = true;

    // formato en el que se guardaran los datos
    [SerializeField]
    formatType format = formatType.JSON;


    // metodo que se ejecuta al iniciar el objeto
    void Start()
    {
        if (Tracker.Instance == null)
        {
            // ruta donde se guardaran los datos
            string path = Application.persistentDataPath;

            // se genera un id unico para la sesion
            string sessionId = System.Guid.NewGuid().ToString();

            // se inicializa el tracker
            string error = Tracker.Init(sessionId, (int)Time.time, path, filePersistence, format);

            // si hay error, se muestra por consola
            if (error != null)
                Debug.LogWarning(error);
        }

        // se guarda la referencia a la instancia del tracker
        tracker = Tracker.Instance;
    }

    private void Update()
    {
        
    }

    // metodo que se ejecuta al cerrar la aplicacion
    void OnApplicationQuit()
    {
        // se finaliza la sesion del tracker
        Tracker.End((int)Time.time);

        tracker = null;
    }
}
