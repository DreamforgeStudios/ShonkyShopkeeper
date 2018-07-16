using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementGameObject : MonoBehaviour {
    public TextMeshProUGUI Heading;
    public TextMeshProUGUI Description;
    public Image Icon;
    public Image Background;

    public void HideAfterSeconds(float seconds) {
        StartCoroutine(HideAfterSeconds(seconds, this.gameObject));
    }
    
    IEnumerator HideAfterSeconds(float seconds, GameObject obj) {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
    
}
