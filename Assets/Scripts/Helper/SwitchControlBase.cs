using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControlBase : MonoBehaviour, ISwitchAble {
    protected Animator animator;
    protected SwitchStatus status = SwitchStatus.CLOSE;
    protected string animName = null;
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        InitAnim();
    }

    protected virtual void InitAnim() { }

    public void ControlDoor(SwitchStatus switchStatus) {
        status = switchStatus;
        animator.SetFloat("speed", status == SwitchStatus.OPEN ? 1 : -1);
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (animName != null) {
            if (info.IsName(animName)) {
                animator.Play(animName, 0, info.normalizedTime);
            } else {
                animator.Play(animName, 0, 0);
            }
        }
    }

    /// <summary>
    /// 正向播放到底时暂停
    /// </summary>
    public void ForwardStopPlay() {
        if (status == SwitchStatus.OPEN) {
            animator.SetFloat("speed", 0);
        }
    }

    /// <summary>
    /// 反向播放到底时暂停
    /// </summary>
    public void BackwardStopPlay() {
        if (status == SwitchStatus.CLOSE) {
            animator.SetFloat("speed", 0);
        }
    }
}