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
    public List<GameObject> targets;
    public Sprite[] sprites;
    protected SpriteRenderer render;
    protected SwitchStatus status = SwitchStatus.CLOSE;
    protected List<ISwitchAble> controlTargets;


    protected virtual void Awake() {
        render = GetComponent<SpriteRenderer>();
        InitControlTargets();
    }

    protected virtual void InitControlTargets() {
        if (targets != null) {
            controlTargets = new List<ISwitchAble>();
            foreach (GameObject target in targets) {
                ISwitchAble switchAble = target.CompareTag(ConstantVar.DoorTag) ? target.transform.Find("sprite").GetComponent<ISwitchAble>() : target.GetComponent<ISwitchAble>();
                if (switchAble != null) {
                    controlTargets.Add(switchAble);
                }
            }
        }
    }

    public virtual void React() {
        status = status == SwitchStatus.OPEN ? SwitchStatus.CLOSE : SwitchStatus.OPEN;
        render.sprite = sprites[(int)status];
        OnReact();
        if (controlTargets == null || controlTargets.Count == 0) {
            return;
        }
        foreach (ISwitchAble switchAble in controlTargets) {
            switchAble.ControlDoor(status);
        }
    }

    /// <summary>
    /// 在Switch被调用时触发
    /// </summary>
    public virtual void OnReact() {

    }

}