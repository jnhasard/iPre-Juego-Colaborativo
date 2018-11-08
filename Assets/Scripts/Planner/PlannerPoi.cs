using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerPoi : MonoBehaviour {

	public string name;
	public string nameFeedback;
	public PoiType type;
	public List<PlannerPoi> routeTo;
	public List<PlannerPoi> routeBlockPoi;
	public List<PlannerObstacle> routeBlockObstacle;
	public List<PlannerPoi> doorRoutePoi;
	public List<PlannerObstacle> doorRouteObstacle;

	public PlannerObstacle araña = null;

	public PlannerPoi(){
		this.routeTo = new List<PlannerPoi> ();
		this.routeBlockPoi = new List<PlannerPoi> ();
		this.routeBlockObstacle = new List<PlannerObstacle> ();
		this.doorRoutePoi = new List<PlannerPoi> ();
		this.doorRouteObstacle = new List<PlannerObstacle> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		foreach (PlannerPoi item in routeTo) {
			def.Add("(route-to " + name + " " + item.name + ")");
		}
		for (int i = 0; i < routeBlockPoi.Count; i++) {
			PlannerPoi itemOut = routeBlockPoi [i];
			PlannerObstacle itemIn = routeBlockObstacle [i];
			def.Add("(route-block " + name + " " + itemOut.name + " " + itemIn.name + ")");
		}
		for (int i = 0; i < doorRoutePoi.Count; i++) {
			PlannerPoi itemOut = doorRoutePoi [i];
			PlannerObstacle itemIn = doorRouteObstacle [i];
			def.Add ("(door-route " + name + " " + itemOut.name + " " + itemIn.name + ")");
		}
		return def;
	}
}

public enum PoiType{
	poi = 0
}