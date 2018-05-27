using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineAudio : MonoBehaviour {
    public AudioSource background;

    //Variables to randomise instrumental start position
    private float clipLength;

    //Main BGM
    public AudioClip instrumental;
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
