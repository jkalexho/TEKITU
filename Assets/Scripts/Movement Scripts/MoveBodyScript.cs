using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBodyScript : MonoBehaviour {

    #region Editor Variables
    [SerializeField]
    [Tooltip("The speed of the body.")]
    private float speed;
    [SerializeField]
    [Tooltip("The mass of the body, which affects the strength of things that push it.")]
    private float mass = 1;
    #endregion

    #region Private Variables
    private Rigidbody2D body;
    private Vector2 destination;
    private StateManager stateManager;

    private Vector2 direction;
    #endregion

    #region Awake/Start/Update
    void Awake()
    {
        body = this.GetComponent<Rigidbody2D>();
        if (body == null)
        {
            Debug.LogError(gameObject.ToString() + ": No rigidbody found!");
        }
        stateManager = this.GetComponent<StateManager>();
        if (stateManager == null)
        {
            Debug.LogError(gameObject.ToString() + ": No state manager found!");
        }

        direction = new Vector2(0, 0);
    }
    
    #endregion

    #region public functions
    // Takes in a destination and starts a coroutine that moves the object towards the point. The coroutine moves the object on FixedUpdates
    public void MoveToPoint(Vector2 destination)
    {
        if (stateManager.CurrentState <= State.Running)
        {
            StopCoroutine("DoMoveToPoint");
            StartCoroutine("DoMoveToPoint", new Vector3(destination.x, destination.y, 0));
        }
    }

    // move in direction using the object's built-in speed. Call this function in a FixedUpdate()
    public void MoveInDirection(Vector2 dir)
    {
        MoveInDirection(dir, speed);
    }

    // move in direction using a custom input speed. Call this function in a FixedUpdate()
    public void MoveInDirection(Vector2 dir, float spd)
    {
        if (direction != dir)
        {
            direction = dir.normalized;
            //direction.y *= 0.9f;
        }
        body.MovePosition(body.position + direction * spd * Time.fixedDeltaTime);
    }

    // Starts a coroutine that will simulate pushing the object, modified by its mass 
    public void PushInDirection(Vector2 dir, float pushStrength)
    {
        stateManager.HitDirection = dir;
        stateManager.PushDuration = pushStrength / mass;
        StopCoroutine("DoPush");
        StartCoroutine("DoPush", new Vector3(dir.x, dir.y, pushStrength));
    }
    #endregion

    #region Accessors/Mutators
    public Vector2 Direction
    {
        get
        {
            return this.direction;
        }
        set
        {
            this.direction = value.normalized;
        }
    }

    public float Speed
    {
        get
        {
            return this.speed;
        }
        set
        {
            this.speed = Mathf.Abs(value);
        }
    }

    public float GetMass()
    {
        return mass;
    }
    #endregion

    #region Helper Functions and Coroutines
    // for pathfinding, if we need it. A little messy right now, as it touches the StateManagerScript
    public IEnumerator DoMoveToPoint(Vector3 destination)
    {
        direction = (destination - transform.position).normalized;
        stateManager.Direction = direction;
        while (transform.position != destination && stateManager.CurrentState == State.Running)
        {
            yield return new WaitForFixedUpdate();
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
        }
        stateManager.ReturnToIdle(State.Running);
        yield return null;
    }

    // for pushing the object. This communicates with the StateManager.
    IEnumerator DoPush(Vector3 data)
    {
        Vector2 direction = new Vector2(data.x, data.y).normalized;
        float strength = data.z;
        while (strength > 0 && stateManager.CurrentState == State.Pushed)
        {
            yield return new WaitForFixedUpdate();
            body.MovePosition(body.position + direction * strength * Time.fixedDeltaTime);
            strength -= mass * Time.fixedDeltaTime;
        }
        yield return null;
    }
    #endregion

}
