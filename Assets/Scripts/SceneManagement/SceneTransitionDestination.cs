using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CVXIV {
    public class SceneTransitionDestination : MonoBehaviour {
        public string destinationName;
        public UnityEvent OnReachDestination;
    }
}

