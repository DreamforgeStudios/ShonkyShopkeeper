using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is designed to be used in conjunction with the Hall scene to provide the relevant narrative associated
 * with true golems
 */
public class TrueGolemIntro : MonoBehaviour {
	//Relevant positions for golem spawning and movement
	public Vector3 golemStartPoint, rubyPedestal, sapphirePedestal, emeraldPedestal, amethystPedestal;
	
	//Relevant camera positions for each said pedestal
	public Vector3 rubyCamera, sapphireCamera, emeraldCamera, amethystCamera, cameraDefaultPosition;
	
	//For holding the relevant position to this golem
	private Vector3 desiredPedestalPos, desiredCameraPos;
	
	//True golem animation and dialogue
	public void IntroduceTrueGolem()
	{
		/*Intro animation*/
		//First spawn the relevant golem
		ItemInstance newGolem = 
			new ItemInstance(GameManager.Instance.typeOfTrueGolem, 1, Quality.QualityGrade.Mystic, true);
		Quaternion defaultRot = new Quaternion();
		GameObject clone = Instantiate(newGolem.item.physicalRepresentation, golemStartPoint, defaultRot);
		
		//Move spawned golem to the relevant pedestal 
		
		//Move camera in on relevant golem
		
		//Strike a pose and then morph into true golem
		
		//Update relevant achievement variable
		
		//Start dialogue
		
		//onClose of dialogue, reset relevant variables and allow player to move camera back to start (auto set right now)
		GameManager.Instance.introduceTrueGolem = false;
		GameManager.Instance.canUseTools = true;
		
		Camera.main.transform.position = cameraDefaultPosition;
	}
}
