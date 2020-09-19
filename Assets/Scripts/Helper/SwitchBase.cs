using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SwitchStatus {
    CLOSE = 0,
    OPEN = 1
}

public interface ISwitchAble {
    void ControlDoor(SwitchStatus switchStatus);
}

public class SwitchBase : MonoBehaviour {
    public GameObject target;
    public Sprite[] sprites;
    protected SpriteRenderer render;
    protected SwitchStatus status = SwitchStatus.CLOSE;


    protected virtual void Awake() {
        render = GetComponent<SpriteRenderer>();
    }

    public virtual void React() {
        status = status == SwitchStatus.OPEN ? SwitchStatus.CLOSE : SwitchStatus.OPEN;
        render.sprite = sprites[(int)status];
        OnReact();
        if (target == null) {
            return;
        }
        ISwitchAble switchAble = target.GetComponent<ISwitchAble>();
        if (switchAble != null) {
            switchAble.ControlDoor(status);
        }
    }

    /// <summary>
    /// 在Switch被调用时触发
    /// </summary>
    public virtual void OnReact() {

    }

}