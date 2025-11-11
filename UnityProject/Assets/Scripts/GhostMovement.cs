using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed;

    void Start()
    {
        gameObject.SetActive(true);
        transform.position = startPoint.position;
    }

    public IEnumerator MoveAcross()
    {
        Vector3 end = endPoint.position;

        while (Vector3.Distance(transform.position, end) > 0.01f)
        {
            // Movimiento recto hacia el endpoint
            Vector3 direction = (end - transform.position).normalized;
            Vector3 move = direction * speed * Time.deltaTime;
            transform.position += move;

            yield return null;
        }

        // Asegura posición exacta final
        transform.position = end;

        Destroy(gameObject);
    }
}
