using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// TODO: don't destroy  on load?
public static class SFX {
    private static SoundDatabase
        soundDB = Object.Instantiate((SoundDatabase) Resources.Load("SFXDatabase"));

    public static void Prewarm() {
        // Forces the soundDB object to be instantiated.
        //  Makes it so the scene doesn't pause while waiting for SFXDatabase to load.
        soundDB.GetInstanceID();
    }
    
    public static AudioSource Play(string sound, float volume = 1, float pitch = 1, float delay = 0, bool looping = false, float playaAt = 0) {
            var clip = soundDB.GetClip(sound);

            var source = soundDB.AudioSourceInstance;
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.time = playaAt;
            source.loop = looping;

            source.PlayDelayed(delay);
        
        if (!GameManager.Instance.PlayingAudio)
            MuteAll();

        return source;
    }

    public static void StopAll() {
        soundDB.StopAll();
    }

    public static void StopSpecific(string sound) {
        soundDB.StopSpecific(sound);
    }

    public static void RemoveSource(AudioSourceHelper source) {
        soundDB.RemoveSource(source);
    }

    public static void MuteAll() {
        soundDB.MuteAll();
    }

    public static void UnMuteAll() {
        soundDB.UnMuteAll();
    }
}
