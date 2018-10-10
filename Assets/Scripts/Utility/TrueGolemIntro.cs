﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/*
 * This class is designed to be used in conjunction with the Hall scene to provide the relevant narrative associated
 * with true golems
 */
public class TrueGolemIntro : MonoBehaviour {
	//Relevant positions for golem spawning and movement
	public Vector3 golemStartPoint, rubyPedestalPos, rubyPedestalRot, sapphirePedestalPos, sapphirePedestalRot,
		emeraldPedestalPos, emeraldPedestalRot, amethystPedestalPos, amethystPedestalRot;
	
	//Relevant camera positions and rotations for each said pedestal
	public Vector3 rubyCamera, sapphireCamera, emeraldCamera, amethystCamera, cameraDefaultPosition, rubyCameraRot, 
	sapphireCameraRot, emeraldCameraRot, amethystCameraRot, defaultCameraRot;
	
	//For holding the relevant position to this golem
	private Vector3 desiredPedestalPos, desiredCameraPos;
	
	//Gameobject that holds the newly spawned normal golem which has to move to the pedestal. trueGolem holds reference
	//to the enable true golem when it is enabled in the sequence
	private GameObject golemClone, trueGolem;
	
	//The True Golem gameobjects set in the hall that just need to be set as active.
	public List<GameObject> trueGolemObjects;
	
	//Particle systems and holders for the transformation
	public GameObject transformationParticles, glowParticles;
	private GameObject transformationObject, glowObject;
	
	//Reference to the main Hall script to aid in camera transitions when revisting golems
	public Hall hallFunctionality;
	
	//Reference to the golem selected so I can make dialogue reappear at a later visit
	public GameObject golemSelected;
	public bool inspectingGolem, readingDialogue;

	//Copies of golem dialogue so they can be reshown at a later visit
	public List<string> rubyGolemDialogue, sapphireGolemDialogue, emeraldGolemDialogue, amethystGolemDialogue;
	
	//Gizmo prefab and holder
	public GameObject gizmoPrefab;
	
	//Initial method to start True golem animation and dialogue
	public void IntroduceTrueGolem()
	{
		/*Intro animation*/
		//First spawn the relevant golem and attach the glow system
		ItemInstance newGolem = 
			new ItemInstance(GameManager.Instance.typeOfTrueGolem, 1, Quality.QualityGrade.Mystic, true);
		Quaternion defaultRot = new Quaternion();
		golemClone = Instantiate(newGolem.item.physicalRepresentation, golemStartPoint, defaultRot);
		//Resize due to hall scene being smaller
		golemClone.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
		
		glowObject = Instantiate(glowParticles, golemClone.transform);
		glowObject.transform.localPosition = new Vector3(0f,0f,0f);
		glowObject.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
		
		//Disable all the components on the golem
		golemClone.GetComponent<ShonkyWander>().enabled = false;
		golemClone.GetComponent<NavMeshAgent>().enabled = false;
		golemClone.GetComponent<Rigidbody>().useGravity = false;
		
		//Set relevant hall variables
		hallFunctionality.forward = true;
		
		//Move spawned golem to the relevant pedestal 
		MoveGolem();
		
	}

	//Determines the type of golem and where it should move to
	private void MoveGolem()
	{
		//Get type of golem in enum form
		TrueGolems.TrueGolem golemType = GetTypeOfGolem();
		
		//Send to desired position and move/rot camera after movement complete
		//Separate methods to allow for future polish in regards to movement individualisation
		switch (golemType)
		{
			case TrueGolems.TrueGolem.amethystGolem:
				MoveToAmethystPedestal();
				break;
			case TrueGolems.TrueGolem.emeraldGolem:
				MoveToEmeraldPedestal();
				break;
			case TrueGolems.TrueGolem.rubyGolem:
				MoveToRubyPedestal();
				break;
			case TrueGolems.TrueGolem.sapphireGolem:
				MoveToSapphirePedestal();
				break;
		}
	}

	//Used to determine where the golem should move to and what dialogue should be displayed
	private TrueGolems.TrueGolem GetTypeOfGolem()
	{
		TrueGolems.TrueGolem currentType;
		switch (GameManager.Instance.typeOfTrueGolem)
		{
			case "EmeraldGolem1":
				currentType = TrueGolems.TrueGolem.emeraldGolem;
				break;
			case "AmethystGolem1":
				currentType = TrueGolems.TrueGolem.amethystGolem;
				break;
			case "RubyGolem1":
				currentType = TrueGolems.TrueGolem.rubyGolem;
				break;
			case "SapphireGolem1":
				currentType = TrueGolems.TrueGolem.sapphireGolem;
				break;
			default:
				currentType = TrueGolems.TrueGolem.rubyGolem;
				break;
		}
		return currentType;
	}

	private void MoveToRubyPedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(rubyPedestalPos, 2f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(rubyPedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToSapphirePedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(sapphirePedestalPos, 2f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(sapphirePedestalRot, 1f, RotateMode.Fast)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToEmeraldPedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(emeraldPedestalPos, 2f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(emeraldPedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToAmethystPedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(amethystPedestalPos, 2f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(amethystPedestalRot, 1f, RotateMode.Fast)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	//Moves the camera in front of the desired golem pedestal
	private void MoveCamera(TrueGolems.TrueGolem golemPedestal)
	{
		switch (golemPedestal)
		{
			case TrueGolems.TrueGolem.amethystGolem:
				Camera.main.transform.DOMove(amethystCamera, 2f, false).OnComplete(()=> TrueGolemMorph());
				Camera.main.transform.DORotate(amethystCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.emeraldGolem:
				Camera.main.transform.DOMove(emeraldCamera, 2f, false).OnComplete(()=> TrueGolemMorph());
				Camera.main.transform.DORotate(emeraldCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.rubyGolem:
				Camera.main.transform.DOMove(rubyCamera, 2f, false).OnComplete(()=> TrueGolemMorph());
				Camera.main.transform.DORotate(rubyCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.sapphireGolem:
				Camera.main.transform.DOMove(sapphireCamera, 2f, false).OnComplete(()=> TrueGolemMorph());
				Camera.main.transform.DORotate(sapphireCameraRot, 1f, RotateMode.FastBeyond360);
				break;
		}
	}
	
	//Method to make golem "strike a pose" and then morph into the true golem
	private void TrueGolemMorph()
	{
		//Pose
		golemClone.GetComponent<Animator>().Play("Dance");
		
		//Start Coroutine which handles the morphing
		StartCoroutine(ParticleCoroutine());
		
	}
	
	//Method to Update achievements backend
	private void UpdateAchievements()
	{
		string gemType;
		//Trigger dialogue by updating achievements
		switch (GameManager.Instance.typeOfTrueGolem)
		{
			case "EmeraldGolem1":
				NarrativeManager.Read("true_emerald_speech");
				gemType = "emerald";
				break;
			case "AmethystGolem1":
				NarrativeManager.Read("true_amethyst_speech");
				gemType = "amethyst";
				break;
			case "RubyGolem1":
				NarrativeManager.Read("true_ruby_speech");
				gemType = "ruby";
				break;
			case "SapphireGolem1":
				NarrativeManager.Read("true_sapphire_speech");
				gemType = "sapphire";
				break;
			default:
				NarrativeManager.Read("true_ruby_speech");
				gemType = "ruby";
				break;
		}
		
		//Unlock relevant true golem in inventory so it appears in future visits to the hall
		Inventory.Instance.UnlockTrueGolem(TrueGolems.GemStringToGolem(gemType));
		
		//Depending on the number of true golems created, may need to trigger game end sequence
		List<TrueGolems.TrueGolem> golemsUnlocked = Inventory.Instance.GetUnlockedTrueGolems();

		if (golemsUnlocked.Count < 4)
		{
			//onClose of dialogue, reset relevant variables and allow player to move camera back to start (auto set right now)
			PopupTextManager.onClose += () => FinishSequence();
		}
		else
		{
			PopupTextManager.onClose += () => FinishSequence();
		}
	}
	
	
	//Finish the sequence and reset relevant variables
	private void FinishSequence()
	{
		GameManager.Instance.introduceTrueGolem = false;
		GameManager.Instance.canUseTools = true;
		inspectingGolem = true;
		
		hallFunctionality.MoveCameraBackButton.SetActive(true);
		PopupTextManager.ResetEvents();
	}

	//Starts relevant particles and then morphs
	private IEnumerator ParticleCoroutine()
	{
		yield return new WaitForSeconds(2f);
		
		//Start concentrated Dance
		//golemClone.GetComponent<Animator>().Play("Dance2");
		
		//Morph by moving upwards
		golemClone.transform.DOMoveY(golemClone.transform.position.y + 0.1f, 2f, false);
		
		yield return new WaitForSeconds(1.5f);
		
		//Start transformation particles and scale correctly
		Vector3 desiredPos = golemClone.transform.position + (Vector3.forward / 10);
		transformationObject = Instantiate(transformationParticles, desiredPos,golemClone.transform.rotation);
		transformationObject.transform.localScale = new Vector3(2f,2f,2f);
		
		yield return new WaitForSeconds(0.5f);
		
		//After waiting disable old glow particle (and golem) and enable true golem object
		golemClone.SetActive(false);
		switch (GameManager.Instance.typeOfTrueGolem)
		{
			case "EmeraldGolem1":
				trueGolem = trueGolemObjects[1];
				trueGolem.SetActive(true);
				break;
			case "AmethystGolem1":
				trueGolem = trueGolemObjects[3];
				trueGolem.SetActive(true);
				break;
			case "RubyGolem1":
				trueGolem = trueGolemObjects[0];
				trueGolem.SetActive(true);
				break;
			case "SapphireGolem1":
				trueGolem = trueGolemObjects[2];
				trueGolem.SetActive(true);
				break;
			default:
				trueGolem = trueGolemObjects[0];
				trueGolem.SetActive(true);
				break;
		}

		golemSelected = trueGolem;
		
		yield return new WaitForSeconds(3f);
		
		//Stop transformation particles from emitting.
		transformationObject.GetComponent<ParticleSystem>().Stop();
		
		yield return new WaitForSeconds(2f);
		
		//Update Achievements which automatically triggers the dialogue
		UpdateAchievements();
	}

	public void HighlightTrueGolem(GameObject golemHit)
	{
		golemSelected = golemHit;
		switch (golemHit.name)
		{
			case "TrueRubyGolem":
				Camera.main.transform.DOMove(rubyCamera, 2f, false);
				Camera.main.transform.DORotate(rubyCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case "TrueAmethystGolem":
				Camera.main.transform.DOMove(amethystCamera, 2f, false);
				Camera.main.transform.DORotate(amethystCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case "TrueSapphireGolem":
				Camera.main.transform.DOMove(sapphireCamera, 2f, false);
				Camera.main.transform.DORotate(sapphireCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case "TrueEmGolemv1":
				Camera.main.transform.DOMove(emeraldCamera, 2f, false);
				Camera.main.transform.DORotate(emeraldCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			default:
				Camera.main.transform.DOMove(rubyCamera, 2f, false);
				Camera.main.transform.DORotate(rubyCameraRot, 1f, RotateMode.FastBeyond360);
				break;
		}
		inspectingGolem = true;
	}

	public void ReshowDialogue()
	{
		readingDialogue = true;
		List<string> selectedDialogue;
		switch (golemSelected.name)
		{
			case "TrueRubyGolem":
				selectedDialogue = rubyGolemDialogue;
				break;
			case "TrueAmethystGolem":
				selectedDialogue = amethystGolemDialogue;
				break;
			case "TrueSapphireGolem":
				selectedDialogue = sapphireGolemDialogue;
				break;
			case "TrueEmGolemv1":
				selectedDialogue = emeraldGolemDialogue;
				break;
			default:
				selectedDialogue = rubyGolemDialogue;
				break;
		}
		
		PopupTextManager clone = Instantiate(gizmoPrefab, GameObject.FindGameObjectWithTag("MainCamera").transform).GetComponentInChildren<PopupTextManager>();
		clone.PopupTexts = selectedDialogue;
		clone.Init();
		clone.DoEnterAnimation();
		
		hallFunctionality.MoveCameraBackButton.SetActive(false);
		PopupTextManager.onClose += () => ReenableButton();

	}

	private void ReenableButton()
	{
		hallFunctionality.MoveCameraBackButton.SetActive(true);
		readingDialogue = false;
		PopupTextManager.ResetEvents();
	}
}
