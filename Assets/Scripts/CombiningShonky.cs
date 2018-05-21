using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class CombiningShonky {
    public static List<GameObject> shonkys;
    //Selection holders
    public GameObject newShonky;
    public GameObject pen;
    public Vector3 penSpawnPosition = new Vector3(1,-1.68f,-4.6f);
    private bool instantiated = false;


    public bool InitialiseList() {
        if (!instantiated) {
            shonkys = new List<GameObject>();
            instantiated = true;
            return true;
        } else {
            return false;
        }
    }

    public void AddNewShonky (GameObject newShonky){
        shonkys.Add(newShonky);
        //newShonky.AddComponent<ShonkyWander>();
        //newShonky.AddComponent<NavMeshAgent>();
        Transform t1 = newShonky.transform;
        t1.DOMove(penSpawnPosition, 0.5f).SetEase(Ease.InCubic).OnComplete(() => Debug.Log("In Pen"));
    }
}
