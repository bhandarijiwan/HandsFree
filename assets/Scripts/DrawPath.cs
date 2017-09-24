using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class DrawPath : MonoBehaviour {

	[HideInInspector]
	public List<GameObject> wplist;

	[HideInInspector]
	public GameObject startObj;

	[HideInInspector]
	public GameObject lastObj;

	[HideInInspector]
	public List<GameObject> wpTList;

	void Update () {

		wplist = new List<GameObject> ();
		wpTList = new List<GameObject> ();

		foreach (Transform child in transform.Find("ExperimentWaypoints")) {
			wplist.Add (child.gameObject);
		}
		wplist.Sort (
			delegate(GameObject g1, GameObject g2) {
				return int.Parse(g1.name) - int.Parse(g2.name);
			}
		);
		foreach (GameObject go in wplist) {
			wpTList.Add (go);
		}

		startObj = wplist [0];
		lastObj = wplist [wplist.Count - 1];


	}

}
#endif