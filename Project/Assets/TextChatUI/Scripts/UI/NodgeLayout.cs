using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ノッジレイアウト
/// </summary>
[ExecuteInEditMode]
public class NodgeLayout : MonoBehaviour
{
    [System.Serializable]
    private enum LayoutType
    {
        Header,     // ヘッダー
        Footer,     // フッター
    }

    [SerializeField] private LayoutType type = LayoutType.Header;
    [SerializeField] private RectTransform nodge = null;
    [SerializeField] private bool isPreview = false;

    private Vector2 screenSize_ = new Vector2();

    void Awake()
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
    public void UpdateNodge()
    {
        if (nodge != null)
        {
            float scale = 1.0f;
            CanvasScaler scaler = GetParentCanvasScaler(this.transform);
            var resolition = Screen.currentResolution;
            if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) { scale = scaler.referenceResolution.y / resolition.height; }
            Vector2 sizeDelta = nodge.sizeDelta;
            if (type == LayoutType.Header) { sizeDelta.y = (resolition.height - Screen.safeArea.yMax) * scale; }
            else if (type == LayoutType.Footer) { sizeDelta.y = Screen.safeArea.yMin * scale; }
            nodge.sizeDelta = sizeDelta;
            VerticalLayoutGroup layoutGroup = this.GetComponent<VerticalLayoutGroup>();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            this.GetComponent<ContentSizeFitter>().SetLayoutVertical();
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
