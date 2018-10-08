using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
	sapphireCameraRot, emeraldCameraRot, amethystCameraRot;
	
	//For holding the relevant position to this golem
	private Vector3 desiredPedestalPos, desiredCameraPos;
	
	//Gameobject that holds the newly spawned normal golem which has to move to the pedestal
	private GameObject golemClone;
	
	//Initial method to start True golem animation and dialogue
	public void IntroduceTrueGolem()
	{
		/*Intro animation*/
		//First spawn the relevant golem
		ItemInstance newGolem = 
			new ItemInstance(GameManager.Instance.typeOfTrueGolem, 1, Quality.QualityGrade.Mystic, true);
		Quaternion defaultRot = new Quaternion();
		golemClone = Instantiate(newGolem.item.physicalRepresentation, golemStartPoint, defaultRot);
		
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
		golemClone.transform.DOMove(rubyPedestalPos, 5f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(rubyPedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToSapphirePedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(sapphirePedestalPos, 5f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(sapphirePedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToEmeraldPedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(emeraldPedestalPos, 5f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(emeraldPedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	private void MoveToAmethystPedestal()
	{
		//Move golem and once moved, move camera
		golemClone.transform.DOMove(amethystPedestalPos, 5f, false).SetEase(Ease.InSine)
			.OnComplete(()=>golemClone.transform.DORotate(amethystPedestalRot, 1f, RotateMode.FastBeyond360)
				.OnComplete(()=>MoveCamera(GetTypeOfGolem())));
	}

	//Moves the camera in front of the desired golem pedestal
	private void MoveCamera(TrueGolems.TrueGolem golemPedestal)
	{
		switch (golemPedestal)
		{
			case TrueGolems.TrueGolem.amethystGolem:
				Camera.main.transform.DOMove(amethystCamera, 2f, false);
				Camera.main.transform.DORotate(amethystCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.emeraldGolem:
				Camera.main.transform.DOMove(emeraldCamera, 2f, false);
				Camera.main.transform.DORotate(emeraldCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.rubyGolem:
				Camera.main.transform.DOMove(rubyCamera, 2f, false);
				Camera.main.transform.DORotate(rubyCameraRot, 1f, RotateMode.FastBeyond360);
				break;
			case TrueGolems.TrueGolem.sapphireGolem:
				Camera.main.transform.DOMove(sapphireCamera, 2f, false);
				Camera.main.transform.DORotate(sapphireCameraRot, 1f, RotateMode.FastBeyond360);
				break;
		}
		
		//Continue sequence (should be moved to happen after camera transitions finish
		TrueGolemMorph();
	}
	
	//Method to make golem "strike a pose" and then morph into the true golem
	private void TrueGolemMorph()
	{
		//Pose
		
		//Morph
		
		//Update Achievements
		UpdateAchievements();
		
		//Start dialogue
		StartTrueGolemDialogue();
	}
	
	//Method to Update achievements backend
	private void UpdateAchievements()
	{
		
	}
	
	//Dialogue Handling
	private void StartTrueGolemDialogue()
	{
		//Dialogue
		
		
		//onClose of dialogue, reset relevant variables and allow player to move camera back to start (auto set right now)
		FinishSequence();
	}
	
	//Finish the sequence and reset relevant variables
	private void FinishSequence()
	{
		GameManager.Instance.introduceTrueGolem = false;
		GameManager.Instance.canUseTools = true;
		
		Camera.main.transform.position = cameraDefaultPosition;
	}
}
