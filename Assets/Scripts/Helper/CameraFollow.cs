using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraFollow : MonoBehaviour {
    #region 变量
    public Transform target;
    private Vector3 speicalPosition;
    private float normalFOV;
    private float specialFOV;
    private bool isNormalFollow = true;
    private Transform cameraPos;
    private const float maxXoffset = 4;
    private const float maxYoffset = 3;
    private Rigidbody2D rigid;
    private Camera thisCamera;
    private CharacterControl characterControl;
    #endregion

    private void Awake() {
        characterControl = target.GetComponent<CharacterControl>();
        thisCamera = GetComponent<Camera>();
        normalFOV = thisCamera.orthographicSize;
        specialFOV = normalFOV * 5 / 6;
        rigid = target.GetComponent<Rigidbody2D>();
        cameraPos = this.transform;
        cameraPos.position = new Vector3(target.position.x, target.position.y, cameraPos.position.z);
    }

    private void Update() {
        if (isNormalFollow) {
            // 恢复镜头大小
            if (thisCamera.orthographicSize != normalFOV) {
                thisCamera.orthographicSize = Mathf.MoveTowards(thisCamera.orthographicSize, normalFOV, Time.deltaTime);
            }
            if (Mathf.Abs(cameraPos.position.x - target.position.x) > maxXoffset || Mathf.Abs(cameraPos.position.y - target.position.y) > maxYoffset) {
                Vector3 goal = new Vector3(target.position.x, target.position.y, cameraPos.position.z);
                cameraPos.position = Vector3.MoveTowards(cameraPos.position, goal, Vector3.Magnitude(Vector2.SqrMagnitude(rigid.velocity) > 0 ? rigid.velocity : new Vector2(characterControl.speedX, characterControl.speedY)) * Time.deltaTime);
            }
        } else {
            cameraPos.position = Vector3.Lerp(cameraPos.position, new Vector3(speicalPosition.x, speicalPosition.y, cameraPos.position.z), 3 * Time.deltaTime);
            if (Vector2.Distance(cameraPos.position, speicalPosition) < 0.1f) {
                thisCamera.orthographicSize = Mathf.MoveTowards(thisCamera.orthographicSize, specialFOV, Time.deltaTime);
            }
        }

    }

    public void FollowTarget(Transform tar) {
        speicalPosition = tar.GetComponent<SpriteRenderer>().bounds.center;
        isNormalFollow = false;
    }

    public void ResetFollowTarget() {
        isNormalFollow = true;
    }
}