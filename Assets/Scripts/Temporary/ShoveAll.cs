using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAll : MonoBehaviour {
    public List<Move> moveList;

    public bool doMove;

	// Use this for initialization
	void Start () {
        moveList.AddRange(transform.GetComponentsInChildren<Move>());
		
	}
	
	// Update is called once per frame
	void Update () {
        if (doMove) {
            foreach(Move move in moveList) {
                move.yes = true;
            }

            doMove = false;
        }
		
	}
}
