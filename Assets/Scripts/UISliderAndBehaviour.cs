using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderAndBehaviour : MonoBehaviour {
    public Slider timerSlider;
    public Image sliderImage;
    public Text qualityText;
    private Camera mainCamera;
    public GameObject tracer;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;


	}
	
	// Update is called once per frame
	void Update () {
        timerSlider.value = tracer.GetComponent<Tracing>().currentTime;
        timerSlider.minValue = tracer.GetComponent<Tracing>().startTime;
        timerSlider.maxValue = tracer.GetComponent<Tracing>().finishTime;
        Color sliderColour = Color.Lerp(Color.green, Color.red, timerSlider.value / timerSlider.maxValue);
        sliderImage.color = sliderColour;
        QualityText();
    }

    public void QualityText() {
        qualityText.text = "Quality = " + tracer.GetComponent<Tracing>().itemQuality;
    }
}
