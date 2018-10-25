using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyButtonManager : MonoBehaviour {
	public Button ButtonNext;

	public void DisableButtons() {
		ButtonNext.interactable = false;
	}
}
