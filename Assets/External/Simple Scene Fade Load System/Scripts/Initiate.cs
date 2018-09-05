using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class Initiate
{
    public static bool IsFading = false;
    private static Fader fadeObj = null;

    public delegate void OnFinishFading();
    public static event OnFinishFading onFinishFading;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(string scene, Color col, float multiplier)
    {
        if (IsFading) {
            Debug.Log("Already Fading");
            return;
        } else {
            IsFading = true;
            if (fadeObj == null) {
                GameObject init = new GameObject();
                init.name = "Fader";
                Canvas myCanvas = init.AddComponent<Canvas>();
                myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                myCanvas.sortingOrder = 1;
                init.AddComponent<Fader>();
                init.AddComponent<CanvasGroup>();
                init.AddComponent<Image>();

                Fader scr = init.GetComponent<Fader>();
                scr.fadeDamp = multiplier;
                scr.fadeScene = scene;
                scr.fadeColor = col;
                scr.start = true;
                //areWeFading = true;
                scr.InitiateFader();
                fadeObj = scr;
            } else {
                fadeObj.fadeScene = scene;
                fadeObj.InitiateFader();
            }
        }
    }

    public static void DoneFading() {
        IsFading = false;
        if (onFinishFading != null) {
            onFinishFading();
        }
    }
}
