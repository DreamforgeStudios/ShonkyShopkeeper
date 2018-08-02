using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Database", fileName = "ItemDatabase.asset")]
public class ItemDatabase : ScriptableObject {
	// Item objects.
	public Item Ruby;
	public Item Sapphire;
	public Item Emerald;
	public Item Amethyst;

	public Item CutRuby;
	public Item CutSapphire;
	public Item CutEmerald;
	public Item CutAmeythst;

	public Item ChargedRuby;
	public Item ChargedSapphire;
	public Item ChargedEmerald;
	public Item ChargedAmethyst;

	public Item Ore;
	public Item Brick;
	public Item Shell;
	public Item Pouch;
    public Item Empty;

    public Item RubyGolem1;
    public Item EmeraldGolem1;
    public Item SapphireGolem1;
	public Item AmethystGolem1;

    public Item GetActual(string name) {
	    if (string.IsNullOrEmpty(name)) {
		    //Debug.Log("GetActual(): name is null or empty.  You're either checking an empty slot or using this function incorrectly.");
		    return null;
	    }
	    
		switch (name.ToLower()) {
			case "ruby": return Ruby;
			case "sapphire": return Sapphire;
			case "emerald": return Emerald;
			case "amethyst": return Amethyst;
			case "cut ruby": return CutRuby;
			case "cut sapphire": return CutSapphire;
			case "cut emerald": return CutEmerald;
			case "cut amethyst": return CutAmeythst;
			case "charged ruby": return ChargedRuby;
			case "charged sapphire": return ChargedSapphire;
			case "charged emerald": return ChargedEmerald;
			case "charged amethyst": return ChargedAmethyst;
			case "ore": return Ore;
			case "brick": return Brick;
			case "shell": return Shell;
			case "pouch": return Pouch;
            case "empty": return Empty;
            case "emeraldgolem1": return EmeraldGolem1;
            case "rubygolem1": return RubyGolem1;
            case "sapphiregolem1": return SapphireGolem1;
            case "amethystgolem1": return AmethystGolem1;

            default: 
	            Debug.Log("Could not find an Item for key \"" + name + "\", is it typed correctly?");
	            return null;
		}
	}

    /* Commented out while fixing item & inventory
	public GameObject LookupItem(Item item) {
		foreach (ItemDict dict in itemDict) {
			if (item.itemType == dict.type && item.gemType == dict.gemType) {
				return dict.prefab;
			}
		}

		return null;
	}
    */
}
