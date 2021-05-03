using System;
using UnityEngine;

/// <summary>
/// シングルトン
/// </summary>
/// <typeparam name="T">シングルトンにするクラスの型</typeparam>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance_;
    public static T Instance
    {
        get
        {
            if (instance_ == null)
            {
                Type t = typeof(T);

                instance_ = (T)FindObjectOfType(t);
                if (instance_ == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance_;
        }
    }

    public static bool IsExist()
    {
        return (instance_ != null);
    }

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Type t = typeof(T);
            Debug.LogError(t + " はすでにアタッチされています");
            return;
        }
    }
}