using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {
	public void ChangeOrRestartScene(string scene) {
        //SceneManager.LoadScene(scene);
		if (GameManager.Instance.canUseTools)
		{
			if (scene == "Hall")
				SFX.Play("Achieve_Hall_Tap", 1f, 1f, 0f, false, 0f);

			Initiate.Fade(scene, Color.black, 2f);
		}
	}
}
