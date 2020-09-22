using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraFollow : MonoBehaviour {
    #region 变量
    public Transform target;
    private Transform player;
    private Vector3 speicalPosition;
    private float normalFOV;
    private float specialFOV;
    private bool isNormalFollow = true;
    private Transform cameraPos;
    private const float maxXoffset = 4;
    private const float maxYoffset = 3;
    private Camera thisCamera;
    private CharacterControl characterControl;
    private float delayTime = 1.0f;
    #endregion

    private void Awake() {
        player = target;
        characterControl = target.GetComponent<CharacterControl>();
        thisCamera = GetComponent<Camera>();
        normalFOV = thisCamera.orthographicSize;
        specialFOV = normalFOV * 5 / 6;
        cameraPos = this.transform;
        cameraPos.position = new Vector3(target.position.x, target.position.y, cameraPos.position.z);
    }

    private void LateUpdate() {
        if (isNormalFollow) {
            // 恢复镜头大小
            if (thisCamera.orthographicSize != normalFOV) {
                thisCamera.orthographicSize = Mathf.MoveTowards(thisCamera.orthographicSize, normalFOV, Time.deltaTime);
            }
            if (Mathf.Abs(cameraPos.position.x - target.position.x) > maxXoffset || Mathf.Abs(cameraPos.position.y - target.position.y) > maxYoffset) {
                Vector3 goal = new Vector3(target.position.x, target.position.y, cameraPos.position.z);
                cameraPos.position = Vector3.Lerp(cameraPos.position, goal, Time.deltaTime);
            }
        } else {
            cameraPos.position = Vector3.Lerp(cameraPos.position, new Vector3(speicalPosition.x, speicalPosition.y, cameraPos.position.z), 3 * Time.deltaTime);
            if (Vector2.Distance(cameraPos.position, speicalPosition) < 0.1f) {
                thisCamera.orthographicSize = Mathf.MoveTowards(thisCamera.orthographicSize, specialFOV, Time.deltaTime);
                if (Mathf.Abs(thisCamera.orthographicSize - specialFOV) < 0.1f) {
                    Invoke(nameof(ResetFollowTarget), delayTime);
                }
            }
        }
    }

    public void FollowTarget(Vector3 pos, float delayTime = 1.0f) {
        this.delayTime = delayTime;
        speicalPosition = pos;
        isNormalFollow = false;
        // 人物不可移动
        characterControl.NotMove();
    }

    public void ResetFollowTarget() {
        target = player;
        isNormalFollow = true;
        characterControl.CanMove();
    }
}