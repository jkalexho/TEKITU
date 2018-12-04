using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

    [SerializeField]
    [Tooltip("The cardinal direction of the body. (6 is South)")]
    private int cardinal = 6;

    private int state = 0;

    // tells the animation controller that there is a new dash
    public bool newDash = false;

    // tells the animation controller that there is a new attack
    public bool newAttack = false;

    // tells the animation controller that there is a new push
    public bool newPush = false;

    // tells the animation controller that there is a new hit
    public bool newHit = false;

    public bool dead = false;

    public bool damaged = false;

    public bool fall = false;

    public int combo = 0;

    public bool empowered = false;

    public bool newHeal = false;

    public int CurrentState
    {
        get
        {
            return state;
        }
        set
        {
            state = Mathf.Clamp(value, 0, 99);
        }
    }

    public int CardinalDirection
    {
        get
        {
            return this.cardinal;
        }
        set
        {
            this.cardinal = value;
            NormalizeCardinal();
        }
    }

    public Vector2 Direction { get; set; }

    public Vector2 HitDirection { get; set; }

    public float PushDuration { get; set; }

    // Call this function after an action has finished, and pass in the original state that called the action.
    public void ReturnToIdle(int fromState)
    {
        if (fromState >= state)
        {
            CurrentState = State.Idle;
        }
    }

    // attempts to set the state to s. On success, it returns true. 
    /* Use this calling convention:
     * 
     * if (stateManager.TrySetState(State.Running)) {
     *     //...Your Code Here
     * }
    */
    
    public bool TrySetState(int stateToTry)
    {
        // if statement
        if (stateToTry >= state && Exceptions(stateToTry))
        {
            state = stateToTry;
            if (state == State.Dashing)
            {
                newDash = true;
            } else if (state == State.Attacking || state == State.UnstoppableAttack)
            {
                newAttack = true;
            } else if (state == State.Pushed)
            {
                newPush = true;
            } else if (state == State.Hit)
            {
                newHit = true;
            } else if (state == State.Dying)
            {
                dead = true;
            } else if (state == State.Falling)
            {
                fall = true;
            } else if (state == State.Heal)
            {
                newHeal = true;
            }
            return true;
        }
        return false;
    }

    void Awake()
    {
        Direction = Cardinal.CardinalToVector(cardinal);
    }

    #region Helper Functions and Coroutines
    private void NormalizeCardinal()
    {
        if (cardinal < 0 || cardinal > 7)
        {
            cardinal = MathZ.Modulo(cardinal, 8);
        }
    }

    // returns true if it passes all the exceptions. false otherwise
    private bool Exceptions(int stateToTry)
    {
        return !(
            ((stateToTry == State.Dashing || stateToTry == State.Attacking || stateToTry == State.UnstoppableAttack) && (state == State.Dashing || state == State.Attacking || state == State.UnstoppableAttack)) || // if the player is attacking or dashing already, they can't overwrite it with another attack or dash
            (stateToTry == state && state == State.Dying) // can't die more than once
            );
    }
    #endregion
}
