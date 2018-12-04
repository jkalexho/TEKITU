using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    // The Game Manager class holds global data, such as the GameObject for the player. The AI can access the Game Manager singleton to find the player location.
    // We can also add other variables here, such as the number of existing enemies, and more.

    public static GameManager gm;
    public static GameObject player;

    public bool enableDebugMenu = false;
    public GameObject debugMenu;

    public static ScreenFlash screenFlasher;

    public static HealthbarScript hpScript;

    private static PlayerControlsScript pc;


    void Start()
    {
        if (gm == null)
        {
            gm = this;
        } else {
            Destroy(this.gameObject);
        }
        Physics2D.IgnoreLayerCollision(Layer.PlayerFeet, Layer.Enemies);
        Physics2D.IgnoreLayerCollision(Layer.EnemyFeet, Layer.Player);
        //Physics2D.IgnoreLayerCollision(Layer.Player, Layer.Chasms, true); // ignore collisions between player hitbox and chasms
        
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
        } else if (debugMenu != null) {
            debugMenu.SetActive(false);
        }
        if (hpScript != null)
        {
            hpScript.health = pc.GetHealth();
            hpScript.edge = pc.GetEdge();
        }
    }
}
