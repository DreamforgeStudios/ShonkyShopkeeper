using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderAndBehaviour : MonoBehaviour {
    public Slider timerSlider;
    public Image sliderImage;
    public Text qualityText;
    public Button next;
    public Button retry;
    private Camera mainCamera;
    public GameObject tracer;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        qualityText.enabled = false;
        //next.enabled = false;
        //retry.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        timerSlider.value = tracer.GetComponent<Tracing>().currentTime;
        timerSlider.minValue = tracer.GetComponent<Tracing>().startTime;
        timerSlider.maxValue = tracer.GetComponent<Tracing>().finishTime;
        Color sliderColour = Color.Lerp(Color.green, Color.red, timerSlider.value / timerSlider.maxValue);
        sliderImage.color = sliderColour;
    }

    public void QualityText() {
        next.enabled = true;
        retry.enabled = true;
        qualityText.enabled = true;
        qualityText.text = "Quality: " + Quality.GradeToString(tracer.GetComponent<Tracing>().grade);
        qualityText.color = Quality.GradeToColor(tracer.GetComponent<Tracing>().grade);
    }
}
