using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Barter : MonoBehaviour {//, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    public GameObject noDeal;
    public GameObject deal;
    public Button offerButton;
    public Button acceptButton;

    public Material mat;


    // for prototype.
    public GameObject next;
    public GameObject retry;

    // NPC / BARTER STUFF
    //*****************//

    // The "accept-offer-chance"-scale.
    public AnimationCurve acceptGradient;
    // The "walk away"-scale.
    public AnimationCurve declineGradient;
    // The price that the NPC wants the item for.
    public float wantsItem;
    // The maximum price that the NPC is willing to pay (initially) -- goes down each turn.
    public float initialMaxPrice;
    private float currentMaxPrice;
    // The absolute maximum before an NPC walks away.
    public float absoluteMaxPrice;
    // The initial offer that the NPC gives.
    public float initialOffer;
    private float currentOffer;
    // The amoutn that the overflow will decrease each turn.
    public float overflowStep;
    // The amoutn that the absolute overflow will decrease each turn.
    public float absoluteOverflowStep;
    // A test for now.
    public float offerMultiplier = 10f;
    // Multiply the force that NPCs shove the slider.  Would be nice to not have to hard code this, maybe make the wheel work more closely with values?
    public float shoveMultiplier = 1f;

    // Keep track of the NPC personality so that we can trigger events for them (shake, talk, etc).
    private Personality personality;

    // Use this for initialization
    void Start () {
        this.currentMaxPrice = this.initialMaxPrice;
        this.currentOffer = this.initialOffer;
	}
	
    // Shove the slider back with a force (counteroffer).
    private void ShoveSlider(float force) {
        //this.backgroundrb.AddForce(new Vector2(force, 0f));
    }

    /*
    // Consider the player's offer and offer a counter.  Returns true if bidding should continue, or false if not.
    // Counter offer is stored in "counter" parameter.  If the counter offer is 0, then the NPC is fed up.
    private bool CounterOffer(float offer, out float counter) {
        // Bidding should terminate if the current max price is lower than the amount the NPCs last offer.
        // AKA, bidding has gone on too long.
        if (this.currentMaxPrice < this.currentOffer) {
            counter = 0f;
            //personality.Shake(1f, 1.2f);
            personality.TalkReject(1f);
            return false;
        }


        // Calculate chance of accepting the players offer.
        float acceptRange = this.currentMaxPrice - this.wantsItem;
        float acceptRatio = (offer - this.wantsItem) / acceptRange;
        float acceptLerp = Mathf.Clamp01(acceptRatio);
        float acceptChance = 1f - this.acceptGradient.Evaluate(acceptLerp);
        Debug.Log("Evaluated an acceptance chance of: " + acceptChance*100f + "%");

        float rejectRange = this.absoluteMaxPrice - this.currentMaxPrice;
        float rejectRatio = (this.absoluteMaxPrice - offer) / rejectRange;
        float rejectLerp = Mathf.Clamp01(rejectRatio);
        float rejectChance = 1f - this.acceptGradient.Evaluate(rejectLerp);
        Debug.Log("Evaluated a rejectance chance of: " + rejectChance*100f + "%");


        float rand = Random.value;
        // Chance is kind, we accept the offer.
        if (rand <= acceptChance) {
            // TODO: use variables to guide this.
            // TODO: use DenyOffer() or something like that?
            //personality.Shake(1.1f-acceptChance, 0.5f);
            personality.TalkAccept(acceptChance);
            counter = offer;
            return false;
        } else if (rand <= rejectChance) {
            //personality.Shake(1-acceptChance, 1.2f);
            personality.TalkReject(rejectChance);
            counter = 0f;
            return false;
        } else {
            // Approximate change based on ratio -- not very AI-like.
            float change = (offerMultiplier*acceptRatio);
            // Never exceed player's offer.
            if (this.currentOffer + change > offer) {
                change = (this.currentOffer / offer) * (offer - this.currentOffer);
            }

            Debug.Log("Change (offer increasing by:): " + change);
            this.currentOffer += change;
            this.currentMaxPrice -= this.overflowStep;
            this.absoluteMaxPrice -= this.absoluteOverflowStep;

            //personality.Shake(change * 0.1f, 1f);
            personality.TalkCounter(acceptChance);

            // Shove the slider to give some feedback on the offer.
            Debug.Log("Should shove the slider back " + (this.fPrice - this.currentOffer) + " points.");
            //ShoveSlider((offer - this.currentOffer) * shoveMultiplier);
            MoveSliderBack(this.fPrice - this.currentOffer);
            counter = this.currentOffer;

            this.acceptButton.interactable = true;
            return true;
        }

    }
        */

    /*
    // Make an offer on the player's behalf.
    public void MakeOffer() {
        // Don't let the player move the wheel anymore.
        // TODO: make it obvious that the wheel is inactive? (A light on top of it?)
        this.wheelActive = false;
        this.offerButton.interactable = false;
        this.acceptButton.interactable = false;

        float counter;
        float offer = this.fPrice;
        bool cont = CounterOffer(offer, out counter);

        // NPC is fed up --
        if (!cont && counter == 0f) {
            this.noDeal.SetActive(true);
            this.offerButton.interactable = false;
            ShowUIButtons();
        // NPC has taken the offer --
        } else if (!cont) {
            this.deal.SetActive(true);
            this.offerButton.interactable = false;
            ShowUIButtons();
        // NPC has countered.
        } else {
            //while (this.backgroundrb.velocity.x != 0f)
                //kthis.offerButton.enabled = false;
            this.wheelActive = true;
            this.offerButton.interactable = true;
            // ?? work is already done in CounterOffer().
        }

        return;
    }

    // Accept the NPCs offer.
    public void AcceptOffer() {
        this.wheelActive = false;
        this.offerButton.interactable = false;

        this.deal.SetActive(true);
        //personality.Shake(0.1f, 0.5f);
        personality.TalkAccept(1f);
        ShowUIButtons();
    }
    */

    // SYSTEM / DRAGGING / SLIDER STUFF
    //*******************************//
    // Make it so that the player stops the wheel as soon as they tap.
    /*
    private void MoveSliderBack(float val) {
        StartCoroutine(InterpolateSliderMove(0, val));
    }

    IEnumerator InterpolateSliderMove(float from, float to) {
        float x = this.background.transform.position.x;
        for(float i= 0f; i < 1f; i += Time.deltaTime) {
            float movX = Mathf.SmoothStep(from, to, i) / this.priceChangeMultiplier;
            this.background.transform.position = new Vector3(x + movX, this.background.transform.position.y, this.background.transform.position.z);
            yield return null;
        }
    }

    // Update the price text box. 
    private void UpdatePrice() {
        string txt = ((int)this.fPrice).ToString();
        this.txtPrice.text = txt;
    }

    private void UpdateDebug() {
        string txt = string.Format("Max price: {0}", (int)this.currentMaxPrice);
        this.txtDebug.text = txt;
        float range = this.currentMaxPrice - this.wantsItem;
        float ratio = (this.fPrice - this.wantsItem)/range;
        float lerp = Mathf.Clamp01(ratio);
        float acceptChance = 1f - this.acceptGradient.Evaluate(lerp);

        this.txtDebug.color = Color.Lerp(Color.red, Color.green, acceptChance);
        if (acceptChance <= 0) {
            float rejectRange = this.absoluteMaxPrice - this.currentMaxPrice;
            float rejectRatio = (this.absoluteMaxPrice - this.fPrice) / rejectRange;
            float rejectLerp = Mathf.Clamp01(rejectRatio);
            float rejectChance = 1f - this.acceptGradient.Evaluate(rejectLerp);
            this.txtDebug.color = Color.Lerp(Color.red, Color.blue, rejectChance);

        }
    }

    public void LoadPersonality(Personality personality) {
        Debug.Log("loading new personality");
        this.personality = personality;
        this.acceptGradient = personality.acceptGradient;
        this.declineGradient = personality.acceptGradient;
        this.wantsItem = personality.wantsItem;
        this.initialMaxPrice = personality.initialMaxPrice;
        this.absoluteMaxPrice = personality.absoluteMaxPrice;
        this.initialOffer = personality.initialOffer;
        this.overflowStep = personality.overflowStep;
        this.absoluteOverflowStep = personality.absoluteOverflowStep;
        //this.offerMultiplier = personality.offerMultiplier;
        this.fPrice = personality.initialOffer;
        this.currentOffer = personality.initialOffer;
        this.currentMaxPrice = personality.initialMaxPrice;
        UpdatePrice();
        UpdateDebug();
    }

    private void ShowUIButtons() {
        next.SetActive(true);
        retry.SetActive(true);
    }
    
    // Update is called once per frame
    void Update () {
        float xPos = this.background.transform.position.x;
        float xDiff = this.prevXPos - xPos;

        this.fPrice += xDiff * this.priceChangeMultiplier;
        UpdatePrice();
        UpdateDebug();

        this.prevXPos = xPos;
	}
    */
}
