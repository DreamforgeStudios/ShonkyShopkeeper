using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

[System.Serializable]
public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip> {}

[System.Serializable]
[CreateAssetMenu(menuName = "SFXDatabase", fileName = "SFXDatabase.asset")]
public class SoundDatabase : ScriptableObject {
    public AudioSource AudioSourcePrefab;
    [ShowNonSerializedField]
    public const int MaxAudioSourceInstances = 5;
    
    // To keep the scene neat...
    private GameObject audioParent = null;
    public GameObject AudioParent {
        get {
            if (audioParent == null) {
                audioParent = new GameObject("SFX");
            }

            return audioParent;
        }
    }

    private List<AudioSource> audioSourceInstances = new List<AudioSource>();
    public AudioSource AudioSourceInstance {
        get {
            AudioSource src = null;
            int size = audioSourceInstances.Count;
            for (int i = 0; i < size; i++) {
                if (!audioSourceInstances[i].isPlaying) {
                    src = audioSourceInstances[i];
                    break;
                }
            }

            if (src == null) {
                if (size < MaxAudioSourceInstances) {
                    src = Instantiate(AudioSourcePrefab, AudioParent.transform);
                    audioSourceInstances.Add(src);
                }
                else {
                    audioSourceInstances[0].Stop();
                    src = audioSourceInstances[0];
                }
            }

            return src;
        }
    }
    
    public StringAudioClipDictionary KeyedAudioClips;
    
    
    public AudioClip GetClip(string key) {
        AudioClip a;
        if (KeyedAudioClips.TryGetValue(key, out a)) {
            return a;
        }

        Debug.Log("Couldn't find an AudioClip matching key \"" + key + "\".");
        return null;
    }

    public void StopAll() {
        for (int i = 0; i < audioSourceInstances.Count; i++) {
            audioSourceInstances[i].Stop();
        }
    }

    public void StopSpecific(string sound) {
        var clip = GetClip(sound);
        for (int i = 0; i < audioSourceInstances.Count; i++) {
            if (audioSourceInstances[i].clip == clip && audioSourceInstances[i].isPlaying) {
                audioSourceInstances[i].Stop();
                break;
            }
        }
    }
    
    
    // Testing functions.
    public string ClipKey;
    
    [Button("PlayString")]
    private void PlayStringTest() {
        SFX.Play(ClipKey);
    }

    [Button("StopAll")]
    private void StopAllTest() {
        SFX.StopAll();
    }
    
    [Button("StopString")]
    private void StopSpecificTest() {
        SFX.StopSpecific(ClipKey);
    }
}
