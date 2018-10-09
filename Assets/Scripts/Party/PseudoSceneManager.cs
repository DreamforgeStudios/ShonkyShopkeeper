using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class SceneObjectDictionary : SerializableDictionary<string, GameObject> {}

[System.Serializable]
public class SceneCanvasObjectDictionary : SerializableDictionary<string, GameObject> {}

public class PseudoSceneManager : MonoBehaviour {
	//public SceneObjectDictionary SceneObjectDict;
	//public SceneCanvasObjectDictionary SceneCanvasObjectDict;

	//public GameObject DefaultScene, DefaultCanvasScene;

	public GameObject Scenes, CanvasScenes;

	private PseudoScene[] scenes, canvasScenes;
	private PseudoScene activeSceneObject, activeSceneCanvasObject;

	void Awake() {
		scenes = Scenes.transform.GetComponentsInChildren<PseudoScene>(true);
		canvasScenes = CanvasScenes.transform.GetComponentsInChildren<PseudoScene>(true);
		
		activeSceneObject = scenes[0];
		activeSceneCanvasObject = canvasScenes[0];
	}

	public void ChangeScene(string to) {
		PseudoScene obj = null, canvasObj = null;
		
		foreach (var pseudoScene in scenes) {
			if (pseudoScene.gameObject.name == to) {
				obj = pseudoScene;
			}
		}
		
		foreach (var canvasScene in canvasScenes) {
			if (canvasScene.gameObject.name == to) {
				canvasObj = canvasScene;
			}
		}

		if (obj && canvasObj) {
			// Perform packup routine (probably means animations).
			activeSceneObject.Depart();
			activeSceneCanvasObject.Depart();
			
			obj.Arrive();
			canvasObj.Arrive();

			activeSceneObject = obj;
			activeSceneCanvasObject = canvasObj;
		} else {
			Debug.LogWarning("Couldn't find scene with name: " + to + " , is it typed correctly?");
		}
	}
}
