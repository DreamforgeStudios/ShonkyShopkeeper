using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdateRetryButton : MonoBehaviour {

    public TextMeshProUGUI ButtonText;

    public GameObject NormalButtons, JunkWarning;

    public Button ButtonShop, ButtonRetry;
    
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
        
        //Customise text to explain why they would receive an item in the tutorial
        if (!GameManager.Instance.InTutorial)
        {
            JunkWarning.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                "If you return to the shop, this resource will be destroyed.";
        }
        else
        {
            JunkWarning.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                "In the normal game, making a junk component will result in it being destroyed";
        }
    }

    public void DisableButtons() {
        ButtonShop.interactable = false;
        ButtonRetry.interactable = false;
    }
	
}
