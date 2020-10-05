using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    public static int count = 0;
    public HubDoor hubDoor;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 获取钥匙，销毁钥匙
            count++;
            Destroy(gameObject);
            hubDoor.SetStatus((HubDoorStatus)count);
        }
    }
}