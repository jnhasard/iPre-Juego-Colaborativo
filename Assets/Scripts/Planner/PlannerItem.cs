using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerItem : MonoBehaviour {

	public string name;
	public string nameFeedback;
	public ItemType type;
	public PlannerPoi itemAt;
	public List<PlannerPlayer> itemAssign;

	private PlannerPlayer onInventory = null;
	
	public PlannerItem(){
		this.itemAssign = new List<PlannerPlayer> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		if (itemAt != null) {
			def.Add ("(item-at " + name + " " + itemAt.name + ")");
		}
		foreach (PlannerPlayer item in itemAssign) {
			def.Add("(item-assign " + name + " " + item.name + ")");
		}
		return def;
	}

	public void PickUp(PlannerPlayer playerObj){
		if (itemAt != null) {
			itemAt = null;
			playerObj.playerInventory.Add (this);
			onInventory = playerObj;
		}
	}

	public void Drop(PlannerPlayer playerObj, PlannerPoi poiObj){
		if (itemAt == null) {
			itemAt = poiObj;
			onInventory = null;
			for (int i = 0; i < playerObj.playerInventory.Count; i++) {
				if (this.name.Equals (playerObj.playerInventory [i].name)) {
					playerObj.playerInventory.RemoveAt (i);
					break;
				}
			}
		}
	}

	public void Use(){
		onInventory = null;
	}
}

public enum ItemType{
	item = 0,
	gear = 1,
	rune = 2
}