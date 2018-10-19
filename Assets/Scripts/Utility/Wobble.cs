using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour {
	Vector3 lastPos;
	Vector3 velocity;
	Vector3 lastRot;  
	Vector3 angularVelocity;
	public float MaxWobble = 0.03f;
	public float WobbleSpeed = 1f;
	public float Recovery = 1f;
	float wobbleAmountX;
	float wobbleAmountZ;
	float wobbleAmountToAddX;
	float wobbleAmountToAddZ;
	float pulse;
	float time = 0.5f;

	public GameObject vial, text;
	private Material vialMaterial, fontMaterial;
	private int upDirID, wobbleXID, wobbleZID;
	public float WobbleMultiplierGyro, WobbleMultiplierPosition;
   
	// Use this for initialization
	void Start() {
		// Accessing the material through the renderer at start is better because it will use the instanced material,
		//  and so material values wont change for the sake of source control.
		vialMaterial = vial.GetComponent<Renderer>().materials[1];
		fontMaterial = text.GetComponent<Renderer>().materials[0];
		
		upDirID = Shader.PropertyToID("_UpDirection");
		wobbleXID = Shader.PropertyToID("_WobbleX");
		wobbleZID = Shader.PropertyToID("_WobbleZ");
	}
	
	private void Update() {
		time += Time.deltaTime;
		// decrease wobble over time
		wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
		wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);
 
		// make a sine wave of the decreasing wobble
		pulse = 2 * Mathf.PI * WobbleSpeed;
		wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
		wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
 
		// send it to the shader
		vialMaterial.SetFloat(wobbleXID, wobbleAmountX);
		vialMaterial.SetFloat(wobbleZID, wobbleAmountZ);
		// Send value to TMPro shader.
		fontMaterial.SetFloat("_FaceUVSpeedX", (wobbleAmountX + wobbleAmountZ) * .2f + .5f);
 
		// velocity
		Vector3 newRot = Shader.GetGlobalVector(upDirID);
		//Vector3 newRot = vialMaterial.GetVector(upDirID);
		velocity = ((lastPos - transform.position) / Time.deltaTime) * WobbleMultiplierPosition;
		angularVelocity = ((newRot - lastRot) / Time.deltaTime) * WobbleMultiplierGyro;
 
		// add clamped velocity to wobble
		wobbleAmountToAddX += Mathf.Clamp((velocity.x + angularVelocity.x * 0.2f) * MaxWobble, -MaxWobble, MaxWobble);
		wobbleAmountToAddZ += Mathf.Clamp((velocity.z + angularVelocity.z * 0.2f) * MaxWobble, -MaxWobble, MaxWobble);
 
		// keep last position
		lastPos = transform.position;
		lastRot = newRot; //transform.rotation.eulerAngles;
	}
}
