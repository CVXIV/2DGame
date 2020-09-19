using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVar {
    public const string HP = "HP";
    public static int MAXHP = 5;
    public const string Volume = "Volume";
    public const string Sound = "Sound";
    public const int groundLayer = 20;
    public const string PlayTag = "Player";
    public const string PlayLayer = "Player";
    public const string EnemyLayer = "Enemy";
    public const string ListenLayer = "ListenRange"; 
    public const string AttackLayer = "AttackRange";
    public const string SkyGroundTag = "SkyGround";
    public const string BulletLayer = "Bullet";
    public const string IgnoreLayer = "IgnoreLayer";
    public const int sceneStart = 0;
    public const int sceneLeven1 = 1;
    public const int sceneLeven2 = 2;

    // tag
    public const string TriggerTag = "Trigger";

    // 内存数据的key
    public const string weapon_key = "weapon_key";
    public const string player_hp = "player_hp";

    public enum PlayerStatus {
        IDEL,
        JUMP,
        RUN,
        CROUCH
    }

}