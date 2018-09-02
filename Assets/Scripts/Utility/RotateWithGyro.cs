using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RotateWithGyro : MonoBehaviour {
	// Sample size for accelerometer.
	private const int SAMPLE_SIZE = 15;
	
	public Material VialMaterial;
	
	private bool enableGyro, enableAccel;

	//public Text accel;

	// Box group these.
	[Range(-1, 1)]
	public float ManualX;
	[Range(-1, 1)]
	public float ManualY;
	[Range(-1, 1)]
	public float ManualZ;
	[Range(-1, 1)]
	public float ManualW;
	public float ZAdd;

	public float RotationMultiplier;


	private Quaternion originalRotation;
	private int updirID;
	
	// Use this for initialization
	void Start () {
		if (SystemInfo.supportsGyroscope) {
			enableGyro = true;
			enableAccel = false;
		} else if (SystemInfo.supportsAccelerometer) {
			enableGyro = false;
			enableAccel = true;
		} else {
			Debug.Log("Neither gyroscope or accelerometer is enabled.");
		}

		accelerations = new Vector3[SAMPLE_SIZE];
		accelerations[0] = Vector3.up;

		updirID = Shader.PropertyToID("_UpDirection");

		originalRotation = gameObject.transform.rotation;
	}
	
	private Vector3[] accelerations;

	private int counter = 0;
	void Update () {
		counter = ++counter % SAMPLE_SIZE;

		if (enableGyro) {
			VialMaterial.SetVector("_UpDirection", Input.gyro.gravity);
		} else if (enableAccel) {
			accelerations[counter] = Input.acceleration;

			Vector3 avg = Vector3.zero;
			foreach (var aval in accelerations) {
				avg += aval;
			}

			avg /= accelerations.Length;
			avg.z = avg.z + ZAdd;
			avg.y = -avg.y;
			avg.x = -avg.x;
			
			Vector4 rotationVec = avg * RotationMultiplier;
			rotationVec.z = 0;
			rotationVec.y = -rotationVec.y;
			
			gameObject.transform.rotation = originalRotation * Quaternion.Euler(-rotationVec);
			
			VialMaterial.SetVector(updirID, avg);
		} else {
			Vector4 vec = new Vector4(ManualX, ManualY, -ManualZ + ZAdd, ManualW);
			Vector4 rotationVec = vec * RotationMultiplier;
			rotationVec.z = 0;
			
			gameObject.transform.rotation = originalRotation * Quaternion.Euler(-rotationVec);
			VialMaterial.SetVector(updirID, vec);
		}
	}
}
