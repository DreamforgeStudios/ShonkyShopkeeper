using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcePouch : ResourcePouchLoot {

    private MineVeinType type;

    //Materials to represent unlocked towns
    public Material unlocked;
    public Material locked;

    //Variables to show numbers while inventory is not properly implemented
    public int oreDropped;
    public int gemsDropped;

    //UI Text
    public TextMeshProUGUI helperText;

    private void Start() {
        //helperText.enabled = false;
        //this.GetComponent<Renderer>().material = unlocked;
        //FindVeinType();
    }

    private void Update() {
        //Debug.Log("This is of type: " + type + " and has ore drop % of " + orePercentage + " and gem % of " + gemPercentage);
        //CheckForInput();
    }

    private void CheckForInput() {
        /*
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 30)) {
                if (hit.transform.gameObject == this.gameObject) {
                    this.GetComponent<Renderer>().material = locked;
                    oreDropped = CalculateOreDrop(type, orePercentage);
                    gemsDropped = CalculateGemDrop(oreDropped, numberOfItems);
                    helperText.enabled = true;
                    helperText.text = "Ore Dropped: " + oreDropped + "\n Gems Dropped: " + gemsDropped;

                }
            }
        }
        */
    }

   // private void FindVeinType
        /*) {
        switch (gameObject.tag) {
            case "DiamondBag":
                CalculateDropRates(MineVeinType.Diamond);
                type = MineVeinType.Diamond;
                break;
            case "EmeraldBag":
                CalculateDropRates(MineVeinType.Emerald);
                type = MineVeinType.Emerald;
                break;
            case "RubyBag":
                CalculateDropRates(MineVeinType.Ruby);
                type = MineVeinType.Ruby;
                break;
            case "SapphireBag":
                CalculateDropRates(MineVeinType.Sapphire);
                type = MineVeinType.Sapphire;
                break;
        }
    */
    }
    

