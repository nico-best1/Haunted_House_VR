using UnityEngine;
using UnityEngine.Analytics;

// clase que gestiona el tracker dentro de unity
public class TrackerManager : MonoBehaviour
{
    Tracker tracker;

    [SerializeField]
    int flushTimeSecond = 5;
    float deltaTime = 0;

    // indica si se guardaran datos en fichero
    [SerializeField]
    bool filePersistence = true;

    // formato en el que se guardaran los datos
    [SerializeField]
    formatType format = formatType.JSON;

    [SerializeField]
    float controllerTrigger = 15.0f; //En metros/segundos

    [SerializeField]
    float headTrigger = 4.0f; //En radianes/segundos

    [SerializeField]
    Transform head;

    Quaternion lastRotation;
    Vector3 angularVelocity;

    [SerializeField]
    Transform controllerRight;

    [SerializeField]
    Transform controllerLeft;

    [SerializeField]
    RoomSensor[] rooms;

    string currentRoom;

    Vector3 controllerLeftLastPosition;
    Vector3 controllerLeftVelocity;
    Vector3 controllerRightLastPosition;
    Vector3 controllerRightVelocity;

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
            string error = Tracker.Init(sessionId, (int)Time.time*1000, path, filePersistence, format);

            // si hay error, se muestra por consola
            if (error != null)
                Debug.LogWarning(error);
        }

        // se guarda la referencia a la instancia del tracker
        tracker = Tracker.Instance;

        lastRotation = head.rotation;
        controllerLeftLastPosition = controllerLeft.position;
        controllerRightLastPosition = controllerRight.position;

        foreach (RoomSensor room in rooms)
        {
            room.Init(this);
        }

        currentRoom = "";
    }

    void Update()
    {
        //Flush cada cierto tiempo
        deltaTime += Time.deltaTime;

        if(deltaTime > flushTimeSecond)
        {
            tracker.flush();
            deltaTime = 0;
        }

        //Comprobacion del movimiento brusco en mandos
        controllerLeftVelocity = (controllerLeft.position - controllerLeftLastPosition) / Time.deltaTime;

        controllerRightVelocity = (controllerRight.position - controllerRightLastPosition) / Time.deltaTime;

        if (controllerLeftVelocity.magnitude > controllerTrigger || controllerRightVelocity.magnitude > controllerTrigger)
        {
            PositionEvent p = new PositionEvent(head.position.x, head.position.y, head.position.z);
            tracker.TrackEvent(new ProgresionEvent("Quick_Jitter_Move", (int)(Time.time * 1000f), p));
        }

        controllerLeftLastPosition = controllerLeft.position;
        controllerRightLastPosition = controllerRight.position;

        //Comprobacion del movimiento bruco en cabeza
        Quaternion delta = head.rotation * Quaternion.Inverse(lastRotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;


        if(angularVelocity.magnitude > headTrigger)
        {
            PositionEvent p = new PositionEvent(head.position.x, head.position.y, head.position.z);
            tracker.TrackEvent(new ProgresionEvent("Quick_HMD_Move", (int)(Time.time * 1000f), p));
        }
        lastRotation = head.rotation;

    }

    public void OnRoomEnter(string roomName)
    {
        if (roomName.Equals(currentRoom)) return;

        tracker.TrackEvent(new TrackerEvent("Enter_" + roomName, (int)(Time.time * 1000f)));
        currentRoom = roomName;
    }

    // metodo que se ejecuta al cerrar la aplicacion
    void OnApplicationQuit()
    {
        // se finaliza la sesion del tracker
        Tracker.End((int)Time.time*1000);

        tracker = null;
    }
}
