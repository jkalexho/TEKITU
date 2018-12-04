using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyEncounter : MonoBehaviour {

	public EnemySpawner spawner;

    public BoxCollider2D encounterZone;

    public bool Active { get; set; } // while Active, the spawner will spawn its waves and the barriers will be active. While not active, the barriers will be deactivated and the spawner won't spawn anything

    public bool Encountered { get; set; } // becomes true when the encounter starts. Stays true after the player defeats the encounter. Becomes false when the player dies and resets.

    public GameObject barriers;

    public Transform restartPoint;

    public SpecialEvent specialEvent;

    private BarrierScript[] bs;

	// Use this for initialization
	protected virtual void Start () {
		spawner = this.transform.GetComponentInChildren<EnemySpawner>();
        bs = barriers.GetComponentsInChildren<BarrierScript>();
        Active = false;
        Encountered = false;
	}

    protected virtual void Update()
    {
        if (!Encountered)
        {
            if (CheckPlayerEntered())
            {
                StartEncounter();
            }
        }
        if (Active)
        {
            if (spawner.IsDone())
            {
                if (specialEvent == null)
                {
                    ClearEncounter();
                } else
                {
                    if (specialEvent.Done)
                    {
                        ClearEncounter();
                    } else if (!specialEvent.Active)
                    {
                        specialEvent.Activate();
                    }
                }
            } else
            {
                StartCoroutine(spawner.SpawnWaves());
            }
        }
    }
    
    protected void StartEncounter()
    {
        GameManager.SetEnemyEncounter(this);
        Encountered = true;
        Activate();
    }

    protected void ClearEncounter()
    {
        GameManager.ClearEnemyEncounter();
        Deactivate();
    }

    public virtual void Activate()
    {
        Active = true;
        foreach (BarrierScript b in bs)
        {
            b.Activate();
        }
    }

    public virtual void Deactivate()
    {
        Active = false;
        foreach (BarrierScript b in bs)
        {
            b.Deactivate();
        }
    }

    public virtual void Reset()
    {
        Encountered = false;
        Deactivate();
        spawner.Reset();
    }

    protected bool CheckPlayerEntered()
    {
        return encounterZone.bounds.Contains(GameManager.player.transform.position - new Vector3(0, 0.5f,0));
    }
    
}
