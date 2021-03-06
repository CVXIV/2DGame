﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVar {
    public const string HP = "HP";
    public static int MAXHP = 5;
    public const string Volume = "Volume";
    public const string Sound = "Sound";
    public const int groundLayer = 20;
    public const string GroundLayerName = "ground";
    public const string PlayLayer = "Player";
    public const string EnemyLayer = "Enemy";
    public const string ThirdEnemyLayer = "ThirdEnemy";
    public const string ListenLayer = "ListenRange"; 
    public const string AttackLayer = "AttackRange";
    public const string SkyGroundTag = "SkyGround";
    public const string BulletLayer = "Bullet";
    public const string EnemyBulletLayer = "EnemyBullet";
    public const string IgnoreLayer = "IgnoreLayer";
    public const int sceneStart = 0;
    public const int sceneLeven1 = 1;
    public const int sceneLeven2 = 2;
    public const string sceneStartName = "level0";
    public const string sceneLeven1Name = "level1";
    public const string sceneLeven2Name = "level2";
    public const string PauseMenuName = "PauseMenus";

    // tag
    public const string PlayTag = "Player";
    public const string TriggerTag = "Trigger";
    public const string DoorTag = "door";
    public const string MovePlatformTag = "moveplatform";
    public const string groundLayerTag = "StaticGround";


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