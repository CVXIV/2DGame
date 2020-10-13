using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PureSingleton<T> where T : PureSingleton<T> {

    /// 显式的静态构造函数用来告诉C#编译器在其内容实例化之前不要标记其类型
    static PureSingleton() {
        Instance = System.Activator.CreateInstance<T>();
    }

    public static T Instance { get; private set; }
}