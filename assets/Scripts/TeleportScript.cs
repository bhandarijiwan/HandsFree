using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using Config = GameConfig;
using BlinkDetection;
using StompDetection;
using KeywordDetection;
/// \brief A test script for "The Lab" style teleportation if you don't have a Vive.  Keep in mind that this 
///        doesn't have fade in/out, whereas TeleportVive (a version of this specifically made for the Vive) does.
/// \sa TeleportVive
[AddComponentMenu("Vive Teleporter/Test/Teleporter Test (No SteamVR)")]
public class TeleportScript : MonoBehaviour {

	public static ParabolicPointer Pointer;

	public static bool ismoving;

	private const float epsilon = 0.001f;

	private Vector3 dest = Vector3.zero;

	public Fader fader;

	public Material cursorMaterial;

	private GameConfig gameConfig;

	[HideInInspector]
	public static bool Stomp;

	[HideInInspector]
	public static bool Voice;

	[HideInInspector]
	public static bool Blink;

	private float dwellDurationTimer;

	private bool Dwell;


	void Start(){	
		previousPosition = transform.position;
		fader.fade (null);
		gameConfig = GameConfig.currentConfig ();
		dwellDurationTimer = 0.0f;
	}
	public void fadercallback(){}

	public static void OnStomp(){
		Stomp = true;
		UnityEngine.Debug.Log ("Step");
	}

	public static void OnVoice(){
		Voice = true;
	}

	public static void OnBlink() {
		Blink = true;
	}

	private Vector3 pointerPreviousPosition;
	private Vector3 delta;
	private bool firstpoint;

	void Update () {

		if (gameConfig.Gamestate == GameState.Play) {

			if (gameConfig.Teleportmethod == TeleportMethod.GazeDwell
			    && Pointer.PointOnNavMesh) {

				if (!firstpoint) {

					delta += Pointer.SelectedPoint - pointerPreviousPosition;

					float t = dwellDurationTimer / gameConfig.DwellDuration;

					cursorMaterial.color = Color.Lerp(Color.red, Color.green,t );

					if (Vector3.SqrMagnitude (delta) <= 3.0f) {

						dwellDurationTimer += Time.deltaTime;

						if (dwellDurationTimer >= gameConfig.DwellDuration) {

							Dwell = true;
							dwellDurationTimer = 0.0f;
						}

					} else {
						delta = Vector3.zero;
						dwellDurationTimer = 0.0f;
					}
				}

				firstpoint = false;


			} else {
				
				dwellDurationTimer = 0.0f;
				firstpoint = true;
				delta = Vector3.zero;
			}

			pointerPreviousPosition = Pointer.SelectedPoint;

			bool gaze = gameConfig.Teleportmethod != TeleportMethod.Controller;
			bool virtualClick = (GvrController.ClickButtonDown  && !gaze)
								|| (Stomp && gaze)  || (Voice && gaze) 
								|| (Blink && gaze) || (Dwell && gaze);
			
			if ( virtualClick  && Pointer.PointOnNavMesh) {
				// we need to teleport
				dest = Pointer.SelectedPoint;
				dest.y = transform.position.y;
				teleport ();
			}
			if (Stomp) Stomp = false;
			if (Voice) Voice = false;
			if (Blink) Blink = false;
			if (Dwell) Dwell = false;
	
			if (ismoving) {
				blendedMove ();
			}


		}
	}

	void teleport(){

		if (WayPointManager.currentwaypointdata != null) {
			WayPointManager.currentwaypointdata.TeleportPositions.Add (dest);
		}
		if ( Config.currentConfig().UseVection) {
			distance=Vector3.Distance (dest , transform.position);
			smoothedBlend = blend = 0;
			ismoving = true;
		} else {
			transform.position = dest;
		}

	}

	float distance;
	float blend = 0;
	float smoothedBlend=0;
	public float SmoothedBlend{
		get{
			return smoothedBlend ;
		}
	}

	const float smoothInPercent = 0.5f;
	const float smoothOutPercent = 1.5f;
	private Vector3 previousPosition;
	public void blendedMove(){
		if(blend < 1){
		blend = Mathf.Clamp01(blend +Config.currentConfig().TeleportSpeed*Time.deltaTime/distance);
		}
		smoothedBlend = blend;
		if(smoothedBlend < 0.001f){
			smoothedBlend = 0;
		}
		else if (smoothedBlend > 0.999f){
			smoothedBlend = 1;
		}
		transform.position = (1-smoothedBlend)*previousPosition + smoothedBlend*dest;
		if(blend >= 1){
			ismoving = false;
			transform.position = dest;
			previousPosition = transform.position;
			blend = 0.0f;
		}

	}
}
public enum TeleportMethod{
	Controller=2,
	GazeStomp,
	GazeVoice,
	GazeDwell,
	GazeBlink
}
public enum GameState{
	Play,
	Pause
}
public enum GameMode {
	Tutorial=1,
	Experiment
}

public class GameConfig{
	private static GameConfig _instance;
	private float teleportSpeed;
	private bool useVection;
	private TeleportMethod teleportmethod;
	private GameState gamestate;
	private float stompSensitivity;
	private float lightThreshold;
	private int blinkThrottle;
	private float dwellDuration;
	private bool logdata;
	private GameMode gameMode;
	private GameConfig(float t, bool v,TeleportMethod m, GameState g, float ss, float lt, int bt, float dd, bool lb, GameMode gm){
		teleportSpeed = t;
		useVection = v;
		teleportmethod = m;
		gamestate = g;
		stompSensitivity = ss;
		lightThreshold = lt;
		blinkThrottle = bt;
		dwellDuration = dd;
		logdata = lb;
		gameMode = gm;
	}
	public static void init(float t, bool v, TeleportMethod m,GameState g, float ss, float lt, int bt, float dd, bool lb, GameMode gm){
		_instance = new GameConfig (t, v, m, g, ss, lt, bt, dd, lb,gm);	

	}
	public static GameConfig currentConfig(){
		return _instance;
	}

	public float DwellDuration {
		get { 
			return dwellDuration;
		}
		set { 
			dwellDuration = value;		
		}

	}

	public GameMode GameMode{
		get { 
			return gameMode;
		}
		set { 
			gameMode = value;
		}
	}

	public bool LogData {
		get { 
			return logdata;
		}
		set { 
			logdata = value;
		}
	}

	public float StompSensitivity {
		get { 
			return  stompSensitivity;
		}
		set { 
			stompSensitivity = value;
		}
	}
	public float LightThreshold {
		get {
			return lightThreshold;
		}
		set { 
			lightThreshold = value;

		}
	}

	public int BlinkThrottle {
		get { 
			return blinkThrottle;
		}
		set { 
			blinkThrottle = value;
		}
	}

	public float TeleportSpeed{
		get{ 
			return teleportSpeed;
		}
		set{
			teleportSpeed = value;
		}
	}
	public bool UseVection{
		get{
			return useVection;
		}
		set{ 
			useVection = value;
		}
	}
	public TeleportMethod Teleportmethod{
		get{ 
			return teleportmethod;
		}
		set{
			teleportmethod = value;
		}
	}
	public GameState Gamestate{
		get{ 
			return gamestate;
		}
		set{ 
			gamestate = value;	
		}
	}
	public string toString(){

		return "Speed=" + _instance.teleportSpeed + "\n" +
			"Use Vection=" + _instance.useVection +"\n" +
			"Teleport Method = " + _instance.teleportmethod+"\n" +
			"Game State= "+ _instance.gamestate + "\n" + 
			"Game Mode= " + _instance.gameMode + "\n" + 
			"Log=_" + _instance.logdata + "\n"+
			"Stomp Sensitivity = " + _instance.stompSensitivity + " ";
	
	}
}

