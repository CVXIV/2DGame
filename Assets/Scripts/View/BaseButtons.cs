using UnityEngine;
using UnityEngine.UI;

public class BaseButtons : MonoBehaviour {
    #region 字段
    public Sprite highLighted;

    protected Sprite normalSprite;
    protected Image bg;
    #endregion

    #region 初始化
    private void Awake() {
        Init();
    }

    public virtual void Init() {
        bg = GetComponent<Image>();
        normalSprite = bg.sprite;
    }
    #endregion

    #region 事件监听
    public virtual void OnPointerEnter() {
        bg.sprite = highLighted;
    }

    public virtual void OnPointerExit() {
        bg.sprite = normalSprite;
    }

    public virtual void OnPointerUp() {
        bg.sprite = normalSprite;
    }
    #endregion
}