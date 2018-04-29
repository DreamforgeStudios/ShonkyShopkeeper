using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetGrade : MonoBehaviour {
	public TextMeshProUGUI gradeText;

	// Use this for initialization
	void Start () {
		if (GameManager.instance) {
			Quality.QualityGrade grade = GameManager.instance.GetQuality();
			gradeText.text = Quality.GradeToString(grade);
			gradeText.color = Quality.GradeToColor(grade);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
