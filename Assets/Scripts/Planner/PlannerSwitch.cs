using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerSwitch : MonoBehaviour {

	public string name;
	public string nameFeedback;
	public SwitchType type;
	public PlannerPoi switchAt;
	public List<PlannerObstacle> linkedSwitch;
	public List<PlannerItem> machineGear;
	public bool machineLoaded;
	public bool switchOn;
	public List<PlannerPlayer> switchAssign;

	public PlannerSwitch(){
		this.linkedSwitch = new List<PlannerObstacle> ();
		this.machineGear = new List<PlannerItem> ();
		this.switchAssign = new List<PlannerPlayer> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		string typeSwitch = "";
		if (type == SwitchType.init) {
			typeSwitch = "switch";
		} else {
			typeSwitch = type.ToString ();
		}
		message += name + " - " + typeSwitch;
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		def.Add("(switch-at " + name + " " + switchAt.name + ")");
		foreach (PlannerObstacle item in linkedSwitch) {
			def.Add("(linked-switch " + name + " " + item.name + ")");
		}
		if(type == SwitchType.machine){
			foreach (PlannerItem item in machineGear) {
				if(item.type == ItemType.gear){
					def.Add("(machine-gear " + name + " " + item.name + ")");
				}
			}
		}
		if (machineLoaded && type == SwitchType.machine) {
			def.Add("(machine-loaded " + name + ")");
		}
		if (switchOn) {
			def.Add("(switch-on " + name + ")");
		}
		foreach (PlannerPlayer item in switchAssign) {
			def.Add("(switch-assign " + name + " " + item.name + ")");
		}
		return def;
	}

	public void ActivateSwitch(){
		switchOn = true;
		if (type == SwitchType.machine) {
			foreach (PlannerObstacle obstacle in linkedSwitch) {
				obstacle.rollableLocked = false;
				obstacle.rollableOpen = true;
			}
		}
		else {
			foreach (PlannerObstacle obstacle in linkedSwitch) {
				obstacle.blocked = false;
				obstacle.open = true;
			}
		}
	}

	public void DeactivateSwitch(){
		switchOn = false;
		if (type == SwitchType.lever) {
			foreach (PlannerObstacle obstacle in linkedSwitch) {
				obstacle.blocked = true;
				obstacle.open = false;
			}
		}
	}
}

public enum SwitchType{
	init = 0,
	step = 1,
	lever = 2,
	machine = 3,
	triple = 4,
	doble = 5
}