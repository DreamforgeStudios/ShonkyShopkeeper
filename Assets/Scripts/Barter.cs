using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Barter : MonoBehaviour {
    public GameObject noDeal;
    public GameObject deal;
    public Button offerButton;
    public Button acceptButton;

    public Material mat;

    public BarterComponentManager manager;


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
    public Personality personality;

    public ItemInstance shonky;

    void Awake() {
        if (DataTransfer.shonkyIndex >= 0) {
            ItemInstance tmp;
            if (ShonkyInventory.Instance.GetItem(DataTransfer.shonkyIndex, out tmp)) {
                shonky = tmp;
                manager.shonkyInstance = shonky;
            }
        } else {
            Debug.Log("No shonky found, using default value.");
            manager.SetBasePrice(150);
            manager.shonkyInstance = new ItemInstance(ScriptableObject.CreateInstance(typeof(Shonky)) as Item, 1, Quality.QualityGrade.Sturdy, false);
        }

        if (DataTransfer.currentPersonality) {
            this.personality = Instantiate(DataTransfer.currentPersonality);
            // TODO: messy code.
            manager.SetBasePrice((shonky.item as Shonky).basePrice);
            this.personality.InfluencePersonality(shonky.quality, (shonky.item as Shonky).basePrice);
            LoadPersonality();
        } else {
            Debug.Log("No personality found, using default values.");
            this.personality = Instantiate(this.personality);
            this.personality.InfluencePersonality(Quality.QualityGrade.Sturdy, 150);
            LoadPersonality();
        }
    }

    // Use this for initialization
    /*
    void Start () {
        //this.currentMaxPrice = this.initialMaxPrice;
        //this.currentOffer = this.initialOffer;
        if (DataTransfer.shonkyIndex >= 0) {
            ItemInstance tmp;
            if (ShonkyInventory.Instance.GetItem(DataTransfer.shonkyIndex, out tmp)) {
                shonky = tmp;
                manager.shonkyInstance = shonky;
            }
        } else {
            Debug.Log("No shonky found, using default value.");
            manager.SetBasePrice(150);
        }

        if (DataTransfer.currentPersonality) {
            this.personality = Instantiate(DataTransfer.currentPersonality);
            // TODO: messy code.
            manager.SetBasePrice((shonky.item as Shonky).basePrice);
            this.personality.InfluencePersonality(shonky.quality, (shonky.item as Shonky).basePrice);
            LoadPersonality();
        } else {
            Debug.Log("No personality found, using default values.");
            this.personality = Instantiate(this.personality);
            this.personality.InfluencePersonality(Quality.QualityGrade.Sturdy, 150);
            LoadPersonality();
        }
	}
    */

    // Consider the player's offer and offer a counter.  Returns true if bidding should continue, or false if not.
    // Counter offer is stored in "counter" parameter.  If the counter offer is 0, then the NPC is fed up.
    private bool CounterOffer(float offer, out float counter) {
        // Bidding should terminate if the current max price is lower than the amount the NPCs last offer.
        // AKA, bidding has gone on too long.
        if (this.currentMaxPrice < this.currentOffer) {
            counter = 0f;
            manager.WizardPunch(1f, 1.2f);
            manager.WizardSpeak(personality.TalkReject(1f));
            return false;
        }

        //Debug.Log("offer: " + offer);
        // Calculate chance of accepting the players offer.
        float acceptRange = this.currentMaxPrice - this.wantsItem;
        float acceptRatio = (offer - this.wantsItem) / acceptRange;
        float acceptLerp = Mathf.Clamp01(acceptRatio);
        float acceptChance = 1f - this.acceptGradient.Evaluate(acceptLerp);
        //Debug.Log("Evaluated an acceptance chance of: " + acceptChance*100f + "%");

        float rejectRange = this.absoluteMaxPrice - this.currentMaxPrice;
        float rejectRatio = (this.absoluteMaxPrice - offer) / rejectRange;
        float rejectLerp = Mathf.Clamp01(rejectRatio);
        float rejectChance = 1f - this.acceptGradient.Evaluate(rejectLerp);
        //Debug.Log("Evaluated a rejectance chance of: " + rejectChance*100f + "%");


        float rand = Random.value;
        // Chance is kind, we accept the offer.
        if (rand <= acceptChance) {
            // TODO: use variables to guide this.
            // TODO: use DenyOffer() or something like that?
            manager.WizardPunch(1.1f-acceptChance, 0.5f);
            manager.WizardSpeak(personality.TalkAccept(acceptChance));
            counter = offer;
            return false;
        } else if (rand <= rejectChance) {
            manager.WizardPunch(1-acceptChance, 1.2f);
            manager.WizardSpeak(personality.TalkReject(rejectChance));
            counter = 0f;
            return false;
        } else {
            // Approximate change based on ratio -- not very AI-like.
            float change = (offerMultiplier*acceptRatio);
            // Never exceed player's offer.
            if (this.currentOffer + change > offer) {
                change = (this.currentOffer / offer) * (offer - this.currentOffer);
            }

            //Debug.Log("Change (offer increasing by:): " + change);
            this.currentOffer += change;
            this.currentMaxPrice -= this.overflowStep;
            this.absoluteMaxPrice -= this.absoluteOverflowStep;

            manager.WizardPunch(change * 0.1f, 1f);
            manager.WizardSpeak(personality.TalkCounter(acceptChance));

            // Shove the slider to give some feedback on the offer.
            //Debug.Log("Should shove the slider back " + (manager.fPrice - this.currentOffer) + " points.");
            manager.MoveSliderBack(manager.fPrice - this.currentOffer);
            counter = this.currentOffer;

            this.acceptButton.interactable = true;
            return true;
        }
    }

    // Make an offer on the player's behalf.
    public void MakeOffer() {
        // Don't let the player move the wheel anymore.
        // TODO: make it obvious that the wheel is inactive? (A light on top of it?)
        //this.wheelActive = false;
        this.offerButton.interactable = false;
        this.acceptButton.interactable = false;

        float counter;
        float offer = manager.fPrice;
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
            Inventory.Instance.AddGold((int)offer);
            ShowUIButtons();
        // NPC has countered.
        } else {
            //while (this.backgroundrb.velocity.x != 0f)
                //kthis.offerButton.enabled = false;
            //this.wheelActive = true;
            this.offerButton.interactable = true;
            // ?? work is already done in CounterOffer().
        }

        return;
    }

    // Accept the NPCs offer.
    public void AcceptOffer() {
        //this.wheelActive = false;
        this.offerButton.interactable = false;

        this.deal.SetActive(true);
        manager.WizardPunch(0.1f, 0.5f);
        manager.WizardSpeak(personality.TalkAccept(1f));
        Inventory.Instance.AddGold((int)manager.fPrice);
        ShowUIButtons();
    }

    // SYSTEM / DRAGGING / SLIDER STUFF
    //*******************************//
    // Make it so that the player stops the wheel as soon as they tap.
    /*
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
    */

    public void LoadPersonality() {
        this.acceptGradient = personality.acceptGradient;
        this.declineGradient = personality.acceptGradient;
        this.wantsItem = personality.wantsItem;
        this.initialMaxPrice = personality.initialMaxPrice;
        this.absoluteMaxPrice = personality.absoluteMaxPrice;
        this.initialOffer = personality.initialOffer;
        this.overflowStep = personality.overflowStep;
        this.absoluteOverflowStep = personality.absoluteOverflowStep;
        this.currentOffer = personality.initialOffer;
        this.currentMaxPrice = personality.initialMaxPrice;
        manager.fPrice = personality.initialOffer;
    }

    private void ShowUIButtons() {
        next.SetActive(true);
        retry.SetActive(true);
    }
}
