using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Database", fileName = "ItemDatabase.asset")]
public class ItemDatabase : ScriptableObject {
	// Item objects.
	public Item ruby;
	public Item sapphire;
	public Item emerald;
	public Item diamond;

	public Item cutRuby;
	public Item cutSapphire;
	public Item cutEmerald;
	public Item cutDiamond;

	public Item chargedRuby;
	public Item chargedSapphire;
	public Item chargedEmerald;
	public Item chargedDiamond;

	public Item ore;
	public Item brick;
	public Item shell;
	public Item pouch;
    public Item empty;

    public Item rubyGolem1;
    public Item emeraldGolem1;
    public Item sapphireGolem1;

    public Item GetActual(string name) {
	    if (name == null) {
		    Debug.Log("GetActual(): name is null.  You're either checking an empty slot or using this function incorrectly.");
		    return null;
	    }
	    
		switch (name.ToLower()) {
			case "ruby": return ruby;
			case "sapphire": return sapphire;
			case "emerald": return emerald;
			case "diamond": return diamond;
			case "cut ruby": return cutRuby;
			case "cut sapphire": return cutSapphire;
			case "cut emerald": return cutEmerald;
			case "cut diamond": return cutDiamond;
			case "charged ruby": return chargedRuby;
			case "charged sapphire": return chargedSapphire;
			case "charged emerald": return chargedEmerald;
			case "charged diamond": return chargedDiamond;
			case "ore": return ore;
			case "brick": return brick;
			case "shell": return shell;
			case "pouch": return pouch;
            case "empty": return empty;
            case "emeraldgolem1": return emeraldGolem1;
            case "rubygolem1": return rubyGolem1;
            case "sapphiregolem1": return sapphireGolem1;

            default: return null;
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
