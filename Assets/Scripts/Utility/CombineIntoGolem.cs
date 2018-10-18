﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using DG.Tweening;
using Spektr;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;

public class CombineIntoGolem : MonoBehaviour
{
	public GameObject item1Position, item2Position, desiredPosition;
	public Camera mainCamera;
	public Canvas canvasOverlay, sceneCanvas;
	private int oldCameraCullingMask;
	public ParticleSystem smoke;
	public Toolbox toolbox;
	public PhysicalShonkyInventory physicalShonkyInventory;
	
	//UI image to fade background
	public RawImage BGFader;
	public TextMeshProUGUI golemText, golemBottomText;
	
	//Used for rotation of objs and background image
	public Vector3 obj1Rotation, obj2Rotation;
	private float BGFaderXRot;
	private Vector3 shellPos;
	
	//Capture gem type in case the golem is a true golem
	private string gemType;
	
	//Particle variables to showcase true golem
	public GameObject glowParticle;
	private GameObject glowObject;
	
	//Scene Changer to transition when closing popup
	public ChangeScene sceneChanger;
	
	public void GolemAnimationSequence(Slot currentSelection, Item currentSelectType, Slot slot, Item slotType)
	{
		oldCameraCullingMask = mainCamera.cullingMask;
		//Change Cameras to highlight sequence
		ChangeCameras(false);
		
		//Disable scene Canvas to remove all buttons and overlay text
		sceneCanvas.gameObject.SetActive(false);
		
		//Disable use of the toolbox
		GameManager.Instance.canUseTools = false;
		GameManager.Instance.introduceTrueGolem = true;
		
		//Get objects and move up
		GameObject obj1, obj2;
		if (currentSelection.GetPrefabInstance(out obj1) && slot.GetPrefabInstance(out obj2)) {
			Transform t1 = obj1.transform,
				t2 = obj2.transform; 
			//Stop Rotate object script
			obj1.GetComponent<Rotate>().enabled = false;
			obj2.GetComponent<Rotate>().enabled = false;

			if (currentSelectType.GetType() == typeof(Shell))
			{
				t2.DOMove(slot.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
					.OnComplete(() => t2.DOMove(item2Position.transform.position, 0.6f).SetEase(Ease.OutBack)
						.OnComplete(() =>
							t2.DOLocalRotate(obj2Rotation, 0.5f, RotateMode.Fast)));

				t1.DOMove(currentSelection.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
					.OnComplete(() =>
						t1.DOMove(item1Position.transform.position, 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
							t1.DOLocalRotate(obj1Rotation, 0.5f, RotateMode.Fast).OnComplete(() =>
								StartAnimations(obj1, currentSelectType, obj2,currentSelection, slot))));

			}
			else
			{
				t2.DOMove(slot.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
					.OnComplete(() => t2.DOMove(item1Position.transform.position, 0.6f).SetEase(Ease.OutBack)
						.OnComplete(() =>
							t2.DOLocalRotate(obj1Rotation, 0.5f, RotateMode.Fast)));

				t1.DOMove(currentSelection.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
					.OnComplete(() =>
						t1.DOMove(item2Position.transform.position, 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
							t1.DOLocalRotate(obj2Rotation, 0.5f, RotateMode.Fast).OnComplete(() =>
								StartAnimations(obj1, currentSelectType, obj2, currentSelection, slot))));
			}
		}
	}

	private void StartAnimations(GameObject obj1, Item currentSelectType, GameObject obj2, Slot current, Slot slot)
	{
		Transform t1 = obj1.transform, t2 = obj2.transform; 
		if (currentSelectType.GetType() == typeof(Shell))
		{
			obj1.GetComponent<LightningRenderer>().enabled = true;
			t2.DOShakeScale(1f, 2.5f, 10, 90f, true).OnComplete(() => t2.DOScale(0.6f,1f));
			t1.DOMove(desiredPosition.transform.position, 2.2f, false);
			t2.DOMove(desiredPosition.transform.position + Vector3.up + Vector3.left, 2.2f, false);
			StartCoroutine(StartParticles(current, slot, obj1));
		}
		else
		{
			obj2.GetComponent<LightningRenderer>().enabled = true;
			t1.DOShakeScale(1f, 2.5f, 10, 90f, true).OnComplete(() => t1.DOScale(0.6f,1f));
			t1.DOMove(desiredPosition.transform.position + Vector3.up + Vector3.left, 2.2f, false);
			t2.DOMove(desiredPosition.transform.position, 2.2f, false);
			StartCoroutine(StartParticles(current, slot, obj2));
		}
	}

	private void FinaliseCombination(Slot current, Slot slot)
	{
		StopAllCoroutines();
		gemType = toolbox.FindGemType(current, slot);
		
		int index1, index2;
		index1 = current.index;
		index2 = slot.index;
		//SFX.Play("golem_created");
		Debug.Log("Created Golem");
		//Get the average quality of the shell and charged gem, assign to new golem.
		Quality.QualityGrade item1 = current.itemInstance.Quality;
		Quality.QualityGrade item2 = slot.itemInstance.Quality;
		Quality.QualityGrade avg = Quality.CalculateCombinedQuality(item1, item2);
		ItemInstance newGolem = new ItemInstance(gemType, 1, avg, true);
		string gem = (newGolem.item as Shonky).type.ToString();
		
		int index = ShonkyInventory.Instance.InsertItem(newGolem);
		if (index != -1)
		{
			Quaternion rot = Quaternion.Euler(obj1Rotation);
			PenSlot pSlot = physicalShonkyInventory.GetSlotAtIndex(index);
			GameObject clone = Instantiate(newGolem.item.physicalRepresentation, desiredPosition.transform.position,
				rot);
			clone.GetComponent<ShonkyWander>().enabled = false;
			clone.GetComponent<NavMeshAgent>().enabled = false;
			clone.GetComponent<Rigidbody>().useGravity = false;
			Inventory.Instance.RemoveItem(index1);
			Inventory.Instance.RemoveItem(index2);
			//Reset Variables in toolbox and remove held references to deleted objects
			pSlot.SetItemInstantiated(newGolem,clone);
			toolbox.ClearGolemCreation(slot);
			
			//If a true golem, do narrative handling
			if (TrueGolems.PotentialUnlockTrueGolem(TrueGolems.GemStringToGolem(gemType)))
			{
				//Show relevant dialogue based on amount of true golems previously made
				List<TrueGolems.TrueGolem> golemsUnlocked = Inventory.Instance.GetUnlockedTrueGolems();
				switch (golemsUnlocked.Count)
				{
					case 0:
						NarrativeManager.Read("true_golem_01");
						break;
					case 1:
						NarrativeManager.Read("true_golem_02");
						break;
					case 2:
						NarrativeManager.Read("true_golem_03");
						break;
					case 3:
						NarrativeManager.Read("true_golem_04");
						break;
				}
				PopupTextManager.onClose += () => TransitionToHall();
				
				//Instantiate glow on golem and make it dance
				glowObject = Instantiate(glowParticle, clone.transform);
				glowObject.transform.localPosition = new Vector3(0f,0f,0f);
				glowObject.transform.localScale = new Vector3(1f,1f,1f);
				clone.GetComponent<Animator>().Play("Dance");
				
				//Remove golem from golem inventory as the player does not receive one when first creating a true golem
				ShonkyInventory.Instance.RemoveItem(index);
			}
			else
			{
				StartCoroutine(ShowText(gem,avg, pSlot, clone));
			}
			
		}
	}

	private IEnumerator ShowText(string gemType, Quality.QualityGrade grade, PenSlot slot, GameObject golemObj)
	{
		golemText.enabled = true;
		golemBottomText.enabled = true;
		golemText.text = string.Format("New {0} {1} Golem!", grade, gemType);
		golemText.color = Quality.GradeToColor(grade);
		golemBottomText.color = Quality.GradeToColor(grade);
		yield return new WaitForSeconds(3f);
		if (golemObj != null)
			golemObj.transform.DOMove(slot.transform.position, 1f, false).OnComplete(() => RestartGolem(golemObj));
		else
			RestartGolem(null);
	}

	private void RestartGolem(GameObject clone)
	{
		if (clone != null)
		{
			clone.GetComponent<ShonkyWander>().enabled = true;
			clone.GetComponent<NavMeshAgent>().enabled = true;
			clone.GetComponent<Rigidbody>().useGravity = true;
		}

		StopAllCoroutines();
		ChangeCameras(true);
		
		//Reenable toolbox use
		GameManager.Instance.canUseTools = true;
		GameManager.Instance.introduceTrueGolem = false;
		
		//Reenable scene canvas
		sceneCanvas.gameObject.SetActive(true);
	}

	private IEnumerator StartParticles(Slot current, Slot slot, GameObject shell)
	{
		Animator shellAnimator = shell.GetComponent<Animator>();
		shellAnimator.SetBool("Zap", true);
		//shell.transform.DOMove(item1Position.transform.position, 1.3f, false);
		yield return new WaitForSeconds(1.3f);
		Instantiate(smoke, desiredPosition.transform.position + Vector3.up, smoke.transform.rotation);
		FinaliseCombination(current, slot);
	}
	
	private void ChangeCameras(bool useNormal)
	{
		if (useNormal)
		{
			BGFader.CrossFadeAlpha(0.0f,0.5f,false);
			BGFader.gameObject.SetActive(false);
			mainCamera.cullingMask = oldCameraCullingMask;
			mainCamera.gameObject.layer = 0;
			canvasOverlay.gameObject.layer = 5;
			golemText.enabled = false;
			golemBottomText.enabled = false;
		}
		else
		{
			BGFader.gameObject.SetActive(true);
			BGFader.CrossFadeAlpha(0.95f,5f,false);
			BGFader.transform.localEulerAngles = new Vector3(BGFaderXRot,0f,0f);
		}
	}
	
	//Used to transition to Hall
	private PopupTextManager.OnClose TransitionToHall()
	{
		GameManager.Instance.introduceTrueGolem = true;
		GameManager.Instance.typeOfTrueGolem = gemType;
		PopupTextManager.ResetEvents();
		Initiate.Fade("Hall", Color.black,2.0f);
		return () => {PopupTextManager.onClose -= TransitionToHall(); };
	}
}
