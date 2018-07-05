using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcess : MonoBehaviour {

	public Shader shader;
	public Texture vignette;
	public float intensity;

	private Material mat;
	
	// Use this for initialization
	void Start () {
		mat = new Material(shader);
		mat.SetTexture("_Vignette", vignette);
	}

	private void Update() {
		mat.SetFloat("_Intensity", intensity);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, mat);
	}
}
