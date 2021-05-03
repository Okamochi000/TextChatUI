using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// �l�C�e�B�u�̃e�L�X�g�t�B�[���h
/// </summary>
public class NativeTextField : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")] private static extern void createTextView_();
    [DllImport("__Internal")] private static extern void updateTextViewRect_(float origin_x, float origin_y, float width, float height);
    [DllImport("__Internal")] private static extern void clearTextViewText_();
    [DllImport("__Internal")] private static extern void getTextViewText_(IntPtr str_p);
    [DllImport("__Internal")] private static extern int getTextViewTextCount_(bool isSpaceOmit);
    [DllImport("__Internal")] private static extern int getTextViewTextByteCount_();
    [DllImport("__Internal")] private static extern void setTextViewFont_(float size, float color_r, float color_g, float color_b, float color_a);
    [DllImport("__Internal")] private static extern float getTextViewHeight_();
    [DllImport("__Internal")] private static extern void showTextView_();
    [DllImport("__Internal")] private static extern void hideTextView_();
    [DllImport("__Internal")] private static extern void resignFirstResponderTextView_();
    [DllImport("__Internal")] private static extern void becomeFirstResponderTextView_();
    [DllImport("__Internal")] private static extern float getViewHeight_();
    [DllImport("__Internal")] private static extern float getViewWidth_();
    [DllImport("__Internal")] private static extern float getKeyboardHeight_();
    [DllImport("__Internal")] private static extern bool getKeyboardIsShow_();
#endif

    private readonly Vector2 CenterAnchor = new Vector2(0.5f, 0.5f);

    public bool IsInitialize { get; private set; } = false;
    public RectTransform NativeTextArea { get { return nativeTextArea; } }
    public RectTransform UnityTextArea { get { return unityTextArea; } }
    public InputField DefaultInputField { get { return defaultInputField; } }

    [SerializeField] private RectTransform nativeTextArea = null;
    [SerializeField] private RectTransform unityTextArea = null;
    [SerializeField] private InputField defaultInputField = null;
    [SerializeField] private Text unityTextAreaText = null;
    [SerializeField] private Button nativeOpenButton = null;

    private float prevHeight_ = 0.0f;
    private Vector2 prevPosition_ = Vector2.zero;
    private Vector2 prevSizeDelta_ = Vector2.zero;
    private Vector2 viewSize_ = Vector2.zero;
    private Vector2 viewScale_ = Vector2.zero;
    private bool isPrevShow_ = false;

    void Start()
    {
        // �����ݒ�
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsNative() || !IsInitialize) { return; }

        // ���o�C���L�[�{�[�h���J���ꂽ�Ƃ��e�L�X�g���X�V����
        bool isKeyboardOpen = IsShowKeyboard();
        if (isKeyboardOpen != isPrevShow_)
        {
            if (isKeyboardOpen) { ShowTextView(); }
            else { HideTextView(); }
            UpdateTextViewText();
            isPrevShow_ = isKeyboardOpen;
        }

        // �s�����������Ƃ��e�L�X�g���X�V����
        if (isKeyboardOpen)
        {
            float currentHeight = GetTextViewHeight();
            if (prevHeight_ != currentHeight)
            {
                UpdateTextViewText();
                prevHeight_ = currentHeight;
            }
        }

        // �ʒu���ς�����������̓T�C�Y���ς�����Ƃ��e�L�X�g���X�V����
        Vector2 globalSize = nativeTextArea.rect.size * nativeTextArea.lossyScale * viewScale_;
        if (prevSizeDelta_ != globalSize || prevPosition_ != (Vector2)unityTextArea.position)
        {
            UpdateTextView();
            SetTextViewFont(unityTextAreaText.fontSize, unityTextAreaText.color);
            prevSizeDelta_ = globalSize;
            prevPosition_ = (Vector2)unityTextArea.position;
        }
    }

    public void OnEnable()
    {
        if (!IsInitialize) { return; }

        HideTextView();
        if (IsNative())
        {
            if (!Application.isEditor && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UniSoftwareKeyboardArea.SoftwareKeyboardArea.TextFieldIOS = this;
            }
        }
    }
    
    public void OnDisable()
    {
        if (!IsInitialize) { return; }

        HideTextView();
        if (IsNative())
        {
            if (!Application.isEditor && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UniSoftwareKeyboardArea.SoftwareKeyboardArea.TextFieldIOS = null;
            }
        }
    }

    /// <summary>
    /// �����ݒ�
    /// </summary>
    public void Initialize()
    {
        if (IsInitialize) { return; }

        // �e�L�X�g�\��
        defaultInputField.gameObject.SetActive(!IsNative());
        unityTextArea.gameObject.SetActive(IsNative());

        // �e�L�X�g�����Z�b�g����
        defaultInputField.text = "";
        unityTextAreaText.text = "";
        ClearTextViewText();

        // �{�^���R�[���o�b�N�ݒ�
        nativeOpenButton.onClick.AddListener(BecomeFirstResponderTextView);

        // �r���[�T�C�Y��ݒ肷��
        viewSize_.x = GetViewWidth();
        viewSize_.y = GetViewHeight();
        viewScale_.x = viewSize_.x / Screen.width;
        viewScale_.y = viewSize_.y / Screen.height;

        // ���͗����쐬����
        CreateTextView();

        // IOS�l�C�e�B�u�L�[�{�[�h�g�p�ݒ�
        if (IsNative())
        {
            if (!Application.isEditor && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UniSoftwareKeyboardArea.SoftwareKeyboardArea.TextFieldIOS = this;
            }
        }

        // �t�H���g�ݒ�
        if (IsNative()) { SetTextViewFont(unityTextAreaText.fontSize, unityTextAreaText.color); }
        else { SetTextViewFont(defaultInputField.textComponent.fontSize, defaultInputField.textComponent.color); }

        // ���͗����B��
        HideTextView();

        // �����t���O�ؑ�
        IsInitialize = true;
    }

    /// <summary>
    /// �l�C�e�B�u���g�p���邩
    /// </summary>
    /// <returns></returns>
    public bool IsNative()
    {
        if (!Application.isEditor)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer) { return true; }
            else if (Application.platform == RuntimePlatform.Android) { return false; }
            return false;
        }

        return false;
    }
    
    #region Native

    /// <summary>
    /// �e�L�X�g�r���[�쐬
    /// </summary>
    public void CreateTextView()
    {
        if (!IsNative()) { return; }

#if !UNITY_EDITOR && UNITY_IOS
        createTextView_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �e�L�X�g�r���[�X�V
    /// </summary>
    public void UpdateTextView()
    {
        if (!IsNative()) { return; }

        Vector2 nativePos = unityTextArea.position;
        Vector2 globalSize = nativeTextArea.rect.size * nativeTextArea.lossyScale * viewScale_;
        nativePos.x = (nativePos.x * viewScale_.x) - globalSize.x / 2.0f;
        nativePos.y = (viewSize_.y - nativePos.y * viewScale_.y) - globalSize.y / 2.0f;

#if !UNITY_EDITOR && UNITY_IOS
        updateTextViewRect_(nativePos.x, nativePos.y, globalSize.x, globalSize.y);
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �e�L�X�g�폜
    /// </summary>
    public void ClearTextViewText()
    {
        // Unity��̃e�L�X�g�폜
        defaultInputField.text = "";
        unityTextAreaText.text = "";

        if (!IsNative()) { return; }

#if !UNITY_EDITOR && UNITY_IOS
        clearTextViewText_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �e�L�X�g���X�V����
    /// </summary>
    /// <returns></returns>
    public string UpdateTextViewText()
    {
        if (!IsNative()) { return defaultInputField.text; }

#if !UNITY_EDITOR && UNITY_IOS
        int byteCount = GetTextViewTextByteCount();
        if (byteCount == 0)
        {
            defaultInputField.text = "";
            unityTextAreaText.text = "";
            return "";
        }

        IntPtr intPtr = IntPtr.Zero;
        try
        {
            intPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(byteCount);
            byte[] bytes = new byte[byteCount];
            Marshal.Copy(bytes, 0, intPtr, byteCount);
            getTextViewText_(intPtr);
            Marshal.Copy(intPtr, bytes, 0, byteCount);
            string str = System.Text.Encoding.UTF8.GetString(bytes);
            defaultInputField.text = str;
            unityTextAreaText.text = str;
        }
        catch { }
        finally
        {
            if (intPtr != IntPtr.Zero) { System.Runtime.InteropServices.Marshal.FreeCoTaskMem(intPtr); }
        }

        return unityTextAreaText.text;
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif

        return unityTextAreaText.text;
    }

    /// <summary>
    /// �e�L�X�g�̕��������擾����
    /// </summary>
    /// <param name="isSpaceOmit">�󔒂݂̂̏�Ԃ��J�E���g�Ɋ܂߂邩</param>
    /// <returns></returns>
    public int GetTextViewTextCount(bool isSpaceOmit)
    {
        if (!IsNative())
        {
            if (defaultInputField.text.Length == 0) { return 0;  }
            if (isSpaceOmit)
            {
                string replaceText = defaultInputField.text;
                replaceText = replaceText.Replace("\n", "");
                replaceText = replaceText.Replace(" ", "");
                replaceText = replaceText.Replace("�@", "");
                if (replaceText.Length != 0) { return defaultInputField.text.Length; }
                else { return 0; }
            }
            else
            {
                return defaultInputField.text.Length;
            }
        }

#if !UNITY_EDITOR && UNITY_IOS
        return getTextViewTextCount_(isSpaceOmit);
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif

        if (isSpaceOmit)
        {
            if (unityTextAreaText.text.Length == 0) { return 0; }
            string replaceText = unityTextAreaText.text;
            replaceText = replaceText.Replace("\n", "");
            replaceText = replaceText.Replace(" ", "");
            replaceText = replaceText.Replace("�@", "");
            if (replaceText.Length != 0) { return unityTextAreaText.text.Length; }
            else { return 0; }
        }

        return unityTextAreaText.text.Length;
    }

    /// <summary>
    /// �e�L�X�g�̃o�C�g�����擾����
    /// </summary>
    /// <returns></returns>
    public int GetTextViewTextByteCount()
    {
        if (!IsNative())
        {
            if (defaultInputField.text.Length > 0) { return System.Text.Encoding.UTF8.GetBytes(defaultInputField.text).Length; }
            else { return 0; }
        }

#if !UNITY_EDITOR && UNITY_IOS
        return getTextViewTextByteCount_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif

        if (unityTextAreaText.text.Length > 0) { return System.Text.Encoding.UTF8.GetBytes(unityTextAreaText.text).Length; }
        else { return 0; }
    }

    /// <summary>
    /// �������擾����
    /// </summary>
    /// <returns></returns>
    public float GetTextViewHeight()
    {
        if (!IsNative()) { return defaultInputField.preferredHeight; }

#if !UNITY_EDITOR && UNITY_IOS
        float viewHeight = getTextViewHeight_();
        viewHeight = (viewHeight / nativeTextArea.lossyScale.y / viewScale_.y);
        return viewHeight;
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif

        return unityTextAreaText.preferredHeight;
    }

    /// <summary>
    /// �t�H���g�ݒ�
    /// </summary>
    /// <param name="size"></param>
    /// <param name="color"></param>
    public void SetTextViewFont(int size, Color color)
    {
        // Unity��̃e�L�X�g�t�H���g�ݒ�
        color.a = defaultInputField.textComponent.color.a;
        defaultInputField.textComponent.color = color;
        defaultInputField.textComponent.fontSize = size;
        color.a = unityTextAreaText.color.a;
        unityTextAreaText.color = color;
        unityTextAreaText.fontSize = size;

        // �l�C�e�B�u�̃e�L�X�g�t�H���g�ݒ�
        if (!IsNative()) { return; }
        float nativeFontSize = (float)size;
        if (viewScale_ != Vector2.zero)
        {
            Vector2 globalSize = nativeTextArea.rect.size * nativeTextArea.lossyScale * viewScale_;
            float scale = globalSize.y / unityTextArea.rect.size.y;
            nativeFontSize *= scale;
            nativeFontSize = (float)Math.Round(nativeFontSize, 1, MidpointRounding.AwayFromZero);
        }

#if !UNITY_EDITOR && UNITY_IOS
        setTextViewFont_(nativeFontSize, (color.r / 255.0f), (color.g / 255.0f), (color.b / 255.0f), 1.0f);
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �l�C�e�B�u�e�L�X�g���J��
    /// </summary>
    public void ShowTextView()
    {
        if (!IsNative()) { return; }

        // �����F�ݒ�
        Color color = unityTextAreaText.color;
        color.a = 0.0f;
        unityTextAreaText.color = color;

        // �J���{�^���\��
        nativeOpenButton.gameObject.SetActive(false);

#if !UNITY_EDITOR && UNITY_IOS
        showTextView_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �l�C�e�B�u�e�L�X�g���B��
    /// </summary>
    public void HideTextView()
    {
        if (!IsNative()) { return; }

        // �����F�ݒ�
        Color color = unityTextAreaText.color;
        color.a = 1.0f;
        unityTextAreaText.color = color;

        // �J���{�^���\��
        nativeOpenButton.gameObject.SetActive(true);

#if !UNITY_EDITOR && UNITY_IOS
        hideTextView_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// ���o�C���L�[�{�[�h���J��
    /// </summary>
    public void BecomeFirstResponderTextView()
    {
        if (!IsNative()) { return; }

        // ���Ă���ꍇ�͊J��
        ShowTextView();

#if !UNITY_EDITOR && UNITY_IOS
        becomeFirstResponderTextView_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// ���o�C���L�[�{�[�h�����
    /// </summary>
    public void ResignFirstResponderTextView()
    {
        if (!IsNative())
        {
            if (TouchScreenKeyboard.visible) { EventSystem.current.SetSelectedGameObject(null); }
            return;
        }

        // �J���Ă���ꍇ�͕���
        HideTextView();

#if !UNITY_EDITOR && UNITY_IOS
        resignFirstResponderTextView_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
    }

    /// <summary>
    /// �r���[�̍������擾����
    /// </summary>
    public float GetViewHeight()
    {
        if (!IsNative()) { return Screen.height; }

#if !UNITY_EDITOR && UNITY_IOS
        return getViewHeight_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
        return 0.0f;
    }

    /// <summary>
    /// �r���[�̉������擾����
    /// </summary>
    public float GetViewWidth()
    {
        if (!IsNative()) { return Screen.width; }

#if !UNITY_EDITOR && UNITY_IOS
        return getViewWidth_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
        return 0.0f;
    }

    /// <summary>
    /// �L�[�{�[�h�̍������擾����
    /// </summary>
    /// <returns></returns>
    public float GetKeyboardHeight()
    {
        if (!IsNative()) { UniSoftwareKeyboardArea.SoftwareKeyboardArea.GetHeight(); }

#if !UNITY_EDITOR && UNITY_IOS
        if (viewScale_ == Vector2.zero) { return 0.0f; }
        else { return (getKeyboardHeight_() / viewScale_.y); }
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
        return UniSoftwareKeyboardArea.SoftwareKeyboardArea.GetHeight();
    }

    /// <summary>
    /// �L�[�{�[�h�̕\����Ԃ��擾����
    /// </summary>
    /// <returns></returns>
    public bool IsShowKeyboard()
    {
        if (!IsNative()) { return TouchScreenKeyboard.visible; }

#if !UNITY_EDITOR && UNITY_IOS
        return getKeyboardIsShow_();
#elif !UNITY_EDITOR && UNITY_ANDROID
#endif
        return TouchScreenKeyboard.visible;
    }

    #endregion
}
