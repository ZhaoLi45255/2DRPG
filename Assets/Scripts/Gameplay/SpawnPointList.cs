using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointList : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] List<float> spawnX;
    [SerializeField] List<float> spawnY;

    public void SetNewPos()
    {
        player.transform.position = new Vector3(spawnX[PlayerController.spawnIndex], spawnY[PlayerController.spawnIndex], 0);
    }
}