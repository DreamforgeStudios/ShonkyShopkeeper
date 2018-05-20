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

    public Item shonky;

	public Item GetActual(string name) {
		switch (name) {
			case "Ruby": return ruby;
			case "Sapphire": return sapphire;
			case "Emerald": return emerald;
			case "Diamond": return diamond;
			case "Cut Ruby": return cutRuby;
			case "Cut Sapphire": return cutSapphire;
			case "Cut Emerald": return cutEmerald;
			case "Cut Diamond": return cutDiamond;
			case "Charged Ruby": return chargedRuby;
			case "Charged Sapphire": return chargedSapphire;
			case "Charged Emerald": return chargedEmerald;
			case "Charged Diamond": return chargedDiamond;
			case "Ore": return ore;
			case "Brick": return brick;
			case "Shell": return shell;
			case "Pouch": return pouch;
            case "Empty": return empty;
            case "Shonky": return shonky;
			default: return ruby;
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
