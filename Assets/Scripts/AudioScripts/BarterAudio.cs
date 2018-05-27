using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterAudio : MonoBehaviour {
    public AudioSource background;
    public AudioSource effects;

    //Variables to randomise instrumental start position
    private float clipLength;

    //Main BGM
    public AudioClip instrumental;
    public AudioClip coins;

    // Use this for initialization
    void Start() {
        clipLength = instrumental.length;
        float startPos = UnityEngine.Random.Range(0, clipLength);
        background.clip = instrumental;
        background.time = startPos;
        background.Play();
        background.loop = true;
    }

    // Update is called once per frame
    void Update() {

    }
}
