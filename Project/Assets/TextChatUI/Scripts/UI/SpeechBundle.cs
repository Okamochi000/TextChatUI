using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 吹き出し
/// </summary>
public class SpeechBundle : MonoBehaviour
{
    [SerializeField] private Text text = null;

    /// <summary>
    /// テキストを設定する
    /// </summary>
    /// <param name="message"></param>
    public void SetText(string message)
    {
        text.text = message;
    }
}
