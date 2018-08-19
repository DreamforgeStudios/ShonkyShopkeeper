using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public static class Initiate
{
    static bool areWeFading = false;
    private static Fader fadeObj = null;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(string scene, Color col, float multiplier)
    {
        if (areWeFading) {
            Debug.Log("Already Fading");
            return;
        } else {
            areWeFading = true;
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
        areWeFading = false;
    }
}
