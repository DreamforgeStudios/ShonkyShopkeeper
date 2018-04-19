using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Personality : MonoBehaviour {
    // The "accept-offer-chance"-scale.
    public AnimationCurve acceptGradient;
    // The "walk away"-scale.
    public AnimationCurve declineGradient;
    // The price that the NPC wants the item for.
    public float wantsItem;
    // The maximum price that the NPC is willing to pay (initially) -- goes down each turn.
    public float initialMaxPrice;
    // The absolute maximum before an NPC walks away.
    public float absoluteMaxPrice;
    // The initial offer that the NPC gives.
    public float initialOffer;
    // The amoutn that the overflow will decrease each turn.
    public float overflowStep;
    // The amoutn that the absolute overflow will decrease each turn.
    public float absoluteOverflowStep;
    // A test for now.
    public float offerMultiplier = 10f;

    public void InjectPersonality() {
        Debug.Log("click");
        GameObject.FindGameObjectWithTag("Barter").GetComponent<Barter>().LoadPersonality(this);
    }
}
