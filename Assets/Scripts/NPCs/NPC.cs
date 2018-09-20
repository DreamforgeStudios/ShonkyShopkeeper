using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
	public GameObject WizardFront, WizardSide;

	public void ShowSide() {
		WizardSide.SetActive(true);
		WizardFront.SetActive(false);
	}

	public void ShowFront() {
		WizardSide.SetActive(false);
		WizardFront.SetActive(true);
	}

	public void FrontIdle() {
		Animator animator = WizardFront.transform.GetChild(0).GetComponent<Animator>();
		animator.SetBool("Idle", true);

		if (GameManager.Instance.BarterTutorial)
		{
			BarterTutorial.Instance.NextInstruction();
			BarterTutorial.Instance.StartShonkyParticles();
			GameManager.Instance.BarterNPC = false;
			GameManager.Instance.OfferNPC = true;
		}
	}

	public void Turn() {
		var angles = transform.eulerAngles;
		angles.y += 180f;
		transform.eulerAngles = angles;
	}
}
