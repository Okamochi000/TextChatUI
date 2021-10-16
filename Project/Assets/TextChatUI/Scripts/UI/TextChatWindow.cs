using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// テキストチャットウィンドウ
/// </summary>
public class TextChatWindow : MonoBehaviour
{
    /// <summary>
    /// コメントの種類
    /// </summary>
    public enum CommentType
    {
        Mine,       // 自身
        Opponent    // 相手
    }

    [SerializeField] private GameObject myComment = null;
    [SerializeField] private GameObject opponentComment = null;
    [SerializeField] private NativeTextField inputField = null;
    [SerializeField] private Button sendButton = null;
    [SerializeField] private Button closeKeyboardButton = null;
    [SerializeField] private RectTransform backgroundRectTransform = null;
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private float maxFieldSize = 300.0f;

    private RectTransform selfRectTransform_ = null;
    private RectTransform inputFieldRectTransform_ = null;
    private RectTransform scrollRectTransform_ = null;
    private float minFieldSize_ = 0.0f;

    void Start()
    {
        selfRectTransform_ = this.GetComponent<RectTransform>();
        inputFieldRectTransform_ = inputField.GetComponent<RectTransform>();
        scrollRectTransform_ = scrollRect.GetComponent<RectTransform>();
        minFieldSize_ = inputFieldRectTransform_.rect.height;

        // ボタンコールバック
        sendButton.onClick.AddListener(OnSendMessage);
        closeKeyboardButton.onClick.AddListener(OnCloseKeyboard);
    }

    void Update()
    {
        // ボタンの有効状態変更
        if (inputField.GetTextViewTextCount(true) == 0) { sendButton.interactable = false; }
        else { sendButton.interactable = true; }

        // キーボードを閉じるボタンの表示切替
        closeKeyboardButton.gameObject.SetActive(inputField.IsShowKeyboard());

        // 入力欄の拡縮
        float textViewHeight = inputField.GetTextViewHeight();
        if (!inputField.IsNative())
        {
            if (inputField.DefaultInputField.textComponent.cachedTextGeneratorForLayout.lineCount < 2) { textViewHeight = minFieldSize_; }
            else { textViewHeight = minFieldSize_ - inputField.DefaultInputField.textComponent.fontSize + textViewHeight; }
        }
        Vector2 sizeDelta = inputFieldRectTransform_.sizeDelta;
        sizeDelta.y = textViewHeight;
        sizeDelta.y = Mathf.Min(sizeDelta.y, maxFieldSize);
        sizeDelta.y = Mathf.Max(sizeDelta.y, minFieldSize_);
        inputFieldRectTransform_.sizeDelta = sizeDelta;

        // モバイルキーボードの高さに合わせる(非対応の場合は位置固定)
        if (inputField.IsNative())
        {
            // 入力欄の高さを合わせる
            Vector2 offsetMin = selfRectTransform_.offsetMin;
            if (inputField.IsShowKeyboard()) { offsetMin.y = inputField.GetKeyboardHeight() / this.transform.lossyScale.y; }
            else { offsetMin.y = 0.0f; }
            if (offsetMin.y < 0.0f) { offsetMin.y = 0.0f; }
            if (selfRectTransform_.offsetMin != offsetMin)
            {
                scrollRect.enabled = false;
                selfRectTransform_.offsetMin = offsetMin;
                scrollRect.enabled = true;
            }
            // 背景の高さを合わせる
            Vector2 offsetMax = backgroundRectTransform.offsetMax;
            offsetMax.y = selfRectTransform_.offsetMin.y;
            backgroundRectTransform.offsetMax = offsetMax;
        }

        // スクロールビューの高さ調整
        {
            Vector2 offsetMin = scrollRectTransform_.offsetMin;
            offsetMin.y = inputFieldRectTransform_.rect.size.y;
            scrollRectTransform_.offsetMin = offsetMin;
        }
    }

    /// <summary>
    /// コメントを追加
    /// </summary>
    /// <param name="commentType"></param>
    /// <param name="message"></param>
    public void AddComment(CommentType commentType, string message)
    {
        GameObject baseObj = null;
        switch (commentType)
        {
            case CommentType.Mine: baseObj = myComment; break;
            case CommentType.Opponent: baseObj = opponentComment; break;
            default: break;
        }
        if (baseObj == null) { return; }

        GameObject copy = GameObject.Instantiate(baseObj);
        copy.transform.SetParent(baseObj.transform.parent, false);
        copy.SetActive(true);
        copy.GetComponent<SpeechBundle>().SetText(message);
    }

    /// <summary>
    /// メッセージを送信する
    /// </summary>
    public void OnSendMessage()
    {
        string message = inputField.UpdateTextViewText();
        if (message == "") { return; }

        AddComment(CommentType.Mine, message);
        inputField.ClearTextViewText();
        inputField.ResignFirstResponderTextView();
    }

    /// <summary>
    /// キーボードを閉じる
    /// </summary>
    public void OnCloseKeyboard()
    {
        inputField.ResignFirstResponderTextView();
    }
}
