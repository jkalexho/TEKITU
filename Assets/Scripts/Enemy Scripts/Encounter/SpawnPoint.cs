using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public GameObject enemyPrefab;

    public virtual GameObject Spawn()
    {
        return Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
    }
}
