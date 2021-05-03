using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初期設定
/// </summary>
public class Boot
{
#if !UNITY_EDITOR && UNITY_ANDROID
    public static int FLAG_DITHER = 4096;
    public static int FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS = -2147483648;
    public static int FLAG_FORCE_NOT_FULLSCREEN = 2048;
    public static int FLAG_FULLSCREEN = 1024;
    public static int FLAG_HARDWARE_ACCELERATED = 16777216;
    public static int FLAG_IGNORE_CHEEK_PRESSES = 32768;
    public static int FLAG_KEEP_SCREEN_ON = 128;
    public static int FLAG_LAYOUT_ATTACHED_IN_DECOR = 1073741824;
    public static int FLAG_LAYOUT_INSET_DECOR = 65536;
    public static int FLAG_LAYOUT_IN_OVERSCAN = 33554432;
    public static int FLAG_LAYOUT_IN_SCREEN = 256;
    public static int FLAG_LAYOUT_NO_LIMITS = 512;
    public static int FLAG_LOCAL_FOCUS_MODE = 268435456;
    public static int FLAG_NOT_FOCUSABLE = 8;
    public static int FLAG_NOT_TOUCHABLE = 16;
    public static int FLAG_NOT_TOUCH_MODAL = 32;
    public static int FLAG_SCALED = 16384;
    public static int FLAG_SECURE = 8192;
    public static int FLAG_SHOW_WALLPAPER = 1048576;

    private static int clearStatusBarFlag = 0;
    private static int addStatusBarFlag = 0;

    [RuntimeInitializeOnLoadMethod()]
    static void Init()
    {
        Show();
        Screen.fullScreen = false;
        // ステータスバーとホームバーを表示する場合はセーフエリア基準にする
        UniSoftwareKeyboardArea.SoftwareKeyboardArea.IsSafeArea = true;
    }

    public static void Show()
    {
        ChangeFlags(FLAG_FULLSCREEN,FLAG_FORCE_NOT_FULLSCREEN);
    }

    public static void Hide()
    {
        ChangeFlags(FLAG_FORCE_NOT_FULLSCREEN,FLAG_FULLSCREEN);
    }

    private static void ChangeFlags(int clearFlag, int addFlag)
    {
        clearStatusBarFlag = clearFlag;
        addStatusBarFlag = addFlag;
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(RunOnUiThread));
            }
        }
    }

    private static void RunOnUiThread()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    window.Call("clearFlags", clearStatusBarFlag);
                    window.Call("addFlags", addStatusBarFlag);
                }
            }
        }
    }
#endif
}
