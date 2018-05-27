using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAudioManager : MonoBehaviour {
    //One for BGM and other for SFX
    public AudioSource background;
    public AudioSource effects;

    //Main BGM
    public AudioClip instrumental;

    //Variables to randomise instrumental start position
    private float clipLength;

    //SFX
    public AudioClip cursorSelect;
    public AudioClip golumCreated;
    public AudioClip itemLift;
    public AudioClip itemDown;

	// Use this for initialization
	void Start () {
        clipLength = instrumental.length;
        float startPos = UnityEngine.Random.Range(0, clipLength);
        background.clip = instrumental;
        background.time = startPos;
        background.Play();
        background.loop = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
