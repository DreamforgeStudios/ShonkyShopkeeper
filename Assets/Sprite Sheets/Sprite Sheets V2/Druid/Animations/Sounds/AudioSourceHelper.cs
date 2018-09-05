using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceHelper : MonoBehaviour {
    public AudioSource source;

    private void OnDestroy() {
        SFX.RemoveSource(this);
    }
}
