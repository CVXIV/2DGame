using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunnerStatus {
    IDEL,
    WALK,
    DISABLED,
    ATTACK,
    DEAD
}

public class Gunner : MonoBehaviour {
    #region 变量
    private Animator animator;
    private BeDamage gunnerHealth;
    private RecoverBeDamage shieldHealth;
    private GunnerStatus gunnerStatus = GunnerStatus.IDEL;
    private float disabledTime = 2f;
    #endregion

    private void Awake() {
        animator = GetComponent<Animator>();
        InitBedamage();
    }

    private void InitBedamage() {
        gunnerHealth = GetComponent<BeDamage>();
        shieldHealth = transform.Find("GunnerShield").GetComponent<RecoverBeDamage>();

        gunnerHealth.onHurt += GunnerOnHurt;
        gunnerHealth.onDead += GunnerOnDead;
        shieldHealth.onHurt += ShieldOnHurt;
        shieldHealth.onDead += ShieldOnDead;
    }

    #region 受伤相关
    private void GunnerOnHurt(DamageType damageType, string resetPos, int damageNum) {
        if (shieldHealth.Health > 0) {
            shieldHealth.TakeDamage(damageNum, damageType, resetPos);
            gunnerHealth.RecoverHealth(damageNum);
        }
    }

    private void GunnerOnDead(string resetPos) {

    }

    private void ShieldOnHurt(DamageType damageType, string resetPos, int damageNum) {
        // todo
    }

    private void ShieldOnDead(string resetPos) {
        shieldHealth.gameObject.SetActive(false);
        gunnerStatus = GunnerStatus.DISABLED;
        Invoke(nameof(ResetShield), disabledTime);
    }

    private void ResetShield() {
        gunnerStatus = GunnerStatus.IDEL;
        shieldHealth.gameObject.SetActive(true);
        shieldHealth.ResetHealth();
    }
    #endregion

    private void FixedUpdate() {
        UpdateStatus();
        ActionOfStatus();
    }

    private void ActionOfStatus() {
        switch (gunnerStatus) {
            case GunnerStatus.IDEL:
                break;
            case GunnerStatus.WALK:
                break;
            case GunnerStatus.DISABLED:
                break;
            case GunnerStatus.ATTACK:
                break;
            case GunnerStatus.DEAD:
                break;
            default:
                break;
        }
    }

    private void UpdateStatus() {
        animator.SetBool("is_walk", gunnerStatus == GunnerStatus.WALK);
        animator.SetBool("is_disabled", gunnerStatus == GunnerStatus.DISABLED);
        //animator.SetTrigger("attack");
        animator.SetBool("is_dead", gunnerStatus == GunnerStatus.DEAD);
    }
}