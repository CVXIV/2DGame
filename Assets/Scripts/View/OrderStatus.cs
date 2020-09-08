using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderStatus : MonoBehaviour {
    private MeshRenderer meshRenderer;
    public int orderIndex;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = orderIndex;
    }

}