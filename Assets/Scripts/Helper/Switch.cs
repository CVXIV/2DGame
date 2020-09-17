using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwitchStatus {
    CLOSE = 0,
    OPEN = 1
}

public class Switch : MonoBehaviour {
    public Door door;
    public Sprite[] sprites;
    private SpriteRenderer render;
    private SwitchStatus status = SwitchStatus.CLOSE;
    private GameObject point_light;

    private void Awake() {
        render = GetComponent<SpriteRenderer>();
        point_light = transform.Find("Light").gameObject;
        point_light.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(ConstantVar.BulletLayer)) {
            status = status == SwitchStatus.OPEN ? SwitchStatus.CLOSE : SwitchStatus.OPEN;
            render.sprite = sprites[(int)status];
            point_light.SetActive(status == SwitchStatus.OPEN);
            door.ControlDoor(status);
        }
    }

}