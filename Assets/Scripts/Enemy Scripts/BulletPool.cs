using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int count;

    private int index = -1;

    private PoolShotScript[] pool;

    void Awake()
    {
        pool = new PoolShotScript[count];
    }

	// Use this for initialization
	void Start () {
		for (int i = 0; i < count; i++)
        {
            pool[i] = Instantiate(prefab, this.transform).GetComponent<PoolShotScript>();
        }
	}

    public void Reset()
    {
        foreach(PoolShotScript p in pool)
        {
            p.Deactivate();
            index = -1;
        }
    }
	
    public PoolShotScript Spawn(Vector3 location, Vector2 direction)
    {
        index++;
        if (index >= count)
        {
            index = 0;
        }
        pool[index].gameObject.SetActive(true);
        pool[index].Shoot(location, direction);
        return pool[index];
    }
}
