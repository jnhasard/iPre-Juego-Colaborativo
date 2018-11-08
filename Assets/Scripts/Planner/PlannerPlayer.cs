using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerPlayer : MonoBehaviour {

	public string name;
	public string nameFeedback;
	public PlayerType type;
	public bool luring;
	public PlannerPoi playerAt;
	public List<PlannerPlayer> playerDistinct;
	public List<PlannerItem> playerInventory;

	public PlannerPlayer(){
		this.playerInventory = new List<PlannerItem> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		def.Add("(player-at " + name + " " + playerAt.name + ")");
		if(luring && type == PlayerType.mage){
			def.Add("(luring " + name + ")");
		}
		foreach (PlannerPlayer item in playerDistinct) {
			def.Add("(player-distinct " + name + " " + item.name + ")");
		}
		foreach (PlannerItem item in playerInventory) {
			def.Add("(player-inventory " + name + " " + item.name + ")");
		}
		return def;
	}
}

public enum PlayerType{
	player = 0,
	mage = 1,
	warrior = 2,
	inventor = 3
}