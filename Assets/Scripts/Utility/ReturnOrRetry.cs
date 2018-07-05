using UnityEngine.SceneManagement;

public class ReturnOrRetry {
	public static void Return(string itemString, Quality.QualityGrade grade) {
        Inventory.Instance.InsertItem(new ItemInstance(itemString, 1, grade, true));
		SceneManager.LoadScene("Shop");
	}

	public static void Retry() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
