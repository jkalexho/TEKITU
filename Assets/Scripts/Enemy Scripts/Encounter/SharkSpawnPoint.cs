using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkSpawnPoint : SpawnPoint {

    [SerializeField]
    private Transform centerPoint;

    public override GameObject Spawn()
    {
        GameObject shark = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
        SharkScript ss = shark.GetComponent<SharkScript>();
        ss.SetCenterPoint(centerPoint.position);
        return shark;
    }
}
