using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    public static int count = 0;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 获取钥匙，销毁钥匙
            count++;
            Destroy(gameObject);
            // 聚焦到门
            HubDoor hubDoor = GameObject.Find("HubDoor").GetComponent<HubDoor>();
            if (hubDoor == null) {
                throw new System.Exception("未找到门！");
            }
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            // 修改门的状态
            cameraFollow.FollowTarget(hubDoor.GetComponent<SpriteRenderer>().bounds.center);
            hubDoor.SetStatus((HubDoorStatus)count);
        }
    }
}