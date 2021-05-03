using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ノッジレイアウト
/// </summary>
[ExecuteInEditMode]
public class BodyLayout : MonoBehaviour
{
    [SerializeField] private RectTransform header = null;
    [SerializeField] private RectTransform footer = null;
    [SerializeField] private bool isHeaderNodgeOnly = false;
    [SerializeField] private bool isFooterNodgeOnly = false;
    [SerializeField] private bool isPreview = false;

    private RectTransform selfRectTransform_ = null;
    private Vector2 screenSize_ = new Vector2();

    void Start()
    {
        screenSize_ = Vector2.zero;
        UpdateNodge();
        if (!Application.isEditor) { this.enabled = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPreview && !Application.isPlaying) { return; }

        if (screenSize_.x != Screen.currentResolution.width || screenSize_.y != Screen.currentResolution.height)
        {
            screenSize_.x = Screen.currentResolution.width;
            screenSize_.y = Screen.currentResolution.height;
            UpdateNodge();
        }
    }

    /// <summary>
    /// ノッジを更新する
    /// </summary>
    private void UpdateNodge()
    {
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }
        var resolition = Screen.currentResolution;
        var area = Screen.safeArea;
        selfRectTransform_.anchorMin = Vector2.zero;
        selfRectTransform_.anchorMax = Vector2.one;
        selfRectTransform_.offsetMin = Vector2.zero;
        selfRectTransform_.offsetMax = Vector2.zero;

        // スケーリング
        float scale = 1.0f;
        CanvasScaler scaler = GetParentCanvasScaler(this.transform);
        if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) { scale = scaler.referenceResolution.y / resolition.height; }

        // ヘッダー設定
        if (isHeaderNodgeOnly)
        {
            selfRectTransform_.offsetMax = new Vector2(0, (area.yMax - resolition.height) * scale);
        }
        else
        {
            if (header != null)
            {
                NodgeLayout nodgeLayout = header.GetComponent<NodgeLayout>();
                if (nodgeLayout != null) { nodgeLayout.UpdateNodge(); }
                selfRectTransform_.offsetMax = new Vector2(0, -header.sizeDelta.y);
            }
        }

        // フッター設定
        if (isFooterNodgeOnly)
        {
            selfRectTransform_.offsetMin = new Vector2(0, area.yMin * scale);
        }
        else
        {
            if (footer != null)
            {
                NodgeLayout nodgeLayout = footer.GetComponent<NodgeLayout>();
                if (nodgeLayout != null) { nodgeLayout.UpdateNodge(); }
                selfRectTransform_.offsetMin = new Vector2(0, footer.sizeDelta.y);
            }
        }
    }

    /// <summary>
    /// 親キャンバスを取得する
    /// </summary>
    /// <returns></returns>
    private CanvasScaler GetParentCanvasScaler(Transform transform)
    {
        if (transform.parent == null) { return null; }

        CanvasScaler canvas = transform.parent.GetComponent<CanvasScaler>();
        if (canvas == null) { return GetParentCanvasScaler(this.transform.parent); }
        else { return canvas; }
    }
}
