using UnityEngine;
using System.Collections;
using UnityEditor;



[CustomEditor(typeof(DistanceFromLastPoint))]
public class DistanceFromLastPointEditor : Editor {

	DistanceFromLastPoint currentWaypoint;

	void OnEnable(){
		currentWaypoint = (DistanceFromLastPoint)target;
	}
	public override void OnInspectorGUI(){

		if (Application.isPlaying)
			return;
		Vector3 a = currentWaypoint.transform.position - 
			currentWaypoint.previousWaypoint.transform.position;
		Vector3 b = currentWaypoint.previousWaypoint.transform.position - 
			currentWaypoint.prev_previousWaypoint.transform.position;
		currentWaypoint.distanceFromLastWaypoint = Vector3.Distance (currentWaypoint.transform.position,
			currentWaypoint.previousWaypoint.transform.position);
		currentWaypoint.distanceFromPreLastWaypoint = Vector3.Distance(currentWaypoint.transform.position,
			currentWaypoint.prev_previousWaypoint.transform.position);
		float angle = Vector3.Angle(a,b);
		GUILayout.Label ("   ");
		EditorGUILayout.LabelField ("Position = " + currentWaypoint.transform.position );
		EditorGUILayout.LabelField ("Distance from Last Waypoint : (" + currentWaypoint.previousWayPointName+") " +
			"" +currentWaypoint.distanceFromLastWaypoint);
		EditorGUILayout.LabelField ("Distance from Prev-Last Waypoint : (" + currentWaypoint.pre_previousWaypointName+
			") " +currentWaypoint.distanceFromPreLastWaypoint);
		EditorGUILayout.LabelField ("Angle : (" + angle +") ");
		this.Repaint ();
	}
		

}
