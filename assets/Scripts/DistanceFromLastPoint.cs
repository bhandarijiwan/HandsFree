using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
[ExecuteInEditMode]
public class DistanceFromLastPoint : MonoBehaviour {

	[HideInInspector]
	public GameObject previousWaypoint;
	// Use this for initialization

	[HideInInspector]
	public float distanceFromLastWaypoint;

	[HideInInspector]
	public float distanceFromPreLastWaypoint;

	[HideInInspector]
	public string previousWayPointName;

	[HideInInspector]
	public string pre_previousWaypointName;

	[HideInInspector]
	public GameObject prev_previousWaypoint;

	void Start () {
		string t = getNameofPreviousWayPoint ();
		previousWaypoint = GameObject.Find (t);
		pre_previousWaypointName = getNameofPrePreviousWayPoint(t);
		prev_previousWaypoint = GameObject.Find (pre_previousWaypointName);
		distanceFromPreLastWaypoint = 0.0f;
		distanceFromLastWaypoint = 0.0f;
	}

	private string getNameofPreviousWayPoint(){
		int i_name = int.Parse (this.gameObject.name) - 1;

		if (i_name >= 0) {
			previousWayPointName = i_name.ToString ();
		} else {
			previousWayPointName =  "Player";
		}
		return this.previousWayPointName;
	}

	private string getNameofPrePreviousWayPoint(string p){
		if(p.CompareTo("Player")==0) return "Player";
		int p_p = int.Parse (p);
		if (p_p <= 0)
			return "Player";
		else
			p_p--;
		return p_p.ToString ();
	}
}
#endif