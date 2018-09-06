using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class PhysicalAchievement : MonoBehaviour
{
	public AchievementDatabase achievementDB;
	private StringAchievementDictionary achievementDict;
	public List<GameObject> achievementBanners;

	private void Start()
	{
		achievementDict = achievementDB.AchievementDictionary;
		foreach (GameObject banner in achievementBanners)
		{
			banner.SetActive(false);
		}
		CheckForAchievements();
	}

	private void CheckForAchievements()
	{
		foreach (var kvp in achievementDict) {
			if (kvp.Value.Unlocked)
			{
				Debug.Log(kvp.Key + " is unlocked");
				DetermineIndexAndUnlockBanner(kvp);
			}
		}
	}

	private void DetermineIndexAndUnlockBanner(KeyValuePair<String,Achievement> kvp)
	{
		int index = achievementDict.Values.ToList().IndexOf(kvp.Value);
		Debug.Log("Index unlocked is " + index);
		achievementBanners[index].SetActive(true);
	}
}
