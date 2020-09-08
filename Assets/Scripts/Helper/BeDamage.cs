using System;
using UnityEngine;

public class BeDamage : MonoBehaviour {
    public int health;
    public Action onHurt;
    public Action onDead;
    private bool isEnable = true;

    public void Enable() {
        isEnable = true;
    }

    public void Disable() {
        isEnable = false;
    }

    public void TakeDamage(int value) {
        if (isEnable) {
            // 减少血量
            health -= value;
            // 播放受伤动画
            if (health <= 0) {
                onDead?.Invoke();
            } else {
                onHurt?.Invoke();
            }
        }
    }
}