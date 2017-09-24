using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class WayPointManager : MonoBehaviour {

	private int waypointindex = 0;
	private int TOTALWAYPOINTCOUNT = 0;
	private int waypointcount = 0;
	private List<GameObject> waypoints;
	private GameObject currentwaypoint;
	public static WaypointData currentwaypointdata;
	public static bool TrialComplete;
	public Material waypointMaterial;
	public Material waypointSelectMaterial;
	private Renderer currentWaypointRender;
	private bool targetFound = false;
	private AudioSource teleportSound;
	private Stream Log;


	// Use this for initialization
	void Start () {
		TrialComplete = false;
		waypoints = new List<GameObject> ();
		foreach(Transform child in transform) {
			waypoints.Add (child.gameObject);
			TOTALWAYPOINTCOUNT++;
			child.gameObject.SetActive (false);
		}
		teleportSound = GetComponentInParent <AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

		if (GameConfig.currentConfig ().Gamestate == GameState.Play) {

			if (currentwaypoint != null) {

				Vector3 waypointposition = currentwaypoint.transform.position;
				Vector2 waypointposition2D = new Vector2 (waypointposition.x,waypointposition.z); 
				Vector3 pointerPosition = TeleportScript.Pointer.SelectedPoint;
				Vector2 pointerPosition2D = new Vector2 (pointerPosition.x, pointerPosition.z);
				Vector3 playerposition = Camera.main.transform.position;
				Vector2 playerposition2D = new Vector2 (playerposition.x, playerposition.z);
				float squaredistToCursor = Vector2.SqrMagnitude (waypointposition2D - pointerPosition2D);
				float squaredistToPlayer = Vector2.SqrMagnitude (waypointposition2D - playerposition2D);

				if (squaredistToCursor <= 2.0f) {
					currentWaypointRender.material = waypointSelectMaterial;
				} else {
					currentWaypointRender.material = waypointMaterial; 
				}
				if (!targetFound) {
					
					Vector3 screenPoint = Camera.main.WorldToViewportPoint (waypointposition);
					targetFound = screenPoint.z > 0 && screenPoint.x > 0 
						&& screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
				}
				if (targetFound) {
					currentwaypointdata.Teleportduration += Time.deltaTime;

				} else {
					currentwaypointdata.VisualSearchDuration += Time.deltaTime;
				}
				if (squaredistToPlayer <= 2.0f) {
					/* Log the data for current waypoint*/
					log (waypointcount);
					teleportSound.Play ();
					/* Activate the next waypoint*/
					++waypointindex;
					activateNext();
				}
			}
		
		}


	}

	public void activateNext() {
		waypointcount++;
		int n = waypointindex % TOTALWAYPOINTCOUNT;
		int n_prev = Mathf.Abs (waypointindex - 1) % TOTALWAYPOINTCOUNT;

		waypoints [n_prev].SetActive (false);
		if (waypointcount > TOTALWAYPOINTCOUNT) {
			TrialComplete = true;
			currentwaypoint = null;
			currentwaypointdata = null;
			return;
		}
		currentwaypointdata = 
			new WaypointData (n,waypoints[n_prev].transform.position, 
				waypoints [n].transform.position);	
		currentwaypoint = waypoints [n];
		currentwaypoint.SetActive (true);
		currentWaypointRender = currentwaypoint.GetComponent<Renderer>();
		targetFound = false;
	}

	public void reset(bool reset, int nextStart) {

		if (reset) {
			foreach (Transform t in transform) {
				t.gameObject.SetActive (false);
			}
			waypointindex = (nextStart) % TOTALWAYPOINTCOUNT;
			Debug.Log (waypointindex);
			setUpLogger ();
			waypointcount = 0;
			TrialComplete = false;
			activateNext ();
		}

	}
	private void setUpLogger() {

		GameConfig config = GameConfig.currentConfig ();
		if (Log != null) {
			Log.Flush ();
			Log.Close ();
			Log = null;
		}
		if (config.LogData) { // log to a file

			DateTime saveTime = DateTime.Now;
			DirectoryInfo di = new DirectoryInfo (Application.persistentDataPath + "/TeleportData/");
			if (!di.Exists) {
				di.Create ();
			} else {
				di = null;
			}
			string fileName = Application.persistentDataPath + "/TeleportData/" + 
				config.GameMode + "_" + config.Teleportmethod +"_"+
				saveTime.ToString ("yyyyMMddHHmm") + ".csv";
			Debug.Log (fileName);
			Log = File.OpenWrite (fileName);
			byte[] line = ASCIIEncoding.Default.GetBytes (WaypointData.Header);
			Log.Write (line, 0, line.Length);

		} else {
			Log = null;
			Debug.Log (WaypointData.Header);
		}
	}
	private void log(int _waypointcount) {
		if (GameConfig.currentConfig ().LogData && Log != null) {
			byte[] line = ASCIIEncoding.Default.GetBytes(
				currentwaypointdata.toString ());
			Log.Write (line, 0, line.Length);
			Log.Flush ();
		} else {
			Debug.Log (currentwaypointdata.toString());
		} 
		if (_waypointcount == TOTALWAYPOINTCOUNT && Log != null) {
			Log.Flush ();
			Log.Close ();
			Log = null;
		}
	}

}

public class WaypointData {

	private int waypointnumber;

	public int Waypointnumber {
		get { 
			return waypointnumber;
		}
		set { 
			waypointnumber = value;
		}
	}
	private Vector2 Playerposition2D;
	private Vector2 Waypointposition2D;
	public float Teleportduration = 0.0f;
	public float VisualSearchDuration = 0.0f;
	public List<Vector3> TeleportPositions;
	private static char sep =',';
	public const string precision = "F2";

	public WaypointData(int _waypointnumber, Vector3 playerPosition, Vector3 position) {
		waypointnumber = _waypointnumber;
		Vector3 pp = playerPosition;
		Playerposition2D = new Vector2 (pp.x,pp.z);
		Waypointposition2D = new Vector2 (position.x,position.z);
		TeleportPositions = new List<Vector3> ();
	}

	public string toString () {
		string data = waypointnumber.ToString() + sep;
		data += Playerposition2D.ToString (precision) + sep;
		data += Waypointposition2D.ToString (precision) + sep;
		data += VisualSearchDuration.ToString () + sep;
		data += Teleportduration.ToString () + sep;
		foreach( Vector3 v in TeleportPositions){
			data += new Vector2(v.x,v.z).ToString(precision) + sep;
		}
		data = data.Replace("(", "");
		data = data.Replace(")", "");
		return data + "\n";
 	}
	public static string Header {

		get{
	
			string header = "";
			header += "Waypointnumber" + sep;
			header += "PlayerPosition2D_X" + sep;
			header += "PlayerPosition2D_Z" + sep;
			header += "WaypointPosition2D_X" + sep;
			header += "WaypointPosition2D_Z" + sep;
			header += "VisualSearchDuration" + sep;
			header += "TeleportDuration" + sep;
			header += "TeleportPoints2D" + sep;
			header += "\n";
			return header;
		}
			
	}

}