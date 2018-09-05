using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour {
    private static NarrativeDatabase
        narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));

    public static void Read(string key) {
        // Queue up displays AFTER the scene has loaded.  Avoids things randomly popping up and being jarring while the
        // wipe is playing.  We lose the boolean return value here.  Bummer
        if (Initiate.IsFading) {
            Initiate.onFinishFading += () => DoRead(key);
        } else {
            DoRead(key);
        }
    }

    private static bool DoRead(string key) {
        // If true, successfully unlocked.  If false, already unlocked.
        NarrativeElement e;
        if (narrativeDB.TryFindNarrativeWithKey(key, out e)) {
            //SFX.Play("Achieve_Popup", 1f, 1f, 0f, false, 0f);
            return narrativeDB.Unlock(e);
        }
        
        return false;
    }
}
