using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : EnemyBase {
    #region 变量
    private GameObject bullet;
    #endregion

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

    public void MakeBullet() {
        if (bullet == null) {
            bullet = Resources.Load<GameObject>("prefab/FlyingProb/TraceBullet");
        }
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = m_Collider.bounds.center + transform.right * m_Collider.bounds.extents.x;
        newBullet.GetComponent<TraceBullet>().LockTarget(curTarget, isFlip, damage.resetPos);
    }
}