using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class PlayerSelectController : PseudoScene {
	public int MinPlayers, MaxPlayers;
	public int DefaultPlayers;
	
	[ReadOnly]
	public int ActiveNumberOfPlayers;
	
	public TextMeshProUGUI Text;

	public void LessPlayers() {
		if (ActiveNumberOfPlayers <= MinPlayers) {
			return;
		}

		ActiveNumberOfPlayers--;
		UpdateText();
	}
	
	public void MorePlayers() {
		if (ActiveNumberOfPlayers >= MaxPlayers) {
			return;
		}

		ActiveNumberOfPlayers++;
		UpdateText();
	}

	private void UpdateText() {
		Text.text = ActiveNumberOfPlayers.ToString();
	}

	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);
		
		ActiveNumberOfPlayers = DefaultPlayers;
		UpdateText();

		return t;
	}
}
