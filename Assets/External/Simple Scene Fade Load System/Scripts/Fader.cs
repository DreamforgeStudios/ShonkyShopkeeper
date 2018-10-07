using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [HideInInspector]
    public string NextScene;
    [HideInInspector]
    public Color FadeColor;
    
    CanvasGroup myCanvas;
    Image bg;

    // Name of the loading scene.
    private const string InbetweenScene = "Loading";
    
    //Set callback
    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    
    //Remove callback
    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitiateFader() {
        DontDestroyOnLoad(gameObject);

        //Getting the visual elements
        if (transform.GetComponent<CanvasGroup>())
            myCanvas = transform.GetComponent<CanvasGroup>();

        if (transform.GetComponentInChildren<Image>()) {
            // Really simple texture.
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            
            bg = transform.GetComponent<Image>();
            bg.color = FadeColor;
            bg.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            // Alternatively, to use a custom image, place it in the Resources folder and use this.
            //bg.sprite = Resources.Load<Sprite>("LoadingSceneTransition");
            bg.type = Image.Type.Filled;
            bg.fillMethod = Image.FillMethod.Radial90;
        }
        
        //Checking and starting the coroutine
        if (myCanvas && bg) {
            bg.fillAmount = 0f;
            WipeFadeIn();
        }
    }

    private void WipeFadeIn() {
        SFX.Play("Screen_wipe", 1f, 1f, 0f, false, 0f);
        /*
        StartCoroutine(LoadAsyncScene(fadeScene));
        DOTween.To(() => bg.fillAmount, x => bg.fillAmount = x, 1f, .7f).SetUpdate(true).SetEase(Ease.InCubic)
            .OnComplete(() => asyncLoad.allowSceneActivation = true);
            */
        DOTween.To(() => bg.fillAmount, x => bg.fillAmount = x, 1f, .7f).SetUpdate(true).SetEase(Ease.InCubic)
            .OnComplete(() => SceneManager.LoadScene(InbetweenScene));
    }

    private void WipeFadeOut() {
        SFX.Play("Screen_wipe", 1f, 1f, 0f, false, 0f);
        
        DOTween.To(() => bg.fillAmount, x => bg.fillAmount = x, 0f, .7f).SetUpdate(true).SetEase(Ease.InCubic)
            .OnComplete(() => Initiate.DoneFading());
    }
    
    private AsyncOperation asyncLoad;
    IEnumerator LoadAsyncScene(string sceneName) {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        // Unity stops progress at 90% until you set allowSceneActivation to true...
        while (asyncLoad.progress < 0.9f) {
            yield return new WaitForSeconds(.1f);
        }
        
        OnAsyncFinishedLoading(sceneName);
        
        // Wait until the asynchronous scene fully loads.
        // This includes actually starting the scene, so the coroutine wont stop until the scene is changed.
        /*
        while (!asyncLoad.isDone) {
            yield return new WaitForSeconds(.1f);
        }
        */
    }

    void OnAsyncFinishedLoading(string sceneName) {
        if (sceneName == NextScene) {
            bg.DOColor(Color.black, .7f).OnComplete(() => {
                asyncLoad.allowSceneActivation = true;
                WipeFadeOut();
            });
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (scene.name == InbetweenScene) {
            bg.DOColor(Color.clear, .7f).OnComplete(() => StartCoroutine(LoadAsyncScene(NextScene)));
        }
    }
}
