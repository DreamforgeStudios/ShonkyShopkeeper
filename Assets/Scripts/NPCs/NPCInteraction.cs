using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class NPCInteraction : MonoBehaviour {
    //Variables related to NPC interaction
    private int numberOfPlayerShonkys;
    //public float baseChanceOfApproach = 30.0f;
    //private int numberOfInteractionsLately = 0;
    //public int maxInteractions = 5;
    public Vector3 shopFrontPos;

    public bool EnableDebug;

    //Variables related to numberOfInteractions
    //private float nextResetTime;
    public float cooldown = 10.0f;


    //private GameObject[] penShonkys;
    //System.Random generator;

    // Use this for initialization
    void Start() {
        //penShonkys = GameObject.FindGameObjectsWithTag("Shonky");
        //generator = new System.Random();
        //nextResetTime = Time.time + cooldown;
        //shopFrontPos = shopFront.transform.position;
    }

    // Update is called once per frame
    void Update() {
        CheckForInput();
        //RefreshInteractions();

    }
    /*
    private void RefreshInteractions() {
        if (Time.time > nextResetTime) {
            nextResetTime = Time.time + cooldown;
            numberOfInteractionsLately -= 1;
        }
    }
    */

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0) && !GameManager.pickedUpGolem) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Camera.main.ScreenPointToRay(Input.mousePosition));
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5f);
            if (Physics.Raycast(ray, out hit, 100)) {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.CompareTag("NPC")) {
                    //Need to determine if the player has any shonkys and what indexes they are at
                    List<int> shonkyIndexes = ShonkyInventory.Instance.PopulatedShonkySlots();
                    //If they do, select a random one and pass to the barter screen
                    Debug.Log(shonkyIndexes.Count);
                    if (shonkyIndexes.Count > 0) {
                        GameManager.Instance.WizardFrontTransfer = hit.transform.GetComponent<NPCWalker>().WizardFront;
                        //GameManager.Instance.PersonalityTransfer = hit.transform.GetComponent<NPCWalker>().personality;

                        int randomShonky = shonkyIndexes[UnityEngine.Random.Range(0, shonkyIndexes.Count)];
                        ItemInstance chosenShonky;
                        if (ShonkyInventory.Instance.GetItem(randomShonky, out chosenShonky)) {
                            GameManager.Instance.ShonkyIndexTransfer = randomShonky;

                            if (GameManager.Instance.BarterTutorial) {
                                GameManager.Instance.BarterTutorial = false;
                                PlayerPrefs.SetInt("TutorialDone", 1);
                            }
                            //Move NPC to shop front and initiate barter
                            hit.transform.GetComponent<NPCWalker>().ShowFront();
                            hit.transform.DOScale(1.2f, 2f);
                            hit.transform.DOMove(shopFrontPos, 2f, false).OnComplete(() => SceneManager.LoadScene("Barter"));
                            //SFX.Play("sound");
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if (EnableDebug)
            Gizmos.DrawWireSphere(shopFrontPos, 1f);
    }
}
