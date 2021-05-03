using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �e�L�X�g�`���b�g�E�B���h�E
/// </summary>
public class TextChatWindow : MonoBehaviour
{
    /// <summary>
    /// �R�����g�̎��
    /// </summary>
    public enum CommentType
    {
        Mine,       // ���g
        Opponent    // ����
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

        // �{�^���R�[���o�b�N
        sendButton.onClick.AddListener(OnSendMessage);
        closeKeyboardButton.onClick.AddListener(OnCloseKeyboard);
    }

    void Update()
    {
        // �{�^���̗L����ԕύX
        if (inputField.GetTextViewTextCount(true) == 0) { sendButton.interactable = false; }
        else { sendButton.interactable = true; }

        // �L�[�{�[�h�����{�^���̕\���ؑ�
        closeKeyboardButton.gameObject.SetActive(inputField.IsShowKeyboard());

        // ���͗��̊g�k
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

        // ���o�C���L�[�{�[�h�̍����ɍ��킹��(��Ή��̏ꍇ�͈ʒu�Œ�)
        if (inputField.IsNative())
        {
            // ���͗��̍��������킹��
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
            // �w�i�̍��������킹��
            Vector2 offsetMax = backgroundRectTransform.offsetMax;
            offsetMax.y = selfRectTransform_.offsetMin.y;
            backgroundRectTransform.offsetMax = offsetMax;
        }

        // �X�N���[���r���[�̍�������
        {
            Vector2 offsetMin = scrollRectTransform_.offsetMin;
            offsetMin.y = inputFieldRectTransform_.rect.size.y;
            scrollRectTransform_.offsetMin = offsetMin;
        }
    }

    /// <summary>
    /// �R�����g��ǉ�
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
    /// ���b�Z�[�W�𑗐M����
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
    /// �L�[�{�[�h�����
    /// </summary>
    public void OnCloseKeyboard()
    {
        inputField.ResignFirstResponderTextView();
    }
}
