using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOrder : MonoBehaviour {
    public Material material;
    public int order;

    private void Awake() {
        material.renderQueue = order;
    }
}