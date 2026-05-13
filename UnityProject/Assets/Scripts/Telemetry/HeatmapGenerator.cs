using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ZonaHeatmap
{
    public string nombreZona;
    public BoxCollider boundsCollider; // Collider que define la habitación
    public int[,] tensionMap;          // Cuadrícula de esta zona concreta
    public int[,] attentionMap;          // Cuadrícula de esta zona concreta
    public float tamañoCelda = 0.5f;   // Resolución (ej. 50cm por celda)

    [HideInInspector] public float minX;
    [HideInInspector] public float minZ;
}

public class HeatmapGenerator : MonoBehaviour
{
    List<ZonaHeatmap> zonas;

    public void InicializarZona(ZonaHeatmap zona)
    {
        //Guardamos la esquina inferior izquierda
        zona.minX = zona.boundsCollider.bounds.min.x;
        zona.minZ = zona.boundsCollider.bounds.min.z;

        //Medimos el ancho y largo total
        float ancho = zona.boundsCollider.bounds.size.x;
        float largo = zona.boundsCollider.bounds.size.z;

        // Calculamos cuántas columnas y filas necesitamos (redondeando hacia arriba)
        int columnas = Mathf.CeilToInt(ancho / zona.tamañoCelda);
        int filas = Mathf.CeilToInt(largo / zona.tamañoCelda);

        // Creamos la matriz vacía con ese tamaño
        zona.tensionMap = new int[columnas, filas];
    }
    public void RegistrarEventoEnZona(ZonaHeatmap zona, Vector3 posicionEvento)
    {
        //Calculamos la distancia desde el inicio de nuestra cuadrícula (minX, minZ) hasta el evento
        float distanciaX = posicionEvento.x - zona.minX;
        float distanciaZ = posicionEvento.z - zona.minZ;

        //Dividimos esa distancia por el tamaño de celda y truncamos los decimales para obtener el índice entero
        int columna = Mathf.FloorToInt(distanciaX / zona.tamañoCelda);
        int fila = Mathf.FloorToInt(distanciaZ / zona.tamañoCelda);

        // Si un evento cae exactamente en el borde máximo matemático del collider, 
        // el índice podría salirse del array por un píxel. Usamos Clamp para evitar errores.
        int maxColumna = zona.tensionMap.GetLength(0) - 1;
        int maxFila = zona.tensionMap.GetLength(1) - 1;

        columna = Mathf.Clamp(columna, 0, maxColumna);
        fila = Mathf.Clamp(fila, 0, maxFila);

        // Sumamos 1 a la intensidad de esa cuadrícula
        zona.tensionMap[columna, fila] += 1;

        Debug.Log($"Evento registrado en {zona.nombreZona} -> Celda [{columna}, {fila}]");
    }
    public void RegistrarEvento()
    {
        Vector3 posicionEvento = new Vector3();

        foreach (ZonaHeatmap zona in zonas)
        {
            if (zona.boundsCollider.bounds.Contains(posicionEvento))
            {
                // ¡Bingo! El evento ocurrió en esta habitación.
                // Ahora conviertes la posición X,Z a la cuadrícula local de esta zona
                // y le sumas +1 de intensidad.
                RegistrarEventoEnZona(zona, posicionEvento);
                break; // Pasamos al siguiente evento
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        zonas = new List<ZonaHeatmap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
