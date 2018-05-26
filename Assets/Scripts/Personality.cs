using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[CreateAssetMenu(menuName = "Personality", fileName = "PersonalityName.asset")]
public class Personality : ScriptableObject {
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

    // Dialogue box for the NPC to talk with.
    //public TextMeshProUGUI dialogueText;

    [Header("Important!  These lists should be ORDERED.  Higher chance items come first.")]
    public List<Dialogue> acceptLines;
    public List<Dialogue> counterLines;
    public List<Dialogue> rejectLines;

    public string TalkAccept(float chance) {
        // This relies on the list being sorted.
        // Could move this to its own function but its not worth it at the moment.
        foreach(Dialogue line in acceptLines) {
            if (chance >= line.chance) {
                return line.line;
            }
        }

        return "...";
    }

    public string TalkCounter(float chance) {
        // This relies on the list being sorted.
        foreach(Dialogue line in counterLines) {
            if (chance >= line.chance) {
                return line.line;
            }
        }

        return "...";
    }

    public string TalkReject(float chance) {
        // This relies on the list being sorted.
        foreach(Dialogue line in rejectLines) {
            if (chance >= line.chance) {
                return line.line;
            }
        }

        return "...";
    }

    public void InjectPersonality() {
        //GameObject.FindGameObjectWithTag("Barter").GetComponent<Barter>().LoadPersonality(this);
    }

    // Change this to be dependent on shonky type + shonky grade.
    public void InfluencePersonality(Quality.QualityGrade grade) {
        // TODO: don't hardcode this.
        switch (grade) {
            case Quality.QualityGrade.Junk: 
                MultiplyPersonality(0.1f);
                break;
            case Quality.QualityGrade.Brittle: 
                MultiplyPersonality(0.75f);
                break;
            case Quality.QualityGrade.Passable: 
                MultiplyPersonality(1.0f);
                break;
            case Quality.QualityGrade.Sturdy: 
                MultiplyPersonality(1.15f);
                break;
            case Quality.QualityGrade.Magical:
                MultiplyPersonality(1.3f);
                break;
            case Quality.QualityGrade.Mystic:
                MultiplyPersonality(1.5f);
                break;
            default:
                break;
        }
    }

    private void MultiplyPersonality(float multiplier) {
        wantsItem *= multiplier;
        initialMaxPrice *= multiplier;
        absoluteMaxPrice *= multiplier;
        initialOffer *= multiplier;
    }
}
