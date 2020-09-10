using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data { }

public class Data1Value<T> : Data {
    public T Value1 { get; set; }
}

public class Data2Value<T1, T2> : Data {
    public T1 Value1 { get; set; }
    public T2 Value2 { get; set; }
}

public class DataManager : PureSingleton<DataManager> {
    private readonly Dictionary<string, Data> datas = new Dictionary<string, Data>();

    public Data GetData(string key) {
        return datas.TryGetValue(key, out Data result) ? result : null;
    }

    public void SaveData(string key, Data data) {
        if (datas.ContainsKey(key)) {
            datas[key] = data;
        } else {
            datas.Add(key, data);
        }
    }

}