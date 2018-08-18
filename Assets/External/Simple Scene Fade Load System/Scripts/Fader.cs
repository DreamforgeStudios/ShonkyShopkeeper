using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [HideInInspector]
    public bool start = false;
    [HideInInspector]
    public float fadeDamp = 0.0f;
    [HideInInspector]
    public string fadeScene;
    [HideInInspector]
    public float alpha = 0.0f;
    [HideInInspector]
    public Color fadeColor;
    [HideInInspector]
    public bool isFadeIn = false;
    CanvasGroup myCanvas;
    Image bg;
    
    //Set callback
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    //Remove callback
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitiateFader()
    {

        DontDestroyOnLoad(gameObject);

        //Getting the visual elements
        if (transform.GetComponent<CanvasGroup>())
            myCanvas = transform.GetComponent<CanvasGroup>();

        if (transform.GetComponentInChildren<Image>())
        {
            bg = transform.GetComponent<Image>();
            bg.color = fadeColor;
            bg.sprite = Resources.Load<Sprite>("LoadingSceneTransition");
            bg.type = Image.Type.Filled;
            bg.fillMethod = Image.FillMethod.Radial90;
        }
        //Checking and starting the coroutine
        if (myCanvas && bg)
        {
            //myCanvas.alpha = 0.0f;
            bg.fillAmount = 0f;
            WipeFadeIn();
            //StartCoroutine(FadeIt());
        }
        else
            Debug.LogWarning("Something is missing please reimport the package.");
    }

    private void WipeFadeIn() {
        //Remove two slashes when implementing sound
        //SFX.Play("sound");
        StartCoroutine(LoadAsyncScene(fadeScene));
        DOTween.To(() => bg.fillAmount, x => bg.fillAmount = x, 1f, .7f).SetUpdate(true).SetEase(Ease.InCubic)
            .OnComplete(() => asyncLoad.allowSceneActivation = true);
    }

    private void WipeFadeOut() {
        //Remove two slashes when implementing sound
        //SFX.Play("sound");
        DOTween.To(() => bg.fillAmount, x => bg.fillAmount = x, 0f, .7f).SetUpdate(true).SetEase(Ease.InCubic);
    }
    
    private AsyncOperation asyncLoad;
    IEnumerator LoadAsyncScene(string sceneName) {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads.
        // This includes actually starting the scene, so the coroutine wont stop until the scene is changed.
        while (!asyncLoad.isDone) {
            yield return new WaitForSeconds(.1f);
        }
    }

    /*
    IEnumerator FadeIt()
    {

        while (!start)
        {
            //waiting to start
            yield return null;
        }
        lastTime = Time.unscaledTime;
        float coDelta = lastTime;
        bool hasFadedIn = false;

        while (!hasFadedIn)
        {
            coDelta = Time.unscaledTime - lastTime;
            if (coDelta > 1f) {
                coDelta = 0f;
            }
            if (!isFadeIn)
            {
                //Fade in
                alpha = newAlpha(coDelta, 1, alpha);
                if (alpha == 1 && !startedLoading)
                {
                    startedLoading = true;
                    SceneManager.LoadScene(fadeScene);
                }

            }
            else
            {
                //Fade out
                alpha = newAlpha(coDelta, 0, alpha);
                if (alpha == 0)
                {
                    hasFadedIn = true;
                }
            }
            
            lastTime = Time.unscaledTime;
            myCanvas.alpha = alpha;
            yield return null;
        }

        Initiate.DoneFading();

        Debug.Log("Your scene has been loaded , and fading in has just ended");

        Destroy(gameObject);

        yield return null;
    }


    float newAlpha(float delta, int to, float currAlpha)
    {

        switch (to)
        {
            case 0:
                currAlpha -= fadeDamp * delta;
                if (currAlpha <= 0)
                    currAlpha = 0;

                break;
            case 1:
                currAlpha += fadeDamp * delta;
                if (currAlpha >= 1)
                    currAlpha = 1;

                break;
        }

        return currAlpha;
    }
    */

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        WipeFadeOut();
    }
}
