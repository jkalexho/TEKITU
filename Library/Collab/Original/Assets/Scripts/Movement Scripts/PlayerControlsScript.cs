using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsScript : MonoBehaviour
{

    //Controller boolean for mouse or WASD movement. 
    #region Gameplay

    public bool EnableDash { get; set; }
    public bool EnableHeal { get; set; }
    public bool EnableSpecials { get; set; }
    #endregion
    #region Dash variables
    [Header("Dash Settings")]
    public float dashDuration = 0.2f;
    public float dashSpeed = 20;
    public float dashCooldown = 0.5f;
    public float dashTime;
    private Vector3 originalPosition;
    #endregion
    [Space()]
    #region Input Variables

    [Header("Input Settings")]
    public bool useMouseForMovement = false;
    public bool useMousePosForDash = false;
    private MoveBodyScript moveBody;
    private float inputX;
    private float inputY;
    private bool inputDash = false;
    private bool inputClickToMove;
    private bool inputAttack;
    private bool inputEmpower = false;
    private bool disableControls;
    private bool inputHeal = false;
    private bool empowerUsed = false;
    #endregion
    [Space()]
    #region Animations and State Variables
    [Header("Animation Settings")]
    private StateManager stateManager;
    #endregion
    [Space()]
    #region Attack Variables
    [Header("Attack Settings")]
    [Tooltip("The amount of damage the the player gives with one slash (integer value).")]
    public int attackDamage = 1;
    [Tooltip("The duration the sword collider is active")]
    public float attackDuration = 0.5f;
    public float attackCooldown = 1;
    [Tooltip("How long after initial attack that the client listens out for queued attack input.")]
    public float attackQueueLeadTime;
    public float attackSelfOverlap = 0.6f;
    [Tooltip("How fast the character creeps forward when attacking.")]
    public float attackCreepSpeed = 0.1f;

    [Tooltip("READ ONLY. Do not change, else Alex has to draw even more attack poses.")]
    private readonly int comboLength = 3;
    public Collider2D attackCollider;

    //This is only used after 3 attacks, and the player has an attack cooldown on the combo.
    private int attackComboCounter = 0;
    //This is used to track the first two attacks, and reset combo counter if the player has not attacked in a while.
    private float timeSinceLastAttack;
    // If the player presses the attack button before the previous attack finishes, it will queue up the next attack for more smoothness.
    private float timeSinceAttacking;
    // this bool determines if an attack successfully hit something
    private bool attackHit = false;
    // the measure of how edgy the player is


    private bool attackQueued;

    #endregion
    [Space()]
    #region Damage and Health Settings
    [Header("Damage and Health Settings")]
    [Tooltip("Health of the player (readonly)")]
    [SerializeField]
    private int health = 5;
    [Tooltip("How long the player is invulnerable after taking damage.")]
    public float iFrameDuration;
    [Tooltip("How long the player stays pushed down.")]
    public float pushdownDuration;
    [Tooltip("How long the player takes to recover from being pushed down. This + pushdownDuration = duration of invincibility")]
    public float recoveryTime;
    [Tooltip("Is the player invincible?")]
    public bool isInvincible;
    public BoxCollider2D feetCollider;
    public BoxCollider2D bodyCollider;
    #endregion
    [Space()]
    #region Edge Variables
    [Header("Edge Settings")]
    [Tooltip("The maximum edginess that the player can achieve")]
    [SerializeField]
    private int maximumEdge = 3;
    [Tooltip("The duration of the player's edginess")]
    [SerializeField]
    private float edgeCooldown = 3f;
    [Tooltip("The different levels of edge a player can achieve and how much is required for each")]
    [SerializeField]
    private int[] edgeLevels = new int[3] { 4, 6, 8 };
    [Tooltip("The different levels of push strength for the whirlwind attack")]
    [SerializeField]
    private int[] pushStrength = new int[3] { 4, 6, 8 };
    [Tooltip("The different levels of dash damage for the empowered dash attack")]
    [SerializeField]
    private int[] dashDamage = new int[3] { 2, 3, 4 };


    private float edge = 0;
    private float edgePersist; // used as buffer before deciding when the player is out of combat.
    #endregion

    #region Falling Variables
    
    private bool inAir = false;
    #endregion

    void Awake()
    {
        stateManager = this.GetComponent<StateManager>();
        if (stateManager == null)
        {
            Debug.LogError(gameObject.ToString() + ": No state manager found!");
        }
        moveBody = this.GetComponent<MoveBodyScript>();
        if (moveBody == null)
        {
            Debug.LogError(gameObject.ToString() + ": No move body script found!");
        }
        // Tells the game manager that this is the player, so AI scripts can more easily find the player.
        GameManager.SetPlayer(this.gameObject);
    }

    void Start()
    {
        attackCollider.isTrigger = true;
        attackCollider.enabled = false;
        disableControls = false;
        EnableDash = GameManager.gm.enableDash;
        EnableHeal = GameManager.gm.enableHeal;
        EnableSpecials = GameManager.gm.enableSpecials;
    }

    // Update is called once per frame
    void Update()
    {
        if (disableControls == false)
        {
            UpdateInputAxes();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.1f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    void FixedUpdate()
    {
        CheckMovementInput();
        CheckDashInput();
        CheckAttackInput();
        CheckHeal();
        EdgeCooldown();
    }
    #region Input
    //Checks for movement input and forwards call to MoveBodyScript
    void CheckMovementInput()
    {

        if (useMouseForMovement == false)
        {
            if (inputX != 0 || inputY != 0)
            {
                if (stateManager.TrySetState(State.Running))
                {
                    Vector2 direction = new Vector2(inputX, inputY);
                    moveBody.MoveInDirection(direction);
                    stateManager.Direction = direction;
                }
            }
            else
            {
                stateManager.ReturnToIdle(State.Running);
            }
        }
        else
        {
            //alternative movement script by right click mouse (or player's custom key)
            if (inputClickToMove)
            {
                if (stateManager.TrySetState(State.Running))
                {
                    Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    stateManager.Direction = wp - this.transform.position;
                    moveBody.MoveToPoint(wp);
                }
            }
        }
    }
    //Checks for dash input, handles dash cooldowns and forwards call to State Manager
    void CheckDashInput()
    {
        if (inputDash && dashTime <= 0)
        {
            if (stateManager.TrySetState(State.Dashing))
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (useMousePosForDash == true)
                {
                    stateManager.Direction = wp - this.transform.position;
                }
                StopCoroutine("DoDash");
                StartCoroutine("DoDash");
            }
        }
        else
        {
            dashTime -= Time.deltaTime;
            dashTime = Mathf.Clamp(dashTime, 0, dashCooldown);
        }

    }

    //Checks for attack input, handles combo checking and cooldowns and forwards call to state manager.
    void CheckAttackInput()
    {
        if (attackComboCounter == comboLength) // this is outside the other if statements now because it doesn't make sense to check this condition only when the player presses the attack button
        {
            attackComboCounter = 0;
            timeSinceLastAttack = 0;
            attackQueued = false;
        }
        if (inputAttack || attackQueued)
        {
            if (timeSinceLastAttack >= attackCooldown)
            {
                if (timeSinceAttacking > attackQueueLeadTime && attackComboCounter < comboLength) // if the player presses the attack button when they are already attacking, it will queue up another attack.
                {
                    attackQueued = true;
                }
                if (inputEmpower && GetEdgeLevel() > 0 && stateManager.TrySetState(State.UnstoppableAttack))
                {
                    timeSinceAttacking = 0;
                    attackQueued = false;
                    attackComboCounter = 0;
                    stateManager.combo = attackComboCounter;
                    StopCoroutine("DoWhirlwind");
                    StartCoroutine("DoWhirlwind");
                }
                else if (stateManager.TrySetState(State.Attacking))
                {
                    timeSinceAttacking = 0;
                    attackQueued = false;
                    attackComboCounter++;
                    stateManager.combo = attackComboCounter;
                    StopCoroutine("DoAttack");
                    StartCoroutine("DoAttack");
                }
            }
            inputAttack = false;
        }

        ReduceAttackCooldowns();

    }
    #endregion

    #region Accessors/Mutators
    public int GetHealth()
    {
        return health;
    }

    public float GetEdge()
    {
        return edge;
    }

    public void SetEdge(float e)
    {
        edge = Mathf.Min(maximumEdge, e);
    }

    public void HitSuccess()
    {
        attackHit = true;
    }

    public int[] GetEdgeLevels()
    {
        return edgeLevels;
    }
    #endregion

    #region Coroutines
    IEnumerator DoDash()
    {
        originalPosition = this.transform.position;
        float progress = 0;
        Vector2 direction = stateManager.Direction;
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Chasms, true); // ignore collisions between player and chasms
        Physics2D.IgnoreLayerCollision(Layer.Player, Layer.EnemyAttack, true); // ignore collisions between player and enemy attacks
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.EnemyFeet, true); // ignore collisions between player and enemy feet
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.EnemyInAir, true);
        int currentEdge = GetEdgeLevel();
        if (inputEmpower && currentEdge > 0)
        {
            empowerUsed = true;
            attackCollider.enabled = true;
            attackCollider.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            attackCollider.transform.localPosition = direction.normalized * attackSelfOverlap;
            attackCollider.GetComponent<AttackOnCollideScript>().StartDashAttack(dashDamage[currentEdge - 1], 1.75f, 0.05f, 0.2f);
            SpendEdge();
            stateManager.empowered = true;
        }
        while (progress < dashDuration && stateManager.CurrentState == State.Dashing)
        {
            yield return new WaitForFixedUpdate();
            progress += Time.fixedDeltaTime;
            moveBody.MoveInDirection(direction, dashSpeed);
            
        }

        if (stateManager.empowered)
        {
            attackCollider.transform.localScale = new Vector3(1, 1, 1);
            attackCollider.GetComponent<AttackOnCollideScript>().ResetValues();
            attackCollider.enabled = false;
            stateManager.empowered = false;
        }

        
        if (inAir)
        {
            if (stateManager.TrySetState(State.Falling))
            {
                yield return DoFall();
            }
        }

        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Chasms, false); // reactivate collisions between player and chasms
        Physics2D.IgnoreLayerCollision(Layer.Player, Layer.EnemyAttack, false); // reactivate collisions between player and enemy attacks
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.EnemyFeet, false); // ignore collisions between player and enemy feet
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.EnemyInAir, false);

        stateManager.ReturnToIdle(State.Dashing);
        
        dashTime = dashCooldown;
    }

    IEnumerator DoAttack()
    {
        bool successfulHit = false;
        attackHit = false;
        yield return new WaitForSeconds(0.08f);
        float progress = 0;
        while (progress < attackDuration - 0.08f && stateManager.CurrentState == State.Attacking)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;
            attackCollider.enabled = true;

            //This moves the attack collider towards the direction the player is moving.
            Vector2 direction = stateManager.Direction;
            attackCollider.transform.localPosition = direction.normalized * attackSelfOverlap;
            moveBody.MoveInDirection(direction, attackCreepSpeed);

            // checks if the player hit anything
            if (attackHit && !successfulHit)
            {
                edge = Mathf.Clamp(Mathf.RoundToInt(edge + 0.6f), 0, maximumEdge);
                edgePersist = 3;
                successfulHit = true;
            }
        }
        attackCollider.enabled = false;
        stateManager.ReturnToIdle(State.Attacking);
    }

    IEnumerator DoWhirlwind()
    {
        empowerUsed = true;
        int currentEdge = GetEdgeLevel() - 1;
        yield return new WaitForSeconds(0.08f);
        float progress = 0;
        attackCollider.enabled = true;
        attackCollider.transform.localScale = new Vector3(3, 3, 1);
        attackCollider.GetComponent<AttackOnCollideScript>().StartWhirlwindAttack(1, pushStrength[currentEdge], 0.15f, 0.4f);
        SpendEdge();
        stateManager.empowered = true;
        while (progress < attackDuration - 0.08f && stateManager.CurrentState == State.UnstoppableAttack)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;


            //This moves the attack collider towards the direction the player is moving.
            Vector2 direction = stateManager.Direction;
            attackCollider.transform.localPosition = direction.normalized * attackSelfOverlap / 2;
            moveBody.MoveInDirection(direction, attackCreepSpeed);
        }
        attackCollider.transform.localScale = new Vector3(1, 1, 1);
        attackCollider.GetComponent<AttackOnCollideScript>().ResetValues();
        attackCollider.enabled = false;
        stateManager.empowered = false;
        stateManager.ReturnToIdle(State.UnstoppableAttack);
    }

    IEnumerator DoTakeDamage()
    {
        edge -= 1;
        edgePersist = 0;
        float progress = 0;
        //bodyCollider.enabled = false;
        isInvincible = true;

        while (progress < iFrameDuration && stateManager.CurrentState == State.Hit)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;

            //Debug values:

        }
        stateManager.ReturnToIdle(State.Hit);


            //Give player extra .5 seconds after returning to idle to escape.
            yield return new WaitForSeconds(0.5f);

            //bodyCollider.enabled = true;
            isInvincible = false;
        
    }

    IEnumerator DoPushedDown()
    {
        originalPosition = this.transform.position;
        float progress;
        bodyCollider.enabled = false;
        isInvincible = true;


        // this segment checks if the player is still moving from the push effect
        Vector3 position;
        Vector3 position2 = this.transform.position;
        // we need two of these because the body's position needs that many in order to update for the first time.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        position = position2;
        position2 = this.transform.position;

        while (position != position2 && stateManager.CurrentState == State.Pushed)
        {
            yield return new WaitForFixedUpdate();
            position = position2;
            position2 = this.transform.position;
        }

        // implement code to enable falling here
        if (inAir)
        {
            if (stateManager.TrySetState(State.Falling))
            {
                yield return DoFall();
            }
        }
        else
        {
            yield return new WaitForSeconds(pushdownDuration); //this variable determines the delay between when the player lands and when they get back up.
            
            progress = 0;
            
            if (stateManager.TrySetState(State.Recovering))
            {
                while (progress < recoveryTime && stateManager.CurrentState == State.Recovering)
                {
                    yield return new WaitForFixedUpdate();
                    progress += Time.fixedDeltaTime;
                    //print("recovering");
                }

                stateManager.ReturnToIdle(State.Recovering);

                yield return new WaitForSeconds(0.5f);
                
            }
        }
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Chasms, false);
        bodyCollider.enabled = true;
        isInvincible = false;
    }

    IEnumerator DoDie()
    {
        float progress = 0;
        Time.timeScale = 0.5f;
        while (progress < iFrameDuration && stateManager.CurrentState == State.Dying)
        {
            yield return new WaitForFixedUpdate();

            progress += Time.fixedDeltaTime;
            feetCollider.enabled = false;
            bodyCollider.enabled = false;
            isInvincible = true;
            //Debug values:

        }

        //Flash red
        //time scale .1
        //Reset scene?
        
        yield return new WaitForSeconds(2);
        Time.timeScale = 1;
        GameManager.Reset();
    }

    IEnumerator DoFall()
    {
        yield return new WaitForSeconds(1.5f);
        stateManager.ReturnToIdle(State.Falling);
        TakeDamage(2);
        stateManager.newHit = true;
        if (stateManager.TrySetState(State.Recovering))
        {
            this.transform.position = originalPosition;
            yield return new WaitForSeconds(recoveryTime);
            stateManager.ReturnToIdle(State.Recovering);
            inAir = false;
        }
    }
    #endregion

    #region Hit and Damaged
    public void Hit(int damage)
    {
        if (isInvincible == false)
        {
            if (stateManager.TrySetState(State.Hit))
            {
                TakeDamage(damage);
                StartCoroutine(DoTakeDamage());
            }
        }
    }

    public void Hit(int damage, Vector2 direction)
    {
        if (isInvincible == false)
        {
            if (stateManager.TrySetState(State.Hit))
            {
                TakeDamage(damage);
                stateManager.HitDirection = direction;
                StartCoroutine(DoTakeDamage());
            }
        }
    }

    //Hit method used for stronger attacks, can potentially push you off chasms.
    public void Hit(int damage, Vector2 direction, float pushStrength = 0)
    {
        if (isInvincible == false)
        {
            if (stateManager.TrySetState(State.Pushed))
            {
                Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Chasms, true); // ignore collisions between player and chasms
                TakeDamage(damage);
                StartCoroutine(DoPushedDown());
                stateManager.HitDirection = direction;
                moveBody.PushInDirection(direction, pushStrength);
                stateManager.Direction = -direction;
            }
        }
    }

    private void TakeDamage(int damage)
    {
        stateManager.damaged = true;
        health -= damage;
        
        if (health <= 0)
        {
            if (stateManager.TrySetState(State.Dying))
            {
                DisableControls();
                print("dying");
                bodyCollider.enabled = false;
                StartCoroutine(DoDie());
            }
        }
    }
    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
         if (other.gameObject.layer == Layer.Chasms && !inAir) // detects if the player is on the edge of the platform
         {
            originalPosition = this.transform.position;
            
         }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
            inAir = false;
        
    }

    // checks if the player is in the air or not
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == Layer.Chasms) 
        {
            if (other.bounds.Contains(feetCollider.bounds.min) && other.bounds.Contains(feetCollider.bounds.max))
            {
                inAir = true;
            } else
            {
                inAir = false;
            }
        }
    }

    #region Helper Functions
    private void SpendEdge()
    {
        edge -= 4;
    }

    private void CheckHeal()
    {
        if (inputHeal)
        {
            if (!empowerUsed && health < 5 && GetEdgeLevel() > 0)
            {
                if (stateManager.TrySetState(State.Heal))
                {
                    health += 1;
                    SpendEdge();
                    stateManager.ReturnToIdle(State.Heal);
                }
            }
            empowerUsed = false;
        }
    }

    private void Fall()
    {
        StartCoroutine("DoFall");
    }
    private int GetEdgeLevel()
    {
        int x = 0;
        for (int i = 0; i < edgeLevels.Length; i++)
        {
            if (edge >= edgeLevels[i])
            {
                x++;
            }
        }
        return x;
    }
    //Used for the MVP playtest.
    public void ToggleMovement(bool input)
    {
        useMouseForMovement = input;
    }
    //Used for the MVP playtest.
    public void ToggleDashToMouse(bool input)
    {
        useMousePosForDash = input;
    }

    void ReduceAttackCooldowns()
    {
        //We reduce the cooldown for attack time every frame. This is only applicable after the player attacks his last combo attack and has to wait.
        timeSinceLastAttack += Time.fixedDeltaTime;
        timeSinceLastAttack = Mathf.Clamp(timeSinceLastAttack, 0, attackCooldown);

        //Here, we reset the combo when we don't attack for attackCooldown seconds.
        timeSinceAttacking += Time.fixedDeltaTime;
        timeSinceAttacking = Mathf.Clamp(timeSinceAttacking, 0, attackCooldown);
        if (timeSinceAttacking >= attackCooldown)
        {
            attackComboCounter = 0;
        }
    }

    void UpdateInputAxes()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        inputClickToMove = Input.GetButtonDown("MouseClickMovement");
        inputAttack = Input.GetButton("Attack");
        if (EnableDash)
        {
            inputDash = Input.GetButton("Dash");
        }
        if (EnableSpecials)
        {
            inputEmpower = Input.GetButton("Empower");
        }
        if (EnableHeal)
        {
            inputHeal = Input.GetButtonUp("Empower");
        }
    }

    void EdgeCooldown()
    {
        if (edge > 0)
        {
            if (edgePersist > 0)
            {
                edgePersist -= Time.fixedDeltaTime / edgeCooldown;
            }
            else
            {
                edge -= Time.fixedDeltaTime / edgeCooldown;
            }
        }
    }

    void DisableControls()
    {
        disableControls = true;
    }
    #endregion
}
