using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Layer {
    public static int Walls = LayerMask.NameToLayer("Walls");
    public static int Chasms = LayerMask.NameToLayer("Chasms");
    public static int Player = LayerMask.NameToLayer("Player");
    public static int PlayerFeet = LayerMask.NameToLayer("PlayerFeet");
    public static int Enemies = LayerMask.NameToLayer("Enemy");
    public static int EnemyFeet = LayerMask.NameToLayer("EnemyFeet");
    public static int EnemyAttack = LayerMask.NameToLayer("EnemyAttack");
    public static int EnemyInAir = LayerMask.NameToLayer("EnemyInAir");
    public static int EnemyFlying = LayerMask.NameToLayer("EnemyFlying");
    public static int PlayerAttack = LayerMask.NameToLayer("PlayerAttack");
    public static int EnemySwimming = LayerMask.NameToLayer("EnemySwimming");
    public static int CameraZone = LayerMask.NameToLayer("CameraZone");

    public static int SortPlatforms = -4;
    public static int SortShadows = -2;
    public static int SortDeadBodies = -1;
    public static int SortObjects = 0;
}
