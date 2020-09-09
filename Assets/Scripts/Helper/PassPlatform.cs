using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlatform : MonoBehaviour {
    private PlatformEffector2D effect;
    private int layer;

    public bool ReadyToGround { get; set; } = false;

    private void Awake() {
        effect = GetComponent<PlatformEffector2D>();
    }

    public void Pass(GameObject gameObject) {
        layer = 1 << gameObject.layer;
        effect.colliderMask &= ~layer;
        Invoke(nameof(ResetLayer), 0.5f);
    }

    private void ResetLayer() {
        effect.colliderMask |= layer;
    }

}