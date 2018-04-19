using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Barter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    public GameObject background;
    public Text txtPrice;
    public Text txtDebug;
    //private float prevPlayerOffer; // debug.

    private Rigidbody2D backgroundrb;

    public float dragMultiplier = 0.1f;
    public float dragVelocityMultiplier = 1.0f;
    public float priceChangeMultiplier = 0.5f;

    private float dx = 0f;
    private float fPrice = 100f;

    private float prevXPos = 0f;

    private bool wheelActive = true;

    public GameObject noDeal;
    public GameObject deal;
    public Button offerButton;
    public Button acceptButton;

    public Material mat;

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

    // Use this for initialization
    void Start () {
        this.backgroundrb = this.background.GetComponent<Rigidbody2D>();
        this.prevXPos = this.background.transform.position.x;

        this.currentMaxPrice = this.initialMaxPrice;
        this.currentOffer = this.initialOffer;
	}
	
    // Shove the slider back with a force (counteroffer).
    private void ShoveSlider(float force) {
        this.backgroundrb.AddForce(new Vector2(force, 0f));
    }

    // Consider the player's offer and offer a counter.  Returns true if bidding should continue, or false if not.
    // Counter offer is stored in "counter" parameter.  If the counter offer is 0, then the NPC is fed up.
    private bool CounterOffer(float offer, out float counter) {
        // Bidding should terminate if the current max price is lower than the amount the NPCs last offer.
        // AKA, bidding has gone on too long.
        if (this.currentMaxPrice < this.currentOffer) {
            counter = 0f;
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
            counter = offer;
            return false;
        } else if (rand <= rejectChance) {
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

            // Shove the slider to give some feedback on the offer.
            Debug.Log("Should shove the slider back " + (this.fPrice - this.currentOffer) + " points.");
            //ShoveSlider((offer - this.currentOffer) * shoveMultiplier);
            MoveSliderBack(this.fPrice - this.currentOffer);
            counter = this.currentOffer;

            this.acceptButton.interactable = true;
            return true;
        }

    }

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
        // NPC has taken the offer --
        } else if (!cont) {
            this.deal.SetActive(true);
            this.offerButton.interactable = false;
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
    }

    public void ResetButtons() {
        this.noDeal.SetActive(false);
        this.deal.SetActive(false);
        this.offerButton.interactable = true;
    }

    // SYSTEM / DRAGGING / SLIDER STUFF
    //*******************************//

    // Make it so that the player stops the wheel as soon as they tap.
    public void OnPointerDown(PointerEventData eventData) {
        if (!wheelActive) return;

        this.dx = 0f;
        this.backgroundrb.velocity = Vector2.zero;
        this.acceptButton.interactable = false;
    }

    // Might not need this anymore, superseeded by OnPointerDown().
    public void OnBeginDrag(PointerEventData eventData) {
        if (!wheelActive) return;

        this.dx = 0f;
        this.backgroundrb.velocity = Vector2.zero;
    }

    // On drag, move the wheel transform according to the mouse movement.
    public void OnDrag(PointerEventData eventData) {
        if (!wheelActive) return;

        this.dx = eventData.delta.x;
        this.background.transform.position += new Vector3(this.dx * this.dragMultiplier, 0f, 0f);
    }

    // When we finish a drag we want to "shove" the wheel in some way.  Do this based on the most recent drag delta.
    public void OnEndDrag(PointerEventData eventData) {
        if (!wheelActive) return;

        this.backgroundrb.velocity = Vector2.zero;
        this.backgroundrb.AddForce(new Vector2(this.dx * this.dragVelocityMultiplier, 0f));
    }

    private void MoveSliderBack(float val) {
        StartCoroutine(InterpolateSliderMove(0, val));
        //Vector3 move = new Vector3(val * this.priceChangeMultiplier, 0, 0);
        //this.backgroundrb.MovePosition(this.transform.position - move);
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
        this.acceptGradient = personality.acceptGradient;
        this.declineGradient = personality.acceptGradient;
        this.wantsItem = personality.wantsItem;
        this.initialMaxPrice = personality.initialMaxPrice;
        this.absoluteMaxPrice = personality.absoluteMaxPrice;
        this.initialOffer = personality.initialOffer;
        this.overflowStep = personality.overflowStep;
        this.absoluteOverflowStep = personality.absoluteOverflowStep;
        this.offerMultiplier = personality.offerMultiplier;
        this.fPrice = personality.initialOffer;
        this.currentOffer = personality.initialOffer;
        this.currentMaxPrice = personality.initialMaxPrice;
        UpdatePrice();
        UpdateDebug();
    }
    
    public void ResetSystem() {
        this.dx = 0f;
        this.fPrice = this.currentOffer;
        this.wheelActive = true;
        this.currentMaxPrice = this.initialMaxPrice;
        this.currentOffer = this.initialOffer;
        this.ResetButtons();
    }

    // Update is called once per frame
    // TODO, optimize this so we don't need to use update.
    void Update () {
        float xPos = this.background.transform.position.x;
        float xDiff = this.prevXPos - xPos;

        this.fPrice += xDiff * this.priceChangeMultiplier;
        UpdatePrice();
        UpdateDebug();

        this.prevXPos = xPos;
	}
}
