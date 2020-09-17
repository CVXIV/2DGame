using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Animator animator;
    private SwitchStatus status = SwitchStatus.CLOSE;
    private readonly string animName = "open_door";
    private void Awake() {
        animator = GetComponent<Animator>();
    }


    public void ControlDoor(SwitchStatus switchStatus) {
        status = switchStatus;
        animator.SetFloat("speed", status == SwitchStatus.OPEN ? 1 : -1);
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName(animName)) {
            animator.Play(animName, 0, info.normalizedTime);
        } else {
            animator.Play(animName, 0, 0);
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