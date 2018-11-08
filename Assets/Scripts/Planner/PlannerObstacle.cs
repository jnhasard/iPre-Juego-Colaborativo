using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerObstacle : MonoBehaviour {

	public string name;
	public string nameFeedback;
	public ObstacleType type;
	public List<PlannerPoi> enemyAt;
	public List<PlannerPoi> enemyEdgeStart;
	public List<PlannerPoi> enemyEdgeEnd;
	public bool blocked;
	public bool open;
	public List<PlannerItem> doorRune;
	public bool rollableLocked;
	public bool rollableOpen;

	public PlannerObstacle(){
		this.enemyAt = new List<PlannerPoi> ();
		this.enemyEdgeStart = new List<PlannerPoi> ();
		this.enemyEdgeEnd = new List<PlannerPoi> ();
		this.doorRune = new List<PlannerItem> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		if (type == ObstacleType.enemy) {
			foreach (PlannerPoi item in enemyAt) {
				def.Add ("(enemy-at " + name + " " + item.name + ")");
			}
			for (int i = 0; i < enemyEdgeStart.Count; i++) {
				PlannerPoi edgeStart = enemyEdgeStart [i];
				PlannerPoi edgeEnd = enemyEdgeEnd [i];
				def.Add ("(enemy-edge " + name + " " + edgeStart.name + " " + edgeEnd.name + ")");
			}
		}
		if (blocked) {
			def.Add("(blocked " + name + ")");
		}
		if (open) {
			def.Add("(open " + name + ")");
		}
		if(type == ObstacleType.door){
			foreach (PlannerItem item in doorRune) {
				if(item.type == ItemType.rune){
					def.Add("(door-rune " + name + " " + item.name + ")");
				}
			}
		}
		if (type == ObstacleType.rollable) {
			if (rollableLocked) {
				def.Add ("(rollable-locked " + name + ")");
			}
			if (rollableOpen) {
				def.Add ("(rollable-open " + name + ")");
			}
		}
		return def;
	}

	public void OpenDoor(){
		if (this.type == ObstacleType.door) {
			blocked = false;
			open = true;
			foreach (PlannerItem itemObj in this.doorRune) {
				itemObj.Use ();
			}

		}
	}
}

public enum ObstacleType{
	obstacle = 0,
	rollable = 1,
	door = 2,
	jump = 3,
	enemy = 4,
	barrier = 5
}