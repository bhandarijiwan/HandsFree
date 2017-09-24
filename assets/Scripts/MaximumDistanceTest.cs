using UnityEngine;
using System.Collections;

public class MaximumDistanceTest : MonoBehaviour {

	ParabolicPointer p;
	// Use this for initialization
	void Start () {
		p=this.GetComponent<ParabolicPointer> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (Vector3.Distance (transform.position,p.SelectedPoint));
	}
}
