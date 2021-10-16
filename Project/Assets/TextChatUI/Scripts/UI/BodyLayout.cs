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

    private RectTransform selfRectTransform_ = null;
    private Vector2 screenSize_ = new Vector2();
    private Vector2 prevHeader_ = Vector2.zero;
    private Vector2 prevFooter_ = Vector2.zero;

    void Start()
    {
        UpdateNodge();
    }

    // Update is called once per frame
    void Update()
    {
        // 更新チェック
        if (screenSize_.x == Screen.currentResolution.width && screenSize_.y == Screen.currentResolution.height)
        {
            if (header == null || prevHeader_ == header.rect.size)
            {
                if (footer == null || prevFooter_ == footer.rect.size)
                {
                    return;
                }
            }
        }

        // 更新
        UpdateNodge();
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

        screenSize_.x = Screen.currentResolution.width;
        screenSize_.y = Screen.currentResolution.height;
        if (header == null) { prevHeader_ = Vector2.zero; }
        else { prevHeader_ = header.rect.size; }
        if (footer == null) { prevFooter_ = Vector2.zero; }
        else { prevFooter_ = footer.rect.size; }
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

    /// <summary>
    /// インスペクター変更検知
    /// </summary>
    private void OnValidate()
    {
        UpdateNodge();
    }
}
