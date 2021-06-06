using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �m�b�W���C�A�E�g
/// </summary>
[ExecuteInEditMode]
public class NodgeLayout : UIBehaviour
{
    [System.Serializable]
    private enum LayoutType
    {
        Header,     // �w�b�_�[
        Footer,     // �t�b�^�[
    }

    [SerializeField] private LayoutType type = LayoutType.Header;
    [SerializeField] private RectTransform nodge = null;

    /// <summary>
    /// �m�b�W���X�V����
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

    protected override void Awake()
    {
        UpdateNodge();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        UpdateNodge();
    }

    /// <summary>
    /// �e�L�����o�X���擾����
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
