using UnityEngine;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

public class CreepySoundsController : MonoBehaviour
{
    [Header("Reproducción Aleatoria")]
    public float minDelay = 10f;
    public float maxDelay = 20f;

    [Header("Distancia del sonido al jugador")]
    public float minRadius = 2f;
    public float maxRadius = 7f;

    [Header("FMOD PARAMETRO")]
    public string creepyParam = "CreepySoundChance";
    public float paramValue = 1f;

    public Transform player;

    private Coroutine creepyRoutine;
    private bool isActive = false;

    public void StartCreepySounds()
    {
        if (!isActive)
        {
            isActive = true;
            creepyRoutine = StartCoroutine(RandomCreepyRoutine());
        }
    }

    public void StopCreepySounds()
    {
        isActive = false;

        if (creepyRoutine != null)
        {
            StopCoroutine(creepyRoutine);
            creepyRoutine = null;
        }
    }

    private IEnumerator RandomCreepyRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            if (isActive) // comprobamos de nuevo antes de reproducir
                PlayCreepySound();
        }
    }

    private void PlayCreepySound()
    {
        Vector3 pos = player.position + Random.onUnitSphere * Random.Range(minRadius, maxRadius);

        EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.creepySounds);
        instance.setParameterByName(creepyParam, paramValue);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
        instance.start();
        instance.release();
    }

    public void SetCreepyChance(float value)
    {
        paramValue = value;
    }
}
