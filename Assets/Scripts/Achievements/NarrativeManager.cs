using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour {
    private static NarrativeDatabase
        narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));

    public static bool Read(string key) {
        // If true, successfully unlocked.  If false, already unlocked.
        NarrativeElement e;
        if (narrativeDB.TryFindNarrativeWithKey(key, out e)) {
            //SFX.Play("Achieve_Popup", 1f, 1f, 0f, false, 0f);
            return narrativeDB.Unlock(a);
        }
        
        return false;
    }
}
