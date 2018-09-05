using UnityEngine;
using TMPro;

public class UpdateRetryButton : MonoBehaviour {

    public TextMeshProUGUI ButtonText;

    public GameObject NormalButtons, JunkWarning;
	// Use this for initialization
	void Start () {
        ButtonText = GetComponent<TextMeshProUGUI>();
    }

    public void SetText()
    {
        ButtonText.text = string.Format("Retries Remaining: {0}", GameManager.Instance.RetriesRemaining);
    }

    public void WarningTextEnable()
    {
        JunkWarning.SetActive(true);
        NormalButtons.SetActive(false);
    }
	
}
