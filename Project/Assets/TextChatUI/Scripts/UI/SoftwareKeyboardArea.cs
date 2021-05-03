using UnityEngine;

namespace UniSoftwareKeyboardArea
{
    /// <summary>
    /// ソフトウェアキーボードの表示領域を管理するクラス
    /// </summary>
    public static class SoftwareKeyboardArea
    {
        public static bool IsSafeArea { get; set; } = false;
        public static NativeTextField TextFieldIOS { get; set; } = null;

        /// <summary>
        /// 高さを返します
        /// </summary>
        public static int GetHeight()
        {
#if !UNITY_EDITOR && UNITY_IOS
            if (TextFieldIOS != null)
            {
                return (int)TextFieldIOS.GetViewHeight();
            }
            else
            {
                var area = TouchScreenKeyboard.area;
                var height = Mathf.RoundToInt(area.height);
                return Screen.height <= height ? 0 : height;
            }
#elif !UNITY_EDITOR && UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                var unityPlayer = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");
                var view = unityPlayer.Call<AndroidJavaObject>("getView");
                
                if (view == null) return 0;

                int result;

                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    view.Call("getWindowVisibleDisplayFrame", rect);
                    if(IsSafeArea){ result = (int)Screen.safeArea.height - rect.Call<int>("height"); }
                    else { result = Screen.height - rect.Call<int>("height"); }
                }

                if (TouchScreenKeyboard.hideInput) return result;

                var softInputDialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
                var window = softInputDialog?.Call<AndroidJavaObject>("getWindow");
                var decorView = window?.Call<AndroidJavaObject>("getDecorView");

                if (decorView == null) return result;

                var decorHeight = decorView.Call<int>("getHeight");
                result += decorHeight;

                return result;
            }
#else
            var area = TouchScreenKeyboard.area;
            var height = Mathf.RoundToInt(area.height);
            return Screen.height <= height ? 0 : height;
#endif
        }
    }
}