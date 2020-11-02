using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spikes : MonoBehaviour {
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private Damage damage;
    private Dictionary<GameObject, float> object_to_damage;
    private readonly float damageGap = 1.0f;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        damage = GetComponent<Damage>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        object_to_damage = new Dictionary<GameObject, float>();
    }

    private void Update() {
        UpdateObjTime();
    }

    private void UpdateObjTime() {
        if (object_to_damage.Count > 0) {
            foreach (GameObject obj in object_to_damage.Keys.ToArray()) {
                object_to_damage[obj] += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 对游戏物体造成伤害
        GameObject damageObj = collision.gameObject;
        if (object_to_damage.ContainsKey(damageObj)) {
            if (object_to_damage[damageObj] >= damageGap) {
                damage.Attack(damageObj);
                object_to_damage[damageObj] = 0;
            }
        } else {
            damage.Attack(damageObj);
            object_to_damage.Add(damageObj, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        object_to_damage.Remove(collision.gameObject);
    }


    private void OnTriggerStay2D(Collider2D collision) {
        this.OnTriggerEnter2D(collision);
    }

}