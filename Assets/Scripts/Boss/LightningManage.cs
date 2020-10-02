using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningManage : MonoBehaviour {
    public Lightning lightningPrefab;

    public void InitLines(List<LineInfo> infos) {
        if (infos != null) {
            foreach (LineInfo lineInfo in infos) {
                Lightning cell = Instantiate(lightningPrefab, transform);
                cell.SetLinePoint(lineInfo);
            }
        }
    }

}