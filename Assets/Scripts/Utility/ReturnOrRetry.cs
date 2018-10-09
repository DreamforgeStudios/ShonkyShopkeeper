using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnOrRetry {
	public static void Return(string itemString, Quality.QualityGrade grade) {
		//SFX.Play("sound");
		Debug.Log("grade is " + grade);
		if (grade != Quality.QualityGrade.Junk)
			Inventory.Instance.InsertItem(new ItemInstance(itemString, 1, grade, true));
		//SceneManager.LoadScene("Shop");
		if (!GameManager.Instance.InTutorial) {
			Initiate.Fade("Shop", Color.black, 2f);
		} else {
			TutorialProgressChecker.Instance.FinishedComponent(itemString);
			Initiate.Fade("TutorialShop", Color.black, 2f);
		}
	}
	
	public static void Return(int goldAmount, int shonkyIndex) {
        //SFX.Play("sound");
        SFX.Play("Mini_Game_Retry_Button", 1f, 1f, 0f, false, 0f);
        Inventory.Instance.AddGold(goldAmount);
        ShonkyInventory.Instance.RemoveItem(shonkyIndex);
		GameManager.pickedUpGolem = false;
		SaveManager.SaveShonkyInventory();
		//SceneManager.LoadScene("Shop");
		// TODO: tutorial consideration for bartering.
		if (!GameManager.Instance.InTutorial)
			Initiate.Fade("Shop", Color.black, 2f);
		else
			Initiate.Fade("TutorialShop", Color.black, 2f);
	}

	public static void Retry() {
        if (GameManager.Instance.RetriesRemaining > 0) {
            //SFX.Play("sound");
            SFX.Play("Mini_Game_Retry_Button", 1f, 1f, 0f, false, 0f);
            GameManager.Instance.RetriesRemaining--;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	        Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 2f);
	    // Allow retrying in editor for quick changes.
        } else if (Application.isEditor) {
	        Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 2f);
        }
	}

	public static void ReturnParty(float points) {
		// Dequeue the round, because it has been completed.
		var roundInfo = GameManager.Instance.RoundQueue.Dequeue();
		// Keep a history of rounds that we've played.
		GameManager.Instance.RoundHistory.AddFirst(new PostRoundInfo(roundInfo, (int)points));
		GameManager.Instance.PlayerInfos[roundInfo.PlayerIndex].Points += points;
		Initiate.Fade("RoundLobby", Color.black, 2f);
	}
	
	public static void ReturnParty(int gold) {
		// Dequeue the round, because it has been completed.
		var roundInfo = GameManager.Instance.RoundQueue.Dequeue();
		// Keep a history of rounds that we've played.
		GameManager.Instance.RoundHistory.AddFirst(new PostRoundInfo(roundInfo, 0, gold));
		GameManager.Instance.PlayerInfos[roundInfo.PlayerIndex].Gold += gold;
		Initiate.Fade("RoundLobby", Color.black, 2f);
	}
}
