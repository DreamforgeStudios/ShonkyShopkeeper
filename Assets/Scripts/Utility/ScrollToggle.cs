using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollToggle : MonoBehaviour, IPointerClickHandler {
	public Sprite OpenSprite, ClosedSprite;
	public Image Image;

	public GameObject Heading, List;

	private bool open = true;

	private void OpenClose() {
		Debug.Log("Click");
		
		if (open) {
			open = false;
			Image.sprite = ClosedSprite;
			Heading.SetActive(false);
			List.SetActive(false);
		} else {
			open = true;
			Image.sprite = OpenSprite;
			Heading.SetActive(true);
			List.SetActive(true);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		OpenClose();
	}
}
