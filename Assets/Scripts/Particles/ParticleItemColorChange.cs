using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleItemColorChange : MonoBehaviour {
    public List<ParticleSystem> SystemsToChange;
    
    public void SetColor(Color color) {
        for (int i = 0; i < SystemsToChange.Count; i++) {
            var main = SystemsToChange[i].main;
            main.startColor = color;
        }
    }
}
