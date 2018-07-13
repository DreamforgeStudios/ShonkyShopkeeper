using UnityEngine.SceneManagement;

// A class that reloads the current scene (probably a minigame) or returns to the shop.
public class ReturnOrRetry {
	// TODO: load the expected scene in the background.
	
	// Return to the shop, and so give the player their new item.
	public static void Return(string itemString, Quality.QualityGrade grade) {
        Inventory.Instance.InsertItem(new ItemInstance(itemString, 1, grade, true));
		SceneManager.LoadScene("Shop");
	}

	// Retry, so don't give the player an item.
	public static void Retry() {
		if (GameManager.Instance.RetriesRemaining > 0) {
			GameManager.Instance.RetriesRemaining--;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
