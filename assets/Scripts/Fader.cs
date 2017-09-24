using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Fader : MonoBehaviour {

	public float fadeSpeed = 1.5f;            // Speed that the screen fades to and from black.
	private float faderinterp;
	private Color fromColor;
	private Color toColor;
	public Material mat;

	public  delegate void Fadercallback(Transform _params = null);
	Fadercallback fadercallback;

	Transform _params;
	void Start(){
		
		toColor=fromColor = mat.color;
		fromColor.a = 1.0f;
		toColor.a = 0.0f;
		faderinterp = 0.0f;
	}


	void Update ()
	{
		mat.color = Color.Lerp (fromColor,toColor,faderinterp);

		faderinterp += Time.deltaTime * 0.8f;

		if (faderinterp > 0.10f && fadercallback!=null) {
			fadercallback (_params);
		}

		if(faderinterp>1.0f){
			faderinterp = 0.0f;
			gameObject.SetActive (false);
		}	


	}

	public void fade(Fadercallback f, Transform t = null){
		fadercallback = f;
		_params = t;
		gameObject.SetActive (true);
	}
	
}


