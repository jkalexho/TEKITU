using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class State {

    public static readonly int Idle = 0;
    public static readonly int Running = 1;
    public static readonly int Heal = 2;
    public static readonly int Attacking = 3;
    public static readonly int Dashing = 4;
    public static readonly int Hit = 5;
    public static readonly int Pushed = 6;
    public static readonly int Recovering = 7;
    public static readonly int UnstoppableAttack = 8;
    public static readonly int Dying = 9;
    public static readonly int Falling = 10;
    public static readonly int Spawning = 11;

    // Vulture boss states
    public static readonly int Perched = 2;
    public static readonly int Takeoff = 4;
    public static readonly int Diving = 6;
    public static readonly int Invincible = 7;
    public static readonly int SprayAttack = 8;

    public static readonly List<string> toString = new List<string>() { "idle", "run", "heal", "attack", "dash", "hit", "pushed", "recovering", "unstoppableAttack", "dying", "falling", "spawning" }; // don't worry about this. this is for animations
}
