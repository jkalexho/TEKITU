using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    // The Game Manager class holds global data, such as the GameObject for the player. The AI can access the Game Manager singleton to find the player location.
    // We can also add other variables here, such as the number of existing enemies, and more.

    public GameObject playerPrefab;

    public bool enableDash = false;
    public bool enableHeal = false;
    public bool enableSpecials = false;

    public static GameManager gm;
    public static GameObject player;

    public bool enableDebugMenu = false;
    public GameObject debugMenu;

    public static ScreenFlash screenFlasher;

    public static HealthbarScript hpScript;

    public static PlayerControlsScript pc;

    public static FollowCam followCamScript;

    public static CameraZone cameraZone;

    public static EnemyEncounter currentEncounter;

    public static bool freeCamera = false;

    public static TooltipBackgroundAnimator tooltipBGAnimator;

    public static int prevHealth = 5;

    public static float prevEdge = 0;

    public Vector3 restartPoint;

    public static ContactFilter2D chasmFilter;

    void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // feet
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Enemies);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFeet, Layer.Player);

        Physics2D.IgnoreLayerCollision(Layer.EnemyInAir, Layer.Chasms);
        //flying enemy
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.Chasms);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.PlayerFeet);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.Player);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.EnemyFeet);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.EnemyInAir);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFlying, Layer.EnemyFlying);
        // player attack
        Physics2D.IgnoreLayerCollision(Layer.PlayerAttack, Layer.Chasms);
        Physics2D.IgnoreLayerCollision(Layer.PlayerAttack, Layer.Walls);
        Physics2D.IgnoreLayerCollision(Layer.PlayerAttack, Layer.EnemyAttack);
        Physics2D.IgnoreLayerCollision(Layer.PlayerAttack, Layer.EnemyFeet);
        Physics2D.IgnoreLayerCollision(Layer.EnemyAttack, Layer.PlayerFeet);
        // swimming enemy
        Physics2D.IgnoreLayerCollision(Layer.EnemySwimming, Layer.PlayerFeet);
        Physics2D.IgnoreLayerCollision(Layer.EnemySwimming, Layer.Player);
        Physics2D.IgnoreLayerCollision(Layer.EnemySwimming, Layer.EnemyFeet);
        Physics2D.IgnoreLayerCollision(Layer.EnemySwimming, Layer.EnemyInAir);
        Physics2D.IgnoreLayerCollision(Layer.EnemySwimming, Layer.EnemyFlying);
        // camera zone
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.Enemies);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.PlayerFeet);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.PlayerAttack);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.EnemyAttack);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.EnemyFeet);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.EnemyInAir);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.EnemyFlying);
        Physics2D.IgnoreLayerCollision(Layer.CameraZone, Layer.EnemySwimming);
        chasmFilter = new ContactFilter2D
         {
             layerMask = Layer.Chasms,
             useLayerMask = true,
             useTriggers = true
         };
        //Physics2D.IgnoreLayerCollision(Layer.Player, Layer.Chasms, true); // ignore collisions between player hitbox and chasms

    }

    public static void SetTooltipBackground(TooltipBackgroundAnimator bg)
    {
        tooltipBGAnimator = bg;
    }

    public static void Reset()
    {
        if (currentEncounter != null)
        {
            currentEncounter.Reset();
        }
        Destroy(player);
        screenFlasher.Reset();
        player = Instantiate(gm.playerPrefab, gm.restartPoint, Quaternion.identity);
        pc = player.GetComponent<PlayerControlsScript>();
        pc.SetEdge(prevEdge);
        prevHealth++; // heal for 1 each time the player resets
        pc.SetHealth(prevHealth); 
    }

    public static void SetEnemyEncounter(EnemyEncounter ee)
    {
        currentEncounter = ee;
        gm.restartPoint = currentEncounter.restartPoint.position;
        prevHealth = pc.GetHealth();
        prevEdge = pc.GetEdge();
    }

    public static void ClearEnemyEncounter()
    {
        currentEncounter = null;
    }

    public static void SetCameraZone(CameraZone cz)
    {
        cameraZone = cz;
        freeCamera = false;
    }

    public static void ClearCameraZone(CameraZone cz)
    {
        if (cameraZone == cz)
        {
            ClearCameraZone();
        }
    }

    public static void ClearCameraZone()
    {
        cameraZone = null;
        freeCamera = true;
    }

    public static void SetPlayer(GameObject p)
    {
        player = p;
        pc = player.GetComponent<PlayerControlsScript>();
        SynchronizePlayerAndHealthbar();
    }

    public static void SetHealthBarScript(HealthbarScript hp)
    {
        hpScript = hp;
        SynchronizePlayerAndHealthbar();
    }

    public static void SetFollowCam(FollowCam fc)
    {
        followCamScript = fc;
    }

    private static void SynchronizePlayerAndHealthbar()
    {
        if (player != null && hpScript != null)
        {
            hpScript.SetEdgeLevels(pc.GetEdgeLevels());
        }
    }

    public static void SetScreenFlasher(GameObject s)
    {
        screenFlasher = s.GetComponent<ScreenFlash>();
    }

    void Update()
    {
        if (enableDebugMenu == true){
            debugMenu.SetActive(true);       
        } else if (enableDebugMenu == false && debugMenu != null) {
            debugMenu.SetActive(false);
        }
        if (hpScript != null)
        {
            hpScript.health = pc.GetHealth();
            hpScript.edge = pc.GetEdge();
        }
        if (cameraZone != null && !freeCamera)
        {
            followCamScript.desiredPos = cameraZone.Pos;
        }
    }
}
