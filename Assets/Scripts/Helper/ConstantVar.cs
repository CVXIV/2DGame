using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVar {
    public const string HP = "HP";
    public static int MAXHP = 2;
    public const string Volume = "Volume";
    public const string Sound = "Sound";
    public const int groundLayer = 20;
    public const string PlayTag = "Player";
    public const string SkyGroundTag = "SkyGround";
    public const int sceneStart = 0;
    public const int sceneLeven1 = 1;
    public const int sceneLeven2 = 2;
    public const string weapon_key = "weapon_key";

    public enum PlayerStatus {
        IDEL,
        JUMP,
        RUN,
        CROUCH
    }

}