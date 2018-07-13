using UnityEngine.SceneManagement;

public class ReturnOrRetry {
	public static void Return(string itemString, Quality.QualityGrade grade) {
        Inventory.Instance.InsertItem(new ItemInstance(itemString, 1, grade, true));
		SceneManager.LoadScene("Shop");
	}

	public static void Retry() {
        if (GameManager.instance.CanRetry()) {
            GameManager.instance.currentRetryNumber++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}

    public static bool CanRetry() {
        return GameManager.instance.CanRetry();
    }
}
