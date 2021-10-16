using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スクロールバーのフェード制御
/// </summary>
public class ScrollBarFader : MonoBehaviour
{
    private enum BarState
    {
        Hide,
        View,
        FadeIn,
        FadeOut,
    }

    [SerializeField] [Min(0.001f)] private float viewTime = 2.0f;
    [SerializeField] [Min(0)] private float fadeInTime = 0.2f;
    [SerializeField] [Min(0)] private float fadeOutTime = 0.5f;
    [SerializeField] [Min(0.001f)] private float scrollValue = 0.01f;

    private ScrollRect scrollRect_ = null;
    private CanvasGroup verticalCanvasGroup_ = null;
    private CanvasGroup horizontalCanvasGroup_ = null;
    private float verticalLimitTime_ = 0.0f;
    private float horizontalLimitTime_ = 0.0f;
    private float verticalFadeTime_ = 0.0f;
    private float horizontalFadeTime_ = 0.0f;
    private float prevVerticalNormPos_ = 0.0f;
    private float prevHorizontalNormPos_ = 0.0f;
    private BarState verticalState_ = BarState.Hide;
    private BarState horizontalState_ = BarState.Hide;

    // Start is called before the first frame update
    void Start()
    {
        // ScrollRect取得
        scrollRect_ = this.GetComponent<ScrollRect>();
        // 縦スクロール取得
        if (scrollRect_.vertical && scrollRect_.verticalScrollbar != null)
        {
            verticalCanvasGroup_ = scrollRect_.GetComponent<CanvasGroup>();
            if (verticalCanvasGroup_ == null) { verticalCanvasGroup_ = scrollRect_.verticalScrollbar.gameObject.AddComponent<CanvasGroup>(); }
            verticalCanvasGroup_.alpha = 0.0f;
        }
        // 横スクロール取得
        if (scrollRect_.horizontal && scrollRect_.horizontalScrollbar != null)
        {
            horizontalCanvasGroup_ = scrollRect_.GetComponent<CanvasGroup>();
            if (horizontalCanvasGroup_ == null) { horizontalCanvasGroup_ = scrollRect_.horizontalScrollbar.gameObject.AddComponent<CanvasGroup>(); }
            horizontalCanvasGroup_.alpha = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 縦スクロールバーのフェード
        if (verticalCanvasGroup_ != null)
        {
            if (verticalState_ == BarState.Hide)
            {
                // 非表示中
                float diff = scrollRect_.verticalNormalizedPosition - prevVerticalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    if (fadeInTime != 0.0f)
                    {
                        // フェードイン
                        verticalState_ = BarState.FadeIn;
                        verticalFadeTime_ = fadeInTime * verticalCanvasGroup_.alpha;
                        verticalCanvasGroup_.alpha = verticalFadeTime_ / fadeInTime;
                    }
                    else
                    {
                        // 表示
                        verticalState_ = BarState.View;
                        verticalCanvasGroup_.alpha = 1.0f;
                        verticalLimitTime_ = viewTime;
                    }
                }
            }
            else if (verticalState_ == BarState.View)
            {
                // 表示中
                float diff = scrollRect_.verticalNormalizedPosition - prevVerticalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    verticalLimitTime_ = viewTime;
                }
                else
                {
                    verticalLimitTime_ -= Time.deltaTime;
                    if (verticalLimitTime_ <= 0.0f)
                    {
                        // フェードアウト
                        verticalLimitTime_ = 0.0f;
                        verticalState_ = BarState.FadeOut;
                        verticalFadeTime_ = fadeOutTime * (1.0f - verticalCanvasGroup_.alpha);
                        verticalCanvasGroup_.alpha = 1.0f - verticalFadeTime_ / fadeOutTime;
                    }
                }
            }
            else if (verticalState_ == BarState.FadeIn)
            {
                // フェードイン中
                verticalFadeTime_ += Time.deltaTime;
                verticalCanvasGroup_.alpha = Mathf.Min((verticalFadeTime_ / fadeInTime), 1.0f);
                if (verticalCanvasGroup_.alpha == 1.0f)
                {
                    // 表示
                    verticalState_ = BarState.View;
                    verticalFadeTime_ = 0.0f;
                    verticalLimitTime_ = viewTime;
                }
            }
            else
            {
                // フェードアウト中
                float diff = scrollRect_.verticalNormalizedPosition - prevVerticalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    // フェードイン
                    verticalState_ = BarState.FadeIn;
                    verticalFadeTime_ = fadeInTime * verticalCanvasGroup_.alpha;
                    verticalCanvasGroup_.alpha = verticalFadeTime_ / fadeInTime;
                }
                else
                {
                    verticalFadeTime_ += Time.deltaTime;
                    verticalCanvasGroup_.alpha = Mathf.Max((1.0f - verticalFadeTime_ / fadeOutTime), 0.0f);
                    if (verticalCanvasGroup_.alpha == 1.0f)
                    {
                        // 非表示
                        verticalState_ = BarState.Hide;
                        verticalFadeTime_ = 0.0f;
                    }
                }
            }

            prevVerticalNormPos_ = scrollRect_.verticalNormalizedPosition;
        }

        // 横スクロールバーのフェード
        if (horizontalCanvasGroup_ != null)
        {
            if (horizontalState_ == BarState.Hide)
            {
                // 非表示中
                float diff = scrollRect_.horizontalNormalizedPosition - prevHorizontalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    if (fadeInTime != 0.0f)
                    {
                        // フェードイン
                        horizontalState_ = BarState.FadeIn;
                        horizontalFadeTime_ = fadeInTime * horizontalCanvasGroup_.alpha;
                        horizontalCanvasGroup_.alpha = horizontalFadeTime_ / fadeInTime;
                    }
                    else
                    {
                        // 表示
                        horizontalState_ = BarState.View;
                        horizontalCanvasGroup_.alpha = 1.0f;
                        horizontalLimitTime_ = viewTime;
                    }
                }
            }
            else if (horizontalState_ == BarState.View)
            {
                // 表示中
                float diff = scrollRect_.horizontalNormalizedPosition - prevHorizontalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    horizontalLimitTime_ = viewTime;
                }
                else
                {
                    horizontalLimitTime_ -= Time.deltaTime;
                    if (horizontalLimitTime_ <= 0.0f)
                    {
                        // フェードアウト
                        horizontalLimitTime_ = 0.0f;
                        horizontalState_ = BarState.FadeOut;
                        horizontalFadeTime_ = fadeOutTime * (1.0f - horizontalCanvasGroup_.alpha);
                        horizontalCanvasGroup_.alpha = 1.0f - horizontalFadeTime_ / fadeOutTime;
                    }
                }
            }
            else if (horizontalState_ == BarState.FadeIn)
            {
                // フェードイン中
                horizontalFadeTime_ += Time.deltaTime;
                horizontalCanvasGroup_.alpha = Mathf.Min((horizontalFadeTime_ / fadeInTime), 1.0f);
                if (horizontalCanvasGroup_.alpha == 1.0f)
                {
                    // 表示
                    horizontalState_ = BarState.View;
                    horizontalFadeTime_ = 0.0f;
                    horizontalLimitTime_ = viewTime;
                }
            }
            else
            {
                // フェードアウト中
                float diff = scrollRect_.horizontalNormalizedPosition - prevHorizontalNormPos_;
                if (Mathf.Abs(diff) >= scrollValue)
                {
                    // フェードイン
                    horizontalState_ = BarState.FadeIn;
                    horizontalFadeTime_ = fadeInTime * horizontalCanvasGroup_.alpha;
                    horizontalCanvasGroup_.alpha = horizontalFadeTime_ / fadeInTime;
                }
                else
                {
                    horizontalFadeTime_ += Time.deltaTime;
                    horizontalCanvasGroup_.alpha = Mathf.Max((1.0f - horizontalFadeTime_ / fadeOutTime), 0.0f);
                    if (horizontalCanvasGroup_.alpha == 1.0f)
                    {
                        // 非表示
                        horizontalState_ = BarState.Hide;
                        horizontalFadeTime_ = 0.0f;
                    }
                }
            }

            prevHorizontalNormPos_ = scrollRect_.horizontalNormalizedPosition;
        }
    }
}
