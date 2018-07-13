using UnityEngine;
using TMPro;

public class UpdateRetryButton : MonoBehaviour {

    public TextMeshProUGUI buttonText;
	// Use this for initialization
	void Start () {
        buttonText = GetComponent<TextMeshProUGUI>();
    }

    public void SetText() {
        buttonText.text = "Retries Remaining: " + GameManager.Instance.RetriesRemaining;
    }
	
}
