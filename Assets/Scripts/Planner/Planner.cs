using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour {

	public bool control;
	public int level;
	public List<PlannerPlayer> playerList;
	public List<PlannerPoi> poiList;
	public List<PlannerObstacle> obstacleList;
	public List<PlannerItem> itemList;
	public List<PlannerSwitch> switchList;

	private string message;
	private List<string> objDef;
	private List<string> initDef;
	public List<string> goalDef = new List<string> ();
	private Dictionary<string, string> feedbackNames = new Dictionary<string, string> ();
	private List<string> EstadoInicial;
	//Plan completo
	private List<string> Plan;
	//Plan por jugador
	private Dictionary<string,List<string>> PlanParcial;
	//Estado por accion
	private List<List<string>> EstadoPorAccion;
	//Estado por accion
	private Dictionary<string,List<List<string>>> EstadoPorAccionParcial;
	//Estado por accion pop
	private List<List<List<string>>> EstadoPorAccionPop;
	//Mapeo de acciones pop
	private List<List<int>> AccionPop;

	private int tipoPlanificacion = 3;

	private int distanciaObjetiva = -1;
	private Dictionary<string, int> distanciaObjetivaParcial = new Dictionary<string, int> ();
	private double timer;
	private int feedbackLevel = 0;
	private int feedbackPlayer = 0;
	private int etapaCumplida = 0;
	private Dictionary<string, int> etapaCumplidaParcial = new Dictionary<string, int> ();
	private string lastAction = "";
	private Dictionary<string,int> nivelPop = new Dictionary<string, int>();
	private Dictionary<string,int> etapaPop = new Dictionary<string, int>();

	//POP
	//Accion por precondicion
	private Dictionary<string, List<string>> actionPerPrecondition;
	private Dictionary<string, List<string>> constraintsPerAction;
	private Dictionary<string, List<string>> reverseConstraintsPerAction;

	// Use this for initialization
	void Start () {
		message = "";
		objDef = new List<string> ();
		initDef = new List<string> ();
		EstadoInicial = new List<string> ();
		Plan = new List<string>();
		PlanParcial = new Dictionary<string, List<string>> ();
		EstadoPorAccion = new List<List<string>> ();
		EstadoPorAccionParcial = new Dictionary<string, List<List<string>>> ();
		EstadoPorAccionPop = new List<List<List<string>>> ();
		AccionPop = new List<List<int>> ();

		foreach (PlannerPlayer item in playerList) {
			distanciaObjetivaParcial.Add (item.name, -1);
			etapaCumplidaParcial.Add (item.name, 0);
			nivelPop.Add (item.name, 999);
			etapaPop.Add (item.name, 0);
		}

		foreach (PlannerPlayer item in playerList) {
			feedbackNames.Add (item.name, item.nameFeedback);
		}
		foreach (PlannerPoi item in poiList) {
			feedbackNames.Add (item.name, item.nameFeedback);
		}
		foreach (PlannerObstacle item in obstacleList) {
			feedbackNames.Add (item.name, item.nameFeedback);
		}
		foreach (PlannerItem item in itemList) {
			feedbackNames.Add (item.name, item.nameFeedback);
		}
		foreach (PlannerSwitch item in switchList) {
			feedbackNames.Add (item.name, item.nameFeedback);
		}
	}

	void Update(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			bool valido = false;
			if (Plan.Count > 0) {
				valido = true;
			}
			foreach (string personaje in PlanParcial.Keys.ToList()) {
				if (PlanParcial [personaje].Count > 0) {
					valido = true;
				}
			}
			if (valido) {
				timer += Time.deltaTime;
			}
			if (tipoPlanificacion == 3) {
				distanciaObjetiva = 0;
				foreach (string personaje in distanciaObjetivaParcial.Keys.ToList()) {
					distanciaObjetiva += distanciaObjetivaParcial [personaje];
				}
			}

			int tope = 130 - 2 * distanciaObjetiva;
			
			if (timer > tope) {
				Debug.Log ("Feedback: " + feedbackPlayer);
				timer = 0;
				if (tipoPlanificacion == 3) {
					string personajeFeedback = "";
					foreach (string personaje in this.etapaCumplidaParcial.Keys.ToList()) {
						if (PlanParcial [personaje] [etapaCumplidaParcial [personaje]].Equals (lastAction)) {
							personajeFeedback = personaje;
						}
					}
					if (personajeFeedback.Equals ("")) {
						int minimo = 0;
						foreach (string personaje in this.etapaCumplidaParcial.Keys.ToList()) {
							bool dependant = false;
							if(reverseConstraintsPerAction.ContainsKey(PlanParcial[personaje] [etapaCumplidaParcial[personaje]])){
								List<string> conditions = reverseConstraintsPerAction [PlanParcial [personaje] [etapaCumplidaParcial [personaje]]];
								foreach (string action in conditions) {
									if (!PlanParcial [personaje].Contains (action)) {
										dependant = true;
									}
								}
							}
							if (!dependant) {
								if (personajeFeedback.Equals ("")) {
									minimo = this.etapaCumplidaParcial [personaje];
									personajeFeedback = personaje;
								}
								if (minimo > this.etapaCumplidaParcial [personaje]) {
									minimo = this.etapaCumplidaParcial [personaje];
									personajeFeedback = personaje;
								}
							}
						}
					}
					//Falta seleccionar la accion a aplicar feedback
					if (!personajeFeedback.Equals ("")) {
						string action = PlanParcial [personajeFeedback] [etapaCumplidaParcial [personajeFeedback]];
						if (action.Substring (0, 1).Equals ("x")) {
							Debug.Log (personajeFeedback+"test5: " + PlanParcial [personajeFeedback] [etapaCumplidaParcial [personajeFeedback] + 1]);
							this.RequestActivateNPCLog (GetFeedback (PlanParcial [personajeFeedback] [etapaCumplidaParcial [personajeFeedback] + 1]));
						} else {
							this.RequestActivateNPCLog (GetFeedback (action));
						}
					}
				} else if (tipoPlanificacion == 4) {
					string player = "";
					int nivel = 0;
					foreach (string personaje in nivelPop.Keys.ToList()) {
						if (nivelPop [personaje] != 999 && nivelPop [personaje] > nivel) {
							nivel = nivelPop [personaje];
							player = personaje;
						}
					}
					if (nivel != 0) {
						this.RequestActivateNPCLog (GetFeedback (Plan [AccionPop [nivel] [etapaPop [player]]]));
					}
				} else {
					if (etapaCumplida != Plan.Count) {
						this.RequestActivateNPCLog (GetFeedback (Plan [etapaCumplida]));
					}
				}
			}
		}
	}

	//Metodo de replanificacion, toma el estado actual y lo envía al servidor.
	public void Replanificar(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			Debug.Log ("Inicio replanificacion");
			//send message (estado actual) al server para planificar
			EstadoInicial = new List<string> (initDef);
			Client.instance.SendMessageToPlanner (this.message);
		}
	}
	//Metodo de recepcion del plan
	public void SetPlanFromServer(string message){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			Debug.Log ("Recibido mensaje Server");
			Debug.Log (message);
			List<string> parameters = new List<string> (message.Split ('/'));
			this.Plan = new List<string> (parameters);
			etapaCumplida = 0;
			foreach (PlannerPlayer item in playerList) {
				if (!etapaCumplidaParcial.ContainsKey (item.name)) {
					etapaCumplidaParcial.Add (item.name, 0);
				}
				etapaCumplidaParcial [item.name] = 0;
			}
			if (tipoPlanificacion == 3) {
				foreach (string personaje in this.PlanParcial.Keys.ToList()) {
					if (!(this.PlanParcial [personaje].Count > 0 && this.PlanParcial [personaje].First ().Equals (lastAction))) {
						feedbackLevel = 0;
					}
				}
			} else {
				if (!(this.Plan.Count > 0 && this.Plan.First ().Equals (lastAction))) {
					feedbackLevel = 0;
				}
			}
			distanciaObjetiva = this.Plan.Count;
			Debug.Log ("distancia objetiva: " + distanciaObjetiva);
			switch (tipoPlanificacion) {
			case 1:
				GetEstadosDesdePlanEstandar ();
				break;
			case 2:
				GetEstadosDesdePlanRegresion ();
				break;
			case 3:
				Pop ();
				SplitPlan ();
				GetEstadosDesdePlanParcialRegresion ();
				//GetEstadosDesdePlanPOP ();
				break;
			}
		}
	}
	//Metodo de calculo estado por accion Estandar
	void GetEstadosDesdePlanEstandar(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> estadoActual = new List<string> (EstadoInicial);
			this.EstadoPorAccion = new List<List<string>> ();
			this.EstadoPorAccion.Add (new List<string> (estadoActual));
			foreach (string action in this.Plan) {
				estadoActual = new List<string> (GetEstadoPorAccionEstandar (estadoActual, action));
				this.EstadoPorAccion.Add (new List<string> (estadoActual));
			}
		}
	}
	//Metodo de calculo estado por accion Regresion
	void GetEstadosDesdePlanRegresion(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> estadoActual = new List<string> (goalDef);
			this.EstadoPorAccion = new List<List<string>> ();
			this.EstadoPorAccion.Add (new List<string> (estadoActual));
			foreach (string action in this.Plan.Reverse<string>().ToList()) {
				estadoActual = new List<string> (GetEstadoPorAccionRegresion (estadoActual, action));
				this.EstadoPorAccion.Add (new List<string> (estadoActual));
			}
		}
	}
	//Metodo de calculo estado por accion Regresion
	void GetEstadosDesdePlanPOP(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			Debug.Log ("plan pop start");
			List<string> estadoActual = new List<string> (goalDef);
			this.EstadoPorAccionPop = new List<List<List<string>>> ();
			this.EstadoPorAccionPop.Add (new List<List<string>> ());
			this.EstadoPorAccionPop[0].Add (new List<string> (estadoActual));
			List<List<string>> acciones = new List<List<string>> ();
			acciones.Add (new List<string> (Plan));
			List<Dictionary<string,List<string>>> ordenes = new List<Dictionary<string, List<string>>> ();
			ordenes.Add (constraintsPerAction);
			this.GetEstadoPorAccionPop (EstadoPorAccionPop [0], acciones, ordenes);
			Debug.Log ("plan pop end");
		}
	}
	//Metodo de escaneo, revisa el estado actual y lo guarda en las variables correspondientes
	void Escaneo(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			objDef = new List<string> ();
			initDef = new List<string> ();
			message = level + "/";
			foreach (PlannerPlayer player in playerList) {
				objDef.Add (player.GetDefinitionObjects ());
			}
			foreach (PlannerPoi poi in poiList) {
				objDef.Add (poi.GetDefinitionObjects ());
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				objDef.Add (obstacle.GetDefinitionObjects ());
			}
			foreach (PlannerItem item in itemList) {
				objDef.Add (item.GetDefinitionObjects ());
			}
			foreach (PlannerSwitch switchItem in switchList) {
				objDef.Add (switchItem.GetDefinitionObjects ());
			}
			foreach (string item in objDef) {
				message += item + " ";
			}
			message += "/";

			foreach (PlannerPlayer player in playerList) {
				initDef.AddRange (player.GetDefinitionInit ());
			}
			foreach (PlannerPoi poi in poiList) {
				initDef.AddRange (poi.GetDefinitionInit ());
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				initDef.AddRange (obstacle.GetDefinitionInit ());
			}
			foreach (PlannerItem item in itemList) {
				initDef.AddRange (item.GetDefinitionInit ());
			}
			foreach (PlannerSwitch switchItem in switchList) {
				initDef.AddRange (switchItem.GetDefinitionInit ());
			}

			foreach (string item in initDef) {
				message += item;
			}

			message += "/";

			foreach (string goal in goalDef) {
				message += goal;
			}
		}
	}
	//Metodo monitor (con cada cambio de accion se llama y decide si sse debe replanificar o no
	public void Monitor(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			bool cumple = false;
			Escaneo ();
			Debug.Log ("comienzo de monitoreo");
			if (this.Plan.Count > 0) {
				switch (tipoPlanificacion) {
				case 1:
					for (int i = 0; i <= this.Plan.Count; i++) {
						List<string> estadoActual = new List<string> (EstadoPorAccion [i]);
						if (estadoActual.Count == initDef.Count && initDef.All (estadoActual.Contains)) {
							Debug.Log ("cumple estado:" + i);
							if (etapaCumplida < i) {
								Debug.Log ("cambio estado");
								etapaCumplida = i;
								feedbackLevel = 0;
							}
							distanciaObjetiva = this.Plan.Count - i;
							Debug.Log ("distancia objetiva: " + distanciaObjetiva);
							cumple = true;
							break;
						}
					}
					break;
				case 2:
					for (int i = 0; i <= this.Plan.Count; i++) {
						List<string> estadoActual = new List<string> (EstadoPorAccion [i]);
						if (estadoActual.All (initDef.Contains)) {
							int etapa = this.Plan.Count - i;
							Debug.Log ("cumple estado:" + etapa);
							if (etapaCumplida < etapa) {
								Debug.Log ("cambio estado");
								etapaCumplida = etapa;
								feedbackLevel = 0;
							}
							distanciaObjetiva = i;
							Debug.Log ("distancia objetiva: " + distanciaObjetiva);
							cumple = true;
							break;
						}
					}
					break;
				case 3:
					Dictionary<string,bool> cumplePorPersonaje = new Dictionary<string, bool> ();
					foreach (string personaje in this.PlanParcial.Keys.ToList()) {
						if (!cumplePorPersonaje.ContainsKey (personaje)) {
							cumplePorPersonaje.Add (personaje, false);
						}
						for (int i = 0; i <= this.PlanParcial [personaje].Count; i++) {	
							List<string> estadoActual = new List<string> (EstadoPorAccionParcial [personaje] [i]);
							if (estadoActual.All (initDef.Contains)) {
								int etapa = this.PlanParcial [personaje].Count - i;
								Debug.Log (personaje + " cumple estado:" + etapa);
								if (etapaCumplidaParcial [personaje] < etapa) {
									Debug.Log (personaje + " cambio estado");
									etapaCumplidaParcial [personaje] = etapa;
									feedbackLevel = 0;
								}
								distanciaObjetivaParcial [personaje] = i;
								Debug.Log (personaje + " distancia objetiva: " + distanciaObjetivaParcial [personaje]);
								cumplePorPersonaje [personaje] = true;
								break;
							}
						}
					}
					cumple = true;
					foreach (string personaje in cumplePorPersonaje.Keys.ToList()) {
						if (!cumplePorPersonaje [personaje]) {
							cumple = false;
						}
					}
					break;
				case 4:
					Dictionary<string,string> etapasAuxiliares = new Dictionary<string, string> ();
					etapasAuxiliares.Add ("pj1", "");
					etapasAuxiliares.Add ("pj2", "");
					etapasAuxiliares.Add ("pj3", "");
					cumple = true;
					for (int i = 0; i < this.EstadoPorAccionPop.Count; i++) {
						bool aux = true;
						List<List<string>> estadosPorNivel = EstadoPorAccionPop [i];
						for (int j = 0; i < estadosPorNivel.Count; j++) {
							List<string> estadoActual = new List<string> (EstadoPorAccionPop [i] [j]);
							if (estadoActual.All (initDef.Contains)) {
								AgregarAccionPop (i, j, etapasAuxiliares);
								aux = true;
								foreach (string personaje in etapasAuxiliares.Keys.ToList()) {
									if (etapasAuxiliares [personaje].Equals ("")) {
										aux = false;
									}
								}
								if (aux) {
									break;
								}
							}
						}
						aux = true;
						foreach (string personaje in etapasAuxiliares.Keys.ToList()) {
							if (etapasAuxiliares [personaje].Equals ("")) {
								aux = false;
							}
						}
						if (aux) {
							break;
						}
					}
					foreach (string personaje in etapasAuxiliares.Keys.ToList()) {
						if (etapasAuxiliares [personaje].Equals ("")) {
							cumple = false;
						}
					}
					break;
				}
			}
			if (!cumple) {
				Debug.Log ("no cumple estados");
				this.Replanificar ();
			}
		}
	}
	//Mapeo estado por accion, guarda el estado para la accion i del plan segun metodo estandar
	List<string> GetEstadoPorAccionEstandar(List<string> estadoActual, string accion){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				foreach (PlannerObstacle enemy in obstacleList) {
					if (enemy.type == ObstacleType.enemy) {
						if (estadoActual.Contains ("(enemy-edge " + enemy.name + " " + parametros [1] + " " + parametros [2] + ")") && estadoActual.Contains ("(luring " + parametros [0] + ")")) {
							estadoActual.Add ("(blocked " + enemy.name + ")");
							estadoActual.Remove ("(open " + enemy.name + ")");
							estadoActual.Remove ("(luring " + parametros [0] + ")");
						}
					}
				}
				break;
			case "move-jump":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "move-through":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "move-distract":
				estadoActual.Remove ("(blocked " + parametros [3] + ")");
				estadoActual.Add ("(open " + parametros [3] + ")");
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(luring " + parametros [0] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "lever-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
						estadoActual.Add ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "lever-off":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && estadoActual.Contains ("open " + obstacle.name + "")) {
						estadoActual.Add ("(blocked " + obstacle.name + ")");
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "machine-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable) {
						if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && estadoActual.Contains ("(rollable-locked " + obstacle.name + ")")) {
							estadoActual.Remove ("(rollable-locked " + obstacle.name + ")");
							estadoActual.Add ("(rollable-open " + obstacle.name + ")");
						}
					}
				}
				break;
			case "item-pick":
				estadoActual.Remove ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				break;
			case "item-drop":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				break;
			case "rune-use":
				foreach (PlannerPoi poi in poiList) {
					if (estadoActual.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && estadoActual.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && estadoActual.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						estadoActual.Remove ("(blocked " + parametros [3] + ")");
						estadoActual.Add ("(open " + parametros [3] + ")");
						estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
					}
				}
				break;
			case "gear-use":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(machine-loaded " + parametros [3] + ")");
				break;
			case "step-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
						estadoActual.Add ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
						estadoActual.Add ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "doble-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
						estadoActual.Add ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "push-boulder":
				estadoActual.Remove ("(blocked " + parametros [1] + ")");
				estadoActual.Add ("(open " + parametros [1] + ")");
				break;
			}
			return estadoActual;
		}
		return null;
	}

	//Mapeo estado por accion, guarda el estado para la accion i del plan segun metodo regresivo
	List<string> GetEstadoPorAccionRegresion(List<string> estadoActual, string accion){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				foreach (PlannerObstacle enemy in obstacleList) {
					if (enemy.type == ObstacleType.enemy) {
						if (initDef.Contains ("(enemy-edge " + enemy.name + " " + parametros [1] + " " + parametros [2] + ")") && !estadoActual.Contains ("(luring " + parametros [0] + ")") && !estadoActual.Contains ("(open " + enemy.name + ")")) {
							estadoActual.Remove ("(blocked " + enemy.name + ")");
						}
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(open " + obstacle.name + ")")) {
							estadoActual.Add ("(open " + obstacle.name + ")");
						}
					}
					if (obstacle.type == ObstacleType.enemy) {
						if (initDef.Contains ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")") && !estadoActual.Contains ("(luring " + parametros [0] + ")") && !estadoActual.Contains ("(open " + obstacle.name + ")")) {
							if (!estadoActual.Contains ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")")) {
								estadoActual.Add ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")");
							}
							estadoActual.Add ("(luring " + parametros [0] + ")");
							estadoActual.Add ("(open " + obstacle.name + ")");
						}
					}
				}
				break;
			case "move-jump":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				break;
			case "move-through":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				break;
			case "move-distract":
				estadoActual.Remove ("(open " + parametros [3] + ")");
				estadoActual.Remove ("(luring " + parametros [0] + ")");
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(enemy-at " + parametros [3] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(enemy-at " + parametros [3] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				break;
			case "lever-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "lever-off":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(open " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(switch-on " + parametros [1] + ")")) {
					estadoActual.Add ("(switch-on " + parametros [1] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(open " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "machine-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable) {
						if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(rollable-locked " + obstacle.name + ")")) {
							estadoActual.Remove ("(rollable-open " + obstacle.name + ")");
						}
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(machine-loaded " + parametros [1] + ")")) {
					estadoActual.Add ("(machine-loaded " + parametros [1] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable) {
						if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(rollable-locked " + obstacle.name + ")")) {
							if (!estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
								estadoActual.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")");
							}
							estadoActual.Add ("(rollable-locked " + obstacle.name + ")");
						}
					}
				}
				break;
			case "item-pick":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(item-at " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				}
				break;
			case "item-drop":
				estadoActual.Remove ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				}
				break;
			case "rune-use":
				foreach (PlannerPoi poi in poiList) {
					if (initDef.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && initDef.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && initDef.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						estadoActual.Remove ("(open " + parametros [3] + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(item-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(item-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(door-rune " + parametros [3] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(door-rune " + parametros [3] + " " + parametros [1] + ")");
				}
				foreach (PlannerPoi poi in poiList) {
					if (initDef.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && initDef.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && initDef.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						if (!estadoActual.Contains ("(route-to " + parametros [2] + " " + poi.name + ")")) {
							estadoActual.Add ("(route-to " + parametros [2] + " " + poi.name + ")");
						}
						if (!estadoActual.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
							estadoActual.Add ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")");
						}
						if (!estadoActual.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
							estadoActual.Add ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")");
						}
					}
				}
				break;
			case "gear-use":
				estadoActual.Remove ("(machine-loaded " + parametros [3] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [3] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [3] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(item-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(item-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(machine-gear " + parametros [3] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(machine-gear " + parametros [3] + " " + parametros [1] + ")");
				}
				break;
			case "step-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(player-at " + parametros [1] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [1] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(player-at " + parametros [2] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [2] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [3] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [3] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [2] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [3] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "doble-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(player-at " + parametros [1] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(player-at " + parametros [1] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [2] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [2] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [2] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [2] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [2] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "push-boulder":
				estadoActual.Remove ("(open " + parametros [1] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-to " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [1] + ")")) {
					estadoActual.Add ("(blocked " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(rollable-open " + parametros [1] + ")")) {
					estadoActual.Add ("(rollable-open " + parametros [1] + ")");
				}
				break;
			}
			return estadoActual;
		}
		return null;
	}

	//Mapeo estado por accion, guarda el estado para la accion i del plan segun metodo regresivo parcial
	List<string> GetEstadoPorAccionRegresionParcial(string personaje, List<string> estadoActual, string accion){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				foreach (PlannerObstacle enemy in obstacleList) {
					if (enemy.type == ObstacleType.enemy) {
						if (initDef.Contains ("(enemy-edge " + enemy.name + " " + parametros [1] + " " + parametros [2] + ")") && !estadoActual.Contains ("(luring " + parametros [0] + ")") && !estadoActual.Contains ("(open " + enemy.name + ")")) {
							estadoActual.Remove ("(blocked " + enemy.name + ")");
						}
					}
				}
				break;
			case "move-jump":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "move-through":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "move-distract":
				estadoActual.Remove ("(open " + parametros [3] + ")");
				estadoActual.Remove ("(luring " + parametros [0] + ")");
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "lever-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "lever-off":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(open " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "machine-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable) {
						if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(rollable-locked " + obstacle.name + ")")) {
							estadoActual.Remove ("(rollable-open " + obstacle.name + ")");
						}
					}
				}
				break;
			case "item-pick":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				break;
			case "item-drop":
				estadoActual.Remove ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				break;
			case "rune-use":
				foreach (PlannerPoi poi in poiList) {
					if (initDef.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && initDef.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && initDef.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						estadoActual.Remove ("(open " + parametros [3] + ")");
					}
				}
				break;
			case "gear-use":
				estadoActual.Remove ("(machine-loaded " + parametros [3] + ")");
				break;
			case "step-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "doble-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				break;
			case "push-boulder":
				estadoActual.Remove ("(open " + parametros [1] + ")");
				break;
			}
			return estadoActual;
		}
		return null;
	}

	//Mapeo estado por accion, guarda el estado para la accion i del plan segun metodo regresivo parcial
	List<string> GetEstadoPorAccionRegresionMultiple(string personaje, List<string> estadoActual, string accion){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (parametros[0].Equals(personaje) && !estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [4] + ")");
				}
				if (parametros[1].Equals(personaje) && !estadoActual.Contains ("(player-at " + parametros [1] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [1] + " " + parametros [4] + ")");
				}
				if (parametros[2].Equals(personaje) && !estadoActual.Contains ("(player-at " + parametros [2] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(player-at " + parametros [2] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [3] + " " + parametros [4] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [3] + " " + parametros [4] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [3] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [3] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [2] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [3] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "doble-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(open " + obstacle.name + ")");
					}
				}
				if (parametros[0].Equals(personaje) && !estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [3] + ")");
				}
				if (parametros[1].Equals(personaje) && !estadoActual.Contains ("(player-at " + parametros [1] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(player-at " + parametros [1] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(switch-at " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(switch-at " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [2] + " " + parametros [0] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [2] + " " + parametros [0] + ")");
				}
				if (!estadoActual.Contains ("(switch-assign " + parametros [2] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(switch-assign " + parametros [2] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")") && !estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						if (!estadoActual.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")")) {
							estadoActual.Add ("(linked-switch " + parametros [2] + " " + obstacle.name + ")");
						}
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			}
			return estadoActual;
		}
		return null;
	}

	string GetFeedback(string accion){
		string feedback = "";
		List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
		parametros.RemoveAt (0);
		string nombre = parametros [0];
		parametros.RemoveAt (0);
		if (feedbackLevel == 0) {
			int targetPlayer = int.Parse (parametros [0].Substring (2)) - 1;
			feedbackPlayer = (targetPlayer + Random.Range (1, 2)) % 3;
			feedback = "Aun no han llegado, ¿Está todo bien?";
		} else {
			switch (nombre) {
			case "move":
				switch (feedbackLevel) {
				//case 1:
					//feedback = "¿Creo que no han pasado por aqui aún?";
					//break;
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe ir desde " + feedbackNames [parametros [1]] + " hasta " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "move-jump":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "No olviden sus habilidades...";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "¿Probaron usar la habilidad especial de " + feedbackNames [parametros [0]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe usar su habilidad especial para llegar hasta " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "move-through":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe ir desde " + feedbackNames [parametros [1]] + " hasta " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "move-distract":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 3:
					feedback = "No olviden sus habilidades...";
					break;
				case 4:
					feedback = "¿Probaron usar la habilidad especial de " + feedbackNames [parametros [0]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe usar su habilidad especial para llegar hasta " + feedbackNames [parametros [3]];
					break;
				}
				break;
			case "lever-on":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Al parecer falta activar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "¿Han visto " + feedbackNames [parametros [1]] + "?";
					break;
				case 5:
					feedback = "Solo " + feedbackNames [parametros [0]] + " puede usar " + feedbackNames [parametros [1]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe activar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "lever-off":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Al parecer falta desactivar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "¿Han visto " + feedbackNames [parametros [1]] + "?";
					break;
				case 5:
					feedback = "Solo " + feedbackNames [parametros [0]] + " puede usar " + feedbackNames [parametros [1]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe desactivar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "machine-on":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Al parecer falta activar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "¿Han visto " + feedbackNames [parametros [1]] + "?";
					break;
				case 5:
					feedback = "Solo " + feedbackNames [parametros [0]] + " puede usar " + feedbackNames [parametros [1]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe activar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "item-pick":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Creo que se les olvido recoger algo...";
					break;
				case 3:
					feedback = "Necesitan encontrar " + feedbackNames [parametros [1]];
					break;
				case 4:
					feedback = "Creo que vi algo en " + feedbackNames [parametros [2]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe tomar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "item-drop":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Creo que tienen muchos items...";
					break;
				case 3:
					feedback = "Quizas otro debería probar " + feedbackNames [parametros [1]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe dejar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "rune-use":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Ya revisaron " + feedbackNames [parametros [4]] + "?";
					break;
				case 3:
					feedback = "Necesitan tener " + feedbackNames [parametros [1]] + " para avanzar";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe usar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [4]];
					break;
				}
				break;
			case "gear-use":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Ya revisaron " + feedbackNames [parametros [4]] + "?";
					break;
				case 3:
					feedback = "Necesitan tener " + feedbackNames [parametros [1]] + " para avanzar";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe usar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [4]];
					break;
				}
				break;
			case "step-on":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + feedbackNames [parametros [0]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Al parecer falta activar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "Han visto " + feedbackNames [parametros [1]] + "?";
					break;
				case 5:
					feedback = "Solo " + feedbackNames [parametros [0]] + " puede usar " + feedbackNames [parametros [1]];
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe activar " + feedbackNames [parametros [1]] + " en " + feedbackNames [parametros [2]];
					break;
				}
				break;
			case "triple-switch":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba verlos a los tres por aquí, ¿pasa algo?";
					break;
				case 2:
					feedback = "Al parecer falta activar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [4]] + "?";
					break;
				case 4:
					feedback = "Han visto " + feedbackNames [parametros [3]] + "?";
					break;
				default:
					feedback = "Deben activar " + feedbackNames [parametros [3]] + " en " + feedbackNames [parametros [4]];
					break;
				}
				break;
			case "doble-switch":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba verl a " + feedbackNames [parametros [0]] + " y " + feedbackNames [parametros [1]] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "Al parecer falta activar algo";
					break;
				case 3:
					feedback = "¿Probaron ir hast " + feedbackNames [parametros [3]] + "?";
					break;
				case 4:
					feedback = "Han visto " + feedbackNames [parametros [2]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " y " + feedbackNames [parametros [1]] + " deben activar " + feedbackNames [parametros [2]] + " en " + feedbackNames [parametros [3]];
					break;
				}
				break;
			case "push-boulder":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a" + feedbackNames [parametros [0]] + " por aquí, ¿pasa algo?";
					break;
                case 2:
					feedback = "No olviden sus habilidades...";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + feedbackNames [parametros [2]] + "?";
					break;
				case 4:
					feedback = "¿Probaron usar la habilidad especial de " + feedbackNames [parametros [0]] + "?";
					break;
				default:
					feedback = feedbackNames [parametros [0]] + " debe usar su habilidad especial en " + feedbackNames [parametros [1]];
					break;
				}
				break;
			}
		}
		feedbackLevel++;
		lastAction = accion;
		return feedback;
	}

	void RequestActivateNPCLog(string feedbackMessage)
	{
		Client.instance.SendMessageToServer ("ActivateNPCLog/" + feedbackMessage + "/" + feedbackPlayer, true);
	}

	public void FirstPlan(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			Escaneo ();
			Replanificar ();
		}
	}

	private void Pop(){
		Debug.Log ("Pop start");
		actionPerPrecondition = new Dictionary<string, List<string>> ();
		constraintsPerAction = new Dictionary<string, List<string>> ();
		reverseConstraintsPerAction = new Dictionary<string, List<string>> ();
		foreach (string goal in goalDef) {
			actionPerPrecondition.Add (goal, new List<string>());
			actionPerPrecondition [goal].Add ("");
 		}
		//Revisar para cada accion su punto en el sistema
		List<string> pool = new List<string> (Plan.Reverse<string> ().ToList ());
		for (int i = 0; i < pool.Count; i++) {
			string action = pool [i];
			PopConstraint (action);
		}
		Debug.Log ("Pop end");
		foreach (string action in constraintsPerAction.Keys.ToList()) {
			foreach (string constAction in constraintsPerAction[action]) {
				if (!reverseConstraintsPerAction.ContainsKey (constAction)) {
					reverseConstraintsPerAction.Add (constAction, new List<string> ());
				}
				reverseConstraintsPerAction [constAction].Add (action);
			}
		}
	}

	private void PopConstraint(string accion){
		List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
		parametros.RemoveAt (0);
		string nombre = parametros [0];
		parametros.RemoveAt (0);
		bool add = false;
		switch (nombre) {
		case "move":
			if (actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
				string cond = "(player-at " + parametros [0] + " " + parametros [2] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			foreach (PlannerObstacle enemy in obstacleList) {
				if (enemy.type == ObstacleType.enemy) {
					if (parametros [0].Equals ("pj1") && initDef.Contains ("(enemy-edge " + enemy.name + " " + parametros [1] + " " + parametros [2] + ")")) {
						if (actionPerPrecondition.ContainsKey ("(blocked " + enemy.name + ")")) {
							string cond = "(blocked " + enemy.name + ")";
							foreach (string actCond in actionPerPrecondition [cond]) {
								if (!actCond.Equals ("")) {
									if (!constraintsPerAction.ContainsKey (accion)) {
										constraintsPerAction.Add (accion, new List<string> ());
									}
									if (!constraintsPerAction [accion].Contains (actCond)) {
										constraintsPerAction [accion].Add (actCond);
									}
								}
							}
							//actionPerPrecondition.Remove (cond);
							add = true;
						}
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(open " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(open " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(open " + obstacle.name + ")"].Add (accion);
						}
					}
					if (obstacle.type == ObstacleType.enemy) {
						if (initDef.Contains ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")")) {
							if (!actionPerPrecondition.ContainsKey ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")")) {
								actionPerPrecondition.Add ("(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
							}
							if (!actionPerPrecondition ["(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
								actionPerPrecondition ["(enemy-edge " + obstacle.name + " " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
							}

							if (!actionPerPrecondition.ContainsKey ("(luring " + parametros [0] + ")")) {
								actionPerPrecondition.Add ("(luring " + parametros [0] + ")", new List<string> ());
							}
							if (!actionPerPrecondition ["(luring " + parametros [0] + ")"].Contains (accion)) {
								actionPerPrecondition ["(luring " + parametros [0] + ")"].Add (accion);
							}

							if (!actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
								actionPerPrecondition.Add ("(open " + obstacle.name + ")", new List<string> ());
							}
							if (!actionPerPrecondition ["(open " + obstacle.name + ")"].Contains (accion)) {
								actionPerPrecondition ["(open " + obstacle.name + ")"].Add (accion);
							}
						}
					}
				}
			}
			break;
		case "move-jump":
			if (actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
				string cond = "(player-at " + parametros [0] + " " + parametros [2] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(blocked " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(blocked " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(blocked " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(blocked " + parametros [3] + ")"].Add (accion);
				}
			}
			break;
		case "move-through":
			if (actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
				string cond = "(player-at " + parametros [0] + " " + parametros [2] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(blocked " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(blocked " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(blocked " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(blocked " + parametros [3] + ")"].Add (accion);
				}
			}
			break;
		case "move-distract":
			if (actionPerPrecondition.ContainsKey ("(open " + parametros [3] + ")")) {
				string cond = "(open " + parametros [3] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (actionPerPrecondition.ContainsKey ("(luring " + parametros [0] + ")")) {
				string cond = "(luring " + parametros [0] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
				string cond = "(player-at " + parametros [0] + " " + parametros [2] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(enemy-at " + parametros [3] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(enemy-at " + parametros [3] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(enemy-at " + parametros [3] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(enemy-at " + parametros [3] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-to " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(blocked " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(blocked " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(blocked " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(blocked " + parametros [3] + ")"].Add (accion);
				}
			}
			break;
		case "lever-on":
			if (actionPerPrecondition.ContainsKey ("(switch-on " + parametros [1] + ")")) {
				string cond = "(switch-on " + parametros [1] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
					if (actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
						string cond = "(open " + obstacle.name + ")";
						foreach (string actCond in actionPerPrecondition [cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(blocked " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(blocked " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(blocked " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(blocked " + obstacle.name + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "lever-off":
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
					if (actionPerPrecondition.ContainsKey ("(blocked " + obstacle.name + ")")) {
						string cond = "(blocked " + obstacle.name + ")";
						foreach (string actCond in actionPerPrecondition [cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-on " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(switch-on " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-on " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-on " + parametros [1] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(open " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(open " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(open " + obstacle.name + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "machine-on":
			if (actionPerPrecondition.ContainsKey ("(switch-on " + parametros [1] + ")")) {
				string cond = "(switch-on " + parametros [1] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (obstacle.type == ObstacleType.rollable) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						if (actionPerPrecondition.ContainsKey ("(rollable-open " + obstacle.name + ")")) {
							string cond = "(rollable-open " + obstacle.name + ")";
							foreach (string actCond in actionPerPrecondition [cond]) {
								if (!actCond.Equals ("")) {
									if (!constraintsPerAction.ContainsKey (accion)) {
										constraintsPerAction.Add (accion, new List<string> ());
									}
									if (!constraintsPerAction [accion].Contains (actCond)) {
										constraintsPerAction [accion].Add (actCond);
									}
								}
							}
							//actionPerPrecondition.Remove (cond);
							add = true;
						}
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(machine-loaded " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(machine-loaded " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(machine-loaded " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(machine-loaded " + parametros [1] + ")"].Add (accion);
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable) {
						if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
								actionPerPrecondition.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")", new List<string> ());
							}
							if (!actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Contains (accion)) {
								actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Add (accion);
							}

							if (!actionPerPrecondition.ContainsKey ("(rollable-locked " + obstacle.name + ")")) {
								actionPerPrecondition.Add ("(rollable-locked " + obstacle.name + ")", new List<string> ());
							}
							if (!actionPerPrecondition ["(rollable-locked " + obstacle.name + ")"].Contains (accion)) {
								actionPerPrecondition ["(rollable-locked " + obstacle.name + ")"].Add (accion);
							}
						}
					}
				}
			}
			break;
		case "item-pick":
			if (actionPerPrecondition.ContainsKey ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
				string cond = "(player-inventory " + parametros [0] + " " + parametros [1] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(item-at " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(item-at " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(item-at " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(item-at " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}
			}
			break;
		case "item-drop":
			if (actionPerPrecondition.ContainsKey ("(item-at " + parametros [1] + " " + parametros [2] + ")")) {
				string cond = "(item-at " + parametros [1] + " " + parametros [2] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

			}
			break;
		case "rune-use":
			foreach (PlannerPoi poi in poiList) {
				if (initDef.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && initDef.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && initDef.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
					if (actionPerPrecondition.ContainsKey ("(open " + parametros [3] + ")")) {
						string cond = "(open " + parametros [3] + ")";
						foreach (string actCond in actionPerPrecondition [cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(item-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(item-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(item-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(item-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(blocked " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(blocked " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(blocked " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(blocked " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(door-rune " + parametros [3] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(door-rune " + parametros [3] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(door-rune " + parametros [3] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(door-rune " + parametros [3] + " " + parametros [1] + ")"].Add (accion);
				}

				foreach (PlannerPoi poi in poiList) {
					if (initDef.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && initDef.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && initDef.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [2] + " " + poi.name + ")")) {
							actionPerPrecondition.Add ("(route-to " + parametros [2] + " " + poi.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(route-to " + parametros [2] + " " + poi.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(route-to " + parametros [2] + " " + poi.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
							actionPerPrecondition.Add ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")"].Contains (accion)) {
							actionPerPrecondition ["(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
							actionPerPrecondition.Add ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")"].Contains (accion)) {
							actionPerPrecondition ["(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "gear-use":
			if (actionPerPrecondition.ContainsKey ("(machine-loaded " + parametros [3] + ")")) {
				string cond = "(machine-loaded " + parametros [3] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [3] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [3] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [3] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [3] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-inventory " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-inventory " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(item-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(item-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(item-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(item-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(machine-gear " + parametros [3] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(machine-gear " + parametros [3] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(machine-gear " + parametros [3] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(machine-gear " + parametros [3] + " " + parametros [1] + ")"].Add (accion);
				}
			}
			break;
		case "step-on":
			if (actionPerPrecondition.ContainsKey ("(switch-on " + parametros [1] + ")")) {
				string cond = "(switch-on " + parametros [1] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
					if (actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
						string cond = "(open " + obstacle.name + ")";
						foreach (string actCond in actionPerPrecondition[cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [1] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [1] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [1] + " " + parametros [0] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(linked-switch " + parametros [1] + " " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(linked-switch " + parametros [1] + " " + obstacle.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(blocked " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(blocked " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(blocked " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(blocked " + obstacle.name + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "triple-switch":
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")")) {
					if (actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
						string cond = "(open " + obstacle.name + ")";
						foreach (string actCond in actionPerPrecondition [cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [4] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [4] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [4] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [4] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [1] + " " + parametros [4] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [1] + " " + parametros [4] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [1] + " " + parametros [4] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [1] + " " + parametros [4] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [2] + " " + parametros [4] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [2] + " " + parametros [4] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [2] + " " + parametros [4] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [2] + " " + parametros [4] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [3] + " " + parametros [4] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [3] + " " + parametros [4] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [3] + " " + parametros [4] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [3] + " " + parametros [4] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [3] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [3] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [3] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [3] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [3] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [3] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [3] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-distinct " + parametros [1] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-distinct " + parametros [1] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-distinct " + parametros [1] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-distinct " + parametros [1] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-distinct " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-distinct " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [3] + " " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(linked-switch " + parametros [3] + " " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(linked-switch " + parametros [3] + " " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(linked-switch " + parametros [3] + " " + obstacle.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(blocked " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(blocked " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(blocked " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(blocked " + obstacle.name + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "doble-switch":
			foreach (PlannerObstacle obstacle in obstacleList) {
				if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")")) {
					if (actionPerPrecondition.ContainsKey ("(open " + obstacle.name + ")")) {
						string cond = "(open " + obstacle.name + ")";
						foreach (string actCond in actionPerPrecondition [cond]) {
							if (!actCond.Equals ("")) {
								if (!constraintsPerAction.ContainsKey (accion)) {
									constraintsPerAction.Add (accion, new List<string> ());
								}
								if (!constraintsPerAction [accion].Contains (actCond)) {
									constraintsPerAction [accion].Add (actCond);
								}
							}
						}
						//actionPerPrecondition.Remove (cond);
						add = true;
					}
				}
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [1] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [1] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [1] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [1] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-at " + parametros [2] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(switch-at " + parametros [2] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-at " + parametros [2] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-at " + parametros [2] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [2] + " " + parametros [0] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [2] + " " + parametros [0] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [2] + " " + parametros [0] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [2] + " " + parametros [0] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(switch-assign " + parametros [2] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(switch-assign " + parametros [2] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(switch-assign " + parametros [2] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(switch-assign " + parametros [2] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(player-distinct " + parametros [0] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(player-distinct " + parametros [0] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-distinct " + parametros [0] + " " + parametros [1] + ")"].Add (accion);
				}

				foreach (PlannerObstacle obstacle in obstacleList) {
					if (initDef.Contains ("(linked-switch " + parametros [2] + " " + obstacle.name + ")")) {
						if (!actionPerPrecondition.ContainsKey ("(linked-switch " + parametros [2] + " " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(linked-switch " + parametros [2] + " " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(linked-switch " + parametros [2] + " " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(linked-switch " + parametros [2] + " " + obstacle.name + ")"].Add (accion);
						}

						if (!actionPerPrecondition.ContainsKey ("(blocked " + obstacle.name + ")")) {
							actionPerPrecondition.Add ("(blocked " + obstacle.name + ")", new List<string> ());
						}
						if (!actionPerPrecondition ["(blocked " + obstacle.name + ")"].Contains (accion)) {
							actionPerPrecondition ["(blocked " + obstacle.name + ")"].Add (accion);
						}
					}
				}
			}
			break;
		case "push-boulder":
			if (actionPerPrecondition.ContainsKey ("(open " + parametros [1] + ")")) {
				string cond = "(open " + parametros [1] + ")";
				foreach (string actCond in actionPerPrecondition [cond]) {
					if (!actCond.Equals ("")) {
						if (!constraintsPerAction.ContainsKey (accion)) {
							constraintsPerAction.Add (accion, new List<string> ());
						}
						if (!constraintsPerAction [accion].Contains (actCond)) {
							constraintsPerAction [accion].Add (actCond);
						}
					}
				}
				//actionPerPrecondition.Remove (cond);
				add = true;
			}
			if (add) {
				if (!actionPerPrecondition.ContainsKey ("(player-at " + parametros [0] + " " + parametros [2] + ")")) {
					actionPerPrecondition.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Contains (accion)) {
					actionPerPrecondition ["(player-at " + parametros [0] + " " + parametros [2] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-to " + parametros [2] + " " + parametros [3] + ")")) {
					actionPerPrecondition.Add ("(route-to " + parametros [2] + " " + parametros [3] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-to " + parametros [2] + " " + parametros [3] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-to " + parametros [2] + " " + parametros [3] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(route-block " + parametros [2] + " " + parametros [3] + " " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(blocked " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(blocked " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(blocked " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(blocked " + parametros [1] + ")"].Add (accion);
				}

				if (!actionPerPrecondition.ContainsKey ("(rollable-open " + parametros [1] + ")")) {
					actionPerPrecondition.Add ("(rollable-open " + parametros [1] + ")", new List<string> ());
				}
				if (!actionPerPrecondition ["(rollable-open " + parametros [1] + ")"].Contains (accion)) {
					actionPerPrecondition ["(rollable-open " + parametros [1] + ")"].Add (accion);
				}
			}
			break;
		}
	}

	private void GetEstadoPorAccionPop(List<List<string>> estadosNivelAnterior, List<List<string>> accionesNivelAnterior, List<Dictionary<string,List<string>>> ordenesNivelAnterior){
		Debug.Log ("estado plan pop start");
		List<List<string>> estadosNivelActual = new List<List<string>> ();
		List<List<string>> accionesNivelActual = new List<List<string>>();
		List<Dictionary<string,List<string>>> ordenesNivelActual = new List<Dictionary<string, List<string>>>();
		//List<int> indexAcciones = new List<int> ();
		bool lastVacio = true;
		for (int i = 0; i < estadosNivelAnterior.Count; i++) {
			Debug.Log ("start i:"+i);
			Debug.Log ("ordenes list:" + estadosNivelAnterior.Count);
			Debug.Log ("acciones list:" + accionesNivelAnterior.Count);
			Debug.Log ("ordenes list:" + ordenesNivelAnterior.Count);
			List<string> actionSet = lastActionSet (accionesNivelAnterior [i], ordenesNivelAnterior [i]);
			Debug.Log ("end i:"+i);
			if (actionSet.Count != 0) {
				lastVacio = false;
			}
			foreach (string accion in actionSet) {
				//Aplicar regresion sobre el estado actual, borrar la accion del estado y aplicar prefijo a los ordenes
				Debug.Log ("regresivo start i:"+i);
				List<string> estadoRegresivo = this.GetEstadoPorAccionRegresion (estadosNivelAnterior [i], accion);
				Debug.Log ("regresivo end i:"+i);
				estadosNivelActual.Add (new List<string> (estadoRegresivo));
				//for (int k = Plan.Count - 1; k >= 0; k--) {
				//	if (Plan [k].Equals (accion)) {
				//		indexAcciones.Add (k);
				//		break;
				//	}
				//}
				Debug.Log ("remove start i:"+i);
				accionesNivelActual.Add( new List<string>(accionesNivelAnterior[i]));
				accionesNivelActual.Last ().Remove (accion);
				ordenesNivelActual.Add (new Dictionary<string, List<string>> (prefixOrdenes (accion, ordenesNivelAnterior [i])));
				//prefixOrdenes (accion, ordenesNivelAnterior [i]);
				Debug.Log ("remove end i:"+i);
			}
		}
		//EstadoPorAccionPop.Add (new List<List<string>> (estadosNivelActual));
		//AccionPop.Add (indexAcciones);
		if (!lastVacio) {
			this.GetEstadoPorAccionPop (estadosNivelActual, accionesNivelActual, ordenesNivelActual);
			Debug.Log ("lista last no vacia");
		}
		Debug.Log ("estado plan pop end");
	}

	private List<string> lastActionSet(List<string> acciones, Dictionary<string,List<string>> ordenes){
		List<string> actionSet = new List<string> ();
		foreach (string accion in acciones) {
			if (!ordenes.ContainsKey (accion)) {
				actionSet.Add (accion);
			}
		}
		return actionSet;
	}

	private Dictionary<string, List<string>> prefixOrdenes(string accion, Dictionary<string,List<string>> ordenes){
		Dictionary<string, List<string>> prefix = new Dictionary<string, List<string>> ();
		List<string> accionPorBorrar = new List<string> ();
		foreach (string accionKey in ordenes.Keys.ToList()) {
			if (ordenes [accionKey].Contains (accion)) {
				ordenes [accionKey].Remove (accion);
			}
			if (ordenes [accionKey].Count == 0) {
				accionPorBorrar.Add (accionKey);
			}
		}
		foreach (string accionKey in accionPorBorrar) {
			ordenes.Remove (accionKey);
		}
		prefix = new Dictionary<string, List<string>> (ordenes);
		return prefix;
	}

	//Agrega a las etapas auxiliares
	private void AgregarAccionPop(int i, int j, Dictionary<string,string> etapasAuxiliares){
		string accion = Plan [AccionPop [i] [j]];
		List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
		parametros.RemoveAt (0);
		string nombre = parametros [0];
		parametros.RemoveAt (0);
		if (nombre.Equals ("triple-switch")) {
			foreach (string personaje in etapasAuxiliares.Keys.ToList()) {
				etapasAuxiliares [personaje] = accion;
				Debug.Log ("cumple accion:" + accion);
				if (nivelPop[personaje] > i) {
					Debug.Log ("cambio estado");
					nivelPop[personaje] = i;
					etapaPop [personaje] = j;
					feedbackLevel = 0;
				}
				distanciaObjetiva += i;
			}
		} else {
			string personaje = parametros [0];
			if (etapasAuxiliares [personaje].Equals ("")) {
				etapasAuxiliares [personaje] = accion;
				Debug.Log ("cumple accion:" + accion);
				if (nivelPop[personaje] > i) {
					Debug.Log ("cambio estado");
					nivelPop[personaje] = i;
					etapaPop [personaje] = j;
					feedbackLevel = 0;
				}
				distanciaObjetiva += i;
			}
		}
	}

	private void SplitPlan(){
		foreach (string action in this.Plan) {
			List<string> personajes = new List<string> ();
			List<string> parametros = new List<string> (action.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			if (nombre.Equals ("triple-switch")) {
				personajes.Add (parametros [0]);
				personajes.Add (parametros [1]);
				personajes.Add (parametros [2]);
			} else if (nombre.Equals ("doble-switch")) {
				personajes.Add (parametros [0]);
				personajes.Add (parametros [1]);
			} else {
				personajes.Add (parametros [0]);
			}
			foreach (string personaje in personajes) {
				if (!this.PlanParcial.ContainsKey (personaje)) {
					this.PlanParcial.Add (personaje, new List<string> ());
				}
				if (reverseConstraintsPerAction.ContainsKey (action) && reverseConstraintsPerAction [action].Count > 0) {
					foreach (string constAction in reverseConstraintsPerAction[action]) {
						List<string> param = new List<string> (constAction.Split (new char[]{ '(', ')', ' ' }));
						if (param [1].Equals ("doble-switch") && !param[2].Equals(personaje) && !param[3].Equals(personaje)) {
							this.PlanParcial [personaje].Add ("x" + constAction);
						} else if (!param [1].Equals ("triple-switch") && !param [2].Equals (personaje)) {
							this.PlanParcial [personaje].Add ("x" + constAction);
						}
					}
				}
				this.PlanParcial [personaje].Add (action);
			}
		}
		distanciaObjetiva = 0;
		foreach (string personaje in this.PlanParcial.Keys.ToList()) {
			distanciaObjetiva += this.PlanParcial [personaje].Count;
		}
	}

	void GetEstadosDesdePlanParcialRegresion(){
		if (control && Client.instance != null && Client.instance.GetLocalPlayer () != null && Client.instance.GetLocalPlayer ().controlOverEnemies) {
			//Se deben separar los objetivos por agente
			this.EstadoPorAccionParcial = new Dictionary<string, List<List<string>>> ();
			foreach (string personaje in this.PlanParcial.Keys.ToList()) {
				List<string> estadoActual = new List<string> ();
				foreach (string pred in goalDef) {
					List<string> parametros = new List<string> (pred.Split (new char[]{ '(', ')', ' ' }));
					if (parametros [2].Equals (personaje)) {
						estadoActual.Add (pred);
					}
				}
				this.EstadoPorAccionParcial.Add (personaje, new List<List<string>> ());
				this.EstadoPorAccionParcial[personaje].Add (new List<string> (estadoActual));
				foreach (string action in this.PlanParcial[personaje].Reverse<string>().ToList()) {
					List<string> param = new List<string> (action.Split (new char[]{ '(', ')', ' ' }));
					if (param [1].Equals ("triple-switch") || param [1].Equals ("doble-switch")) {
						estadoActual = new List<string> (GetEstadoPorAccionRegresionMultiple (personaje, estadoActual, action));
					} else if (param [0].Equals ("x")) {
						estadoActual = new List<string> (GetEstadoPorAccionRegresionParcial (personaje, estadoActual, action));
					} else {
						estadoActual = new List<string> (GetEstadoPorAccionRegresion (estadoActual, action));
					}
					this.EstadoPorAccionParcial [personaje].Add (new List<string> (estadoActual));
				}
			}
		}
	}
}