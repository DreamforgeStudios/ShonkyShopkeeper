using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue {
	// Trigger this dialogue when the given chance is higher than this one.
	public float chance;
	// The line to say.
	public string line;
}
