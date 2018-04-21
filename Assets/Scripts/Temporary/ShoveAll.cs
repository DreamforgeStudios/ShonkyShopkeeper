using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAll : MonoBehaviour {
    public List<Move> moveList;

    public bool move;

	// Use this for initialization
	void Start () {
        moveList.AddRange(transform.GetComponentsInChildren<Move>());
		
	}
	
	// Update is called once per frame
	void Update () {
        if (move) {
            foreach(Move move in moveList) {
                move.yes = true;
            }

            move = false;
        }
		
	}
}
