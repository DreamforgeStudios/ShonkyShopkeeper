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
	public string DebugSceneName;

	private PseudoScene[] canvasScenes;
	private PseudoScene activeSceneCanvasObject;

	void Awake() {
		//scenes = Scenes.transform.GetComponentsInChildren<PseudoScene>(true);
		canvasScenes = CanvasScenes.transform.GetComponentsInChildren<PseudoScene>(true);
		
		//activeSceneObject = scenes[0];
		activeSceneCanvasObject = canvasScenes[0];
	}

	public void ChangeScene(string to) {
		PseudoScene canvasObj = null;
		
		/*
		foreach (var pseudoScene in scenes) {
			if (pseudoScene.gameObject.name == to) {
				obj = pseudoScene;
			}
		}
		*/
		
		foreach (var canvasScene in canvasScenes) {
			if (canvasScene.gameObject.name == to) {
				canvasObj = canvasScene;
			}
		}

		if (canvasObj) {
			// Perform packup routine (probably means animations).
			//activeSceneObject.Depart();
			activeSceneCanvasObject.Depart()
				.onComplete += () => canvasObj.Arrive();
			activeSceneCanvasObject = canvasObj;

			//obj.Arrive();
			//canvasObj.Arrive();

			//activeSceneObject = obj;
		} else {
			Debug.LogWarning("Couldn't find scene with name: " + to + " , is it typed correctly?");
		}
	}

	[Button("ChangeSceneDebug")]
	private void ChangeSceneDebug() {
		ChangeScene(DebugSceneName);
	}
}
