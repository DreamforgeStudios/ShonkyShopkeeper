﻿using System.Collections;
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

	public Material VialMaterial;
	private int upDirID, wobbleXID, wobbleZID;
   
	// Use this for initialization
	void Start() {
		upDirID = Shader.PropertyToID("_UpDirection");
		wobbleXID = Shader.PropertyToID("_WobbleX");
		wobbleZID = Shader.PropertyToID("_WobbleZ");
	}
	
	private void Update() {
		time += Time.deltaTime;
		// decrease wobble over time
		wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
		wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
 
		// make a sine wave of the decreasing wobble
		pulse = 2 * Mathf.PI * WobbleSpeed;
		wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
		wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
 
		// send it to the shader
		VialMaterial.SetFloat(wobbleXID, wobbleAmountX);
		VialMaterial.SetFloat(wobbleZID, wobbleAmountZ);
 
		// velocity
		Vector3 newRot = VialMaterial.GetVector(upDirID);
		velocity = (lastPos - transform.position) / Time.deltaTime;
		angularVelocity = newRot - lastRot;//transform.rotation.eulerAngles - lastRot;
 
		// add clamped velocity to wobble
		wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
		wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
 
		// keep last position
		lastPos = transform.position;
		lastRot = newRot; //transform.rotation.eulerAngles;
	}
}
