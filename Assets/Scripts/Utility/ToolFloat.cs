using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ToolFloat : MonoBehaviour
{
	private Sequence seq;
	public ParticleSystem WandParticleSystem;
	public ParticleSystem ItemParticleSystem;
	
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

	public void WandParticles(Vector3 wandPosition, Vector3 itemPosition)
	{
		WandParticleSystem.transform.position = wandPosition;
		WandParticleSystem.Play();
		ItemParticleSystem.transform.position = itemPosition;
		ItemParticleSystem.Play();
	}
}
