using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalWallTrigger : MonoBehaviour
{
    public GameObject rearWall; // La pared que aparece
    public static bool wallEventTriggered = false; // Variable global compartida

    private void OnTriggerEnter(Collider other)
    {
        if (!wallEventTriggered && other.CompareTag("Player"))
        {
            rearWall.SetActive(true);
            wallEventTriggered = true;
            gameObject.SetActive(false); // Desactiva el trigger tras activarse
        }
    }
}
