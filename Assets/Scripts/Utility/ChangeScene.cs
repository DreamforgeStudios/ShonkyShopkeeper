using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {
	public void ChangeOrRestartScene(string scene) {
        //SceneManager.LoadScene(scene);
		Initiate.Fade(scene, Color.black, 2f);
    }
}
