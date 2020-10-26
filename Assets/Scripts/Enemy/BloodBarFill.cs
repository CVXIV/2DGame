using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodBarFill : MonoBehaviour {
    public Image image;
    private Slider slider;
    private void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value => {
            if (value <= 0) {
                image.color = Color.clear;
            }
        });
    }
}