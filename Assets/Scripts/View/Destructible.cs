using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
    private BeDamage beDamage;
    private GameObject beforeDestruct;
    private GameObject afterDestruct;
    private void Awake() {
        beforeDestruct = transform.Find("BeforeDestruct").gameObject;
        beforeDestruct.SetActive(true);

        afterDestruct = transform.Find("AfterDestruct").gameObject;
        afterDestruct.SetActive(false);

        beDamage = beforeDestruct.GetComponent<BeDamage>();
        beDamage.onDead += OnDead;
    }

    private void OnDead(string resetPos) {
        Destroy(beforeDestruct);
        afterDestruct.SetActive(true);
    }

}