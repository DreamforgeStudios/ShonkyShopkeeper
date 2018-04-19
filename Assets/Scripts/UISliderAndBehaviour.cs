using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderAndBehaviour : MonoBehaviour {
    public Slider timerSlider;
    public Image sliderImage;
    public Text qualityText;
    private Camera mainCamera;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;

	}
	
	// Update is called once per frame
	void Update () {
        timerSlider.minValue = mainCamera.GetComponent<Tracing>().startTime;
        timerSlider.value = mainCamera.GetComponent<Tracing>().currentTime;
        timerSlider.maxValue = mainCamera.GetComponent<Tracing>().finishTime;
        Color sliderColour = Color.Lerp(Color.green, Color.red, timerSlider.value / timerSlider.maxValue);
        sliderImage.color = sliderColour;
        QualityText();
    }

    public void QualityText() {
        qualityText.text = "Quality = " + mainCamera.GetComponent<Tracing>().itemQuality;
    }
}
