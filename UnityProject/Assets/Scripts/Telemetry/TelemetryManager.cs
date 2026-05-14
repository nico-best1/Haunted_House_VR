using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Este componente manager va guradando en .json todas las sesiones de de telemetria de juego.
/// Crea un thread que se encarga de rellenar el .json periodicamente mediante una cola y un punto de espera (flushIntervalMs)
/// para no gastar tanta CPU.
/// 
/// Si se quiere recoger tods los archivos generados, estará en en esta ruta en Windows:
/// C:/Users/Usuario/AppData/LocalLow/NineandoCorp/HAUNTED HOUSE VR\telemetry_yyyyMMdd_HHmmss.jsonl
/// </summary>
public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    private readonly ConcurrentQueue<TelemetryEvent> queue =
        new ConcurrentQueue<TelemetryEvent>();

    private Thread workerThread;

    private volatile bool running;

    private string filePath;

    private readonly List<TelemetryEvent> batch =
        new List<TelemetryEvent>(128);

    [Header("Config")]
    public int batchSize = 32;

    public int flushIntervalMs = 500;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(
            Application.persistentDataPath,
            $"telemetry_{DateTime.Now:yyyyMMdd_HHmmss}.jsonl"
        );

        running = true;

        workerThread = new Thread(WorkerLoop);
        workerThread.IsBackground = true;
        workerThread.Start();

        Debug.Log($"Telemetry file: {filePath}");
    }

    public string SessionId => sessionId;

    private string sessionId;

    private float sessionStartTime;

    private void Start()
    {
        sessionId = Guid.NewGuid().ToString();

        sessionStartTime = Time.time;

        Track(new SessionStartEvent(sessionId));
    }

    public void Track(TelemetryEvent telemetryEvent)
    {
        queue.Enqueue(telemetryEvent);
    }
    private void WorkerLoop()
    {
        while (running)
        {
            try
            {
                batch.Clear();

                while (
                    batch.Count < batchSize &&
                    queue.TryDequeue(out var telemetryEvent)
                )
                {
                    batch.Add(telemetryEvent);
                }

                if (batch.Count > 0)
                {
                    WriteBatch(batch);
                }

                Thread.Sleep(flushIntervalMs);
            }
            catch (Exception e)
            {
                File.AppendAllText(
                    filePath + ".errors.txt",
                    e.ToString()
                );
            }
        }

        FlushRemaining();
    }

    private void WriteBatch(List<TelemetryEvent> events)
    {
        var builder = new StringBuilder(4096);

        for (int i = 0; i < events.Count; i++)
        {
            string json = JsonUtility.ToJson(
                (object)events[i]
            );

            builder.AppendLine(json);
        }

        File.AppendAllText(filePath, builder.ToString());
    }

    private void FlushRemaining()
    {
        batch.Clear();

        while (queue.TryDequeue(out var telemetryEvent))
        {
            batch.Add(telemetryEvent);
        }

        if (batch.Count > 0)
        {
            WriteBatch(batch);
        }
    }
    private void OnApplicationQuit()
    {
        Shutdown();
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    private void Shutdown()
    {
        if (!running)
            return;

        float duration = Time.time - sessionStartTime;

        Track(new SessionEndEvent(sessionId, duration));

        running = false;

        workerThread?.Join();
    }
}