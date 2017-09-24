using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DrawPath))]
public class DrawPathEditor : Editor {

	private DrawPath drawpathObj;
	private float arrowSize=0.5f;
	private Vector3[] positions;

	void OnEnable(){
		drawpathObj = (DrawPath)target;
		if (drawpathObj == null)
			return;
		positions = new Vector3[drawpathObj.wplist.Count+1];
		positions[0] = Camera.main.transform.position;
		int i = 0;
		foreach (GameObject wp in drawpathObj.wplist)
			positions [++i] = wp.transform.position;
	}
	void OnSceneGUI(){
		Handles.color = Color.yellow;
		Handles.DrawAAPolyLine (3.0f,positions);
		Handles.color = Color.red;
		for (int i = 0; i < drawpathObj.wplist.Count; i++) {
			Quaternion t_rotation = Quaternion.LookRotation (positions [i+1]-positions [i]);
			Handles.ConeCap(0,positions[i+1],t_rotation,arrowSize);

		}
	}
	public override void OnInspectorGUI(){

//		int count = 0;

//		foreach (TeleportPoint wp in drawpathObj.wpTList) {
//
//			string showtext = wp.shortwayPointCount.ToString();
//			string wpcount =	EditorGUILayout.TextField(wp.gameObject.name, showtext);
//			wp.shortwayPointCount = int.Parse (wpcount);
//			count += wp.shortwayPointCount;
//		}
//		EditorGUILayout.LabelField( "Total shorter = " +count.ToString());
	}

}
