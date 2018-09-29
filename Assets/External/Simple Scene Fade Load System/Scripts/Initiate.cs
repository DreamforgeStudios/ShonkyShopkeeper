using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class Initiate
{
    public static bool IsLoading = false;
    private static Fader fadeObj = null;

    public delegate void OnFinishFading();
    public static event OnFinishFading onFinishFading;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(string scene, Color col, float multiplier)
    {
        // Can't load multiple scenes at once.
        if (IsLoading) {
            Debug.LogWarning("Already loading a scene, please wait until that's done before loading another.");
            return;
        }
        
        IsLoading = true;
        if (fadeObj == null) {
            // Create a base game object which can be used to fade.
            GameObject init = new GameObject();
            init.name = "Fader";
            Canvas myCanvas = init.AddComponent<Canvas>();
            myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            myCanvas.sortingOrder = 1;
            init.AddComponent<Fader>();
            init.AddComponent<CanvasGroup>();
            init.AddComponent<Image>();

            Fader scr = init.GetComponent<Fader>();
            scr.NextScene = scene;
            scr.FadeColor = col;
            scr.InitiateFader();
            fadeObj = scr;
        } else {
            fadeObj.NextScene = scene;
            fadeObj.InitiateFader();
        }
    }

    public static void DoneFading() {
        IsLoading = false;
        if (onFinishFading != null) {
            onFinishFading();
        }
    }
}
