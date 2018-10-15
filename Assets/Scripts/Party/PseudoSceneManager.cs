using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.UI;

[System.Serializable]
public class SceneObjectDictionary : SerializableDictionary<string, GameObject> {}

[System.Serializable]
public class SceneCanvasObjectDictionary : SerializableDictionary<string, GameObject> {}

public class PseudoSceneManager : MonoBehaviour {
	//public SceneObjectDictionary SceneObjectDict;
	//public SceneCanvasObjectDictionary SceneCanvasObjectDict;

	//public GameObject DefaultScene, DefaultCanvasScene;

	public Image Fader;
	public GameObject CanvasScenes;
	public bool SetPartyMode;
	public string DebugSceneName;

	private PseudoScene[] canvasScenes;
	private PseudoScene activeSceneCanvasObject;

	void Awake() {
		//scenes = Scenes.transform.GetComponentsInChildren<PseudoScene>(true);
		canvasScenes = CanvasScenes.transform.GetComponentsInChildren<PseudoScene>(true);
		
		//activeSceneObject = scenes[0];
		activeSceneCanvasObject = canvasScenes[0];

		if (SetPartyMode)
			GameManager.Instance.ActiveGameMode = GameMode.Party;
	}

	public void ChangeScene(string to) {
		PseudoScene canvasObj = null;
		
		foreach (var canvasScene in canvasScenes) {
			if (canvasScene.gameObject.name == to) {
				canvasObj = canvasScene;
			}
		}

		if (canvasObj) {
            activeSceneCanvasObject.Depart()
                .onComplete += () => canvasObj.Arrive();
			activeSceneCanvasObject = canvasObj;
		} else {
			Debug.LogWarning("Couldn't find scene with name: " + to + " , is it typed correctly?");
		}
	}

	public void ChangeSceneWithoutAnimation(string to) {
		PseudoScene canvasObj = null;
		
		foreach (var canvasScene in canvasScenes) {
			if (canvasScene.gameObject.name == to) {
				canvasObj = canvasScene;
			}
		}

		if (canvasObj) {
            activeSceneCanvasObject.Depart(false);
            canvasObj.Arrive(false);
			activeSceneCanvasObject = canvasObj;
		} else {
			Debug.LogWarning("Couldn't find scene with name: " + to + " , is it typed correctly?");
		}
		
	}

	[Button("ChangeSceneDebug")]
	private void ChangeSceneDebug() {
		ChangeScene(DebugSceneName);
	}

	[Button("ChangeSceneDebugNoAnimations")]
	private void ChangeSceneDebugNoAnimation() {
		ChangeSceneWithoutAnimation(DebugSceneName);
	}
}
