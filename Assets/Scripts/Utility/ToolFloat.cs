using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ToolFloat : MonoBehaviour
{
	private Sequence seq;
	private void Start()
	{
		CreateFloat();
	}

	public void StartFloat()
	{
		seq.Play();
	}

	public void EndFloat()
	{
		seq.Pause();
	}

	private void CreateFloat()
	{
		seq = DOTween.Sequence();
		seq.Append(transform.DOShakePosition(1f, 0.04f, 3, 120f, false, true).SetEase(Ease.OutQuart));
		seq.SetLoops(-1);
		seq.SetRecyclable(true);
		seq.Pause();
	}
}
