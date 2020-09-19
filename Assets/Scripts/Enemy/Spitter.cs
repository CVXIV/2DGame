using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : EnemyBase {

    protected override void InitNumParm() {
        idel_time = 1.5f;
        WalkSpeed = 1f;
        RunSpeed = 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            damage.Attack(collision.gameObject);
        }
    }

    protected override void PlayAnimation() {
        animator.SetBool("is_run", status == EnemyStatus.RUN);
        animator.SetBool("is_walk", status == EnemyStatus.WALK);
        animator.SetBool("is_attack", status == EnemyStatus.ATTACK);
    }
}