using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : Singleton<GamePanel> {

    private GameObject hp_prefab;
    private Transform hp_panel;
    private List<GameObject> hp;

    protected override void Awake() {
        base.Awake();
        hp_prefab = Resources.Load<GameObject>("prefab/View/hp_item");
        hp_panel = transform.Find("HP");
    }


    public void InitHP(int num) {
        hp = new List<GameObject>(num);
        for (int i = 0; i < num; ++i) {
            hp.Add(Instantiate(hp_prefab, hp_panel));
        }
    }

    public void UpdateHP(int currentHP) {
        for (int i = currentHP; i < hp.Count; ++i) {
            Toggle toggle = hp[i].GetComponent<Toggle>();
            toggle.isOn = false;
        }
    }
}