using UnityEngine;
using System.Collections;

public class SelfDisappear : MonoBehaviour {

	private float timetodestroy=7.0f;

	private float t=0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		t += Time.deltaTime;
		if (t > timetodestroy) {
			t = 0;
			gameObject.SetActive (false);
		}

	}
	public void showMessagePanel(){
		gameObject.SetActive (true);
	}

}
