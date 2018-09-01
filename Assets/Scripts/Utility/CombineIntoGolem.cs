using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CombineIntoGolem : MonoBehaviour
{
	public GameObject item1position, item2Position;
	public Camera mainCamera;

	private int oldCameraCullingMask;
	//UI image to fade background
	public Image BGFader;

	public void GolemAnimationSequence(Slot currentSelection, Slot slot)
	{
		oldCameraCullingMask = mainCamera.cullingMask;
		//Change Cameras to highlight sequence
		ChangeCameras(false);
		
		//Get objects and move up
		GameObject obj1, obj2;
		Vector2 midPoint;
		if (currentSelection.GetPrefabInstance(out obj1) && slot.GetPrefabInstance(out obj2)) {
			Transform t1 = obj1.transform,
				t2 = obj2.transform;
			//Change object layers show they are shown by new camera
			obj1.layer = 16;
			obj2.layer = 16;
                            
			midPoint = (((currentSelection.transform.position + Vector3.up) + (slot.transform.position + Vector3.up)) / 2f);
			Debug.Log("midPoint is " + midPoint);
                            
			//SFX.Play("item_lift");
			t2.DOMove(slot.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
				.OnComplete(() => t2.DOMove(item1position.transform.position, 0.6f).SetEase(Ease.OutBack));
			t1.DOMove(currentSelection.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
				.OnComplete(() => t1.DOMove(item2Position.transform.position, 0.6f).SetEase(Ease.OutBack).OnComplete(() => ChangeCameras(true)));
			//.OnComplete(() => CombineItems(slot));
		}
		
		
		//Return Cameras to normal
		//ChangeCameras(true);
	}

	private void ChangeCameras(bool UseNormal)
	{
		if (UseNormal)
		{
			BGFader.CrossFadeAlpha(2f,0.5f,false);
			BGFader.gameObject.SetActive(false);
			mainCamera.cullingMask = oldCameraCullingMask;
			mainCamera.gameObject.layer = 
		}
		else
		{
			//BGFader.gameObject.SetActive(true);
			//BGFader.CrossFadeAlpha(215f,0.5f,false);
			mainCamera.cullingMask = (1 << LayerMask.NameToLayer("Tools")) 
			                         | (1 << LayerMask.NameToLayer("Shop")) 
										| (1 << LayerMask.NameToLayer("InventoryBin"));
		}
	}
}
