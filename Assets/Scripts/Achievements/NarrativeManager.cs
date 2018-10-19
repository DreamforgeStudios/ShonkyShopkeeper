using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour {
    private static NarrativeDatabase
        narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));

    // Returns true if the element was locked, and will be unlocked.  False if the element was already unlocked our couldnt be found.
    public static bool Read(string key) {
        var narrative = GetNarrative(key);
        if (narrative == null) {
            return false;
        }

        bool unlocked = narrative.Unlocked;
        
        if (Initiate.IsLoading) {
            Initiate.onFinishFading += () => DoRead(narrative);
        } else {
            DoRead(narrative);
        }

        return unlocked;
    }

    // Returns true if successfully unlocked.  If false, already unlocked.
    private static bool DoRead(string key) {
        NarrativeElement e;
        if (narrativeDB.TryFindNarrativeWithKey(key, out e)) {
            //SFX.Play("Achieve_Popup", 1f, 1f, 0f, false, 0f);
            return narrativeDB.Unlock(e);
        }
        
        return false;
    }

    private static bool DoRead(NarrativeElement e) {
        return narrativeDB.Unlock(e);
    }

    private static NarrativeElement GetNarrative(string key) {
        NarrativeElement e;
        narrativeDB.TryFindNarrativeWithKey(key, out e);
        return e;
    }
}
