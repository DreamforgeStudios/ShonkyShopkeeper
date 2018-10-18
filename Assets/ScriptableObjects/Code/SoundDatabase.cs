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
    public AudioSourceHelper AudioSourcePrefab;
    [ShowNonSerializedField]
    public const int MAX_AUDIO_SOURCES = 10;
    
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

    private List<AudioSourceHelper> audioSourceInstances = new List<AudioSourceHelper>();
    public AudioSource AudioSourceInstance {
        get {
            AudioSourceHelper src = null;
            int size = audioSourceInstances.Count;
            for (int i = 0; i < size; i++) {
                if (!audioSourceInstances[i].source.isPlaying) {
                    src = audioSourceInstances[i];
                    break;
                }
            }

            if (src == null) {
                if (size < MAX_AUDIO_SOURCES) {
                    src = Instantiate(AudioSourcePrefab, AudioParent.transform);
                    audioSourceInstances.Add(src);
                }
                else {
                    audioSourceInstances[0].source.Stop();
                    src = audioSourceInstances[0];
                }
            }

            return src.source;
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
            audioSourceInstances[i].source.Stop();
        }
    }

    public void StopSpecific(string sound) {
        var clip = GetClip(sound);
        for (int i = 0; i < audioSourceInstances.Count; i++) {
            if (audioSourceInstances[i].source.clip == clip && audioSourceInstances[i].source.isPlaying) {
                audioSourceInstances[i].source.Stop();
                break;
            }
        }
    }

    public void RemoveSource(AudioSourceHelper source) {
        audioSourceInstances.Remove(source);
    }

    public void MuteAll()
    {
        for (int i = 0; i < audioSourceInstances.Count; i++)
        {
            audioSourceInstances[i].source.mute = true;
            audioSourceInstances[i].source.Pause();
        }
    }

    public void UnMuteAll()
    {
        for (int i = 0; i < audioSourceInstances.Count; i++)
        {
            audioSourceInstances[i].source.mute = false;
            audioSourceInstances[i].source.UnPause();
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
