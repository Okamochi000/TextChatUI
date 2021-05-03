using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����o��
/// </summary>
public class SpeechBundle : MonoBehaviour
{
    [SerializeField] private Text text = null;

    /// <summary>
    /// �e�L�X�g��ݒ肷��
    /// </summary>
    /// <param name="message"></param>
    public void SetText(string message)
    {
        text.text = message;
    }
}
