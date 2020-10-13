using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yields : PureSingleton<Yields> {

    private Yields() { }

    private static readonly Dictionary<float, WaitForSeconds> waitMap = new Dictionary<float, WaitForSeconds>();


    public static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate _FixedUpdate = new WaitForFixedUpdate();
    public static WaitForSeconds GetWaitForSeconds(float delay) {
        waitMap.TryGetValue(delay, out WaitForSeconds result);
        if (result == null) {
            result = new WaitForSeconds(delay);
            waitMap.Add(delay, result);
        }
        return result;
    }
}