using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System;
using Config = GameConfig;
using KeywordDetection;
using BlinkDetection;
using StompDetection;

public class UserInputManager : MonoBehaviour {


	public WayPointManager TutorialWaypointManager;
	public WayPointManager ExperimentWaypointManager;

	public GameObject methodSelectionCanvas;
	public Toggle vectionToggle;
	public Slider speedSlider;
	public Slider stompSlider;
	public Text stompSliderValue;
	public Slider blinkSlider;
	public Text blinkSliderValue;
	public Toggle LogDataToggle;
	public Slider dwellSlider;
	public Text dwellSliderValue;

	public Image PanelImage;
	public Image[] ButtonImages;

	public GameObject ControllerParabolicPointer;
	public GameObject GazeParabolicPointer;
	public GameObject laserPointer;
	public GameObject controller;


	public float teleportSpeed;
	public bool useVection;
	public float dwellDuration;

	private GameObject mainCamera;
	private float canvasYOffset;

	private StompDetector stompDetector;
	private KeywordDetector keywordDetector;
	private BlinkDetector blinkDetector;
		
	public GameObject blinkLogCanvas;

	private Color ExperimentPanelColor;
	private Color ExperimentButtonColor;
	private Color TutorialPanelColor;
	private Color TutorialButtonColor;

	[SerializeField]
	private Fader fader;

	[SerializeField]
	private string[] startPositionsTutorial;
	[SerializeField]
	private string[] startPositionsExperiment;

	private static Transform playerTransform;

	void Start () {

		playerTransform = transform;

		ExperimentPanelColor = new Color32 (0x03,0x8E,0x32,0x71);
		ExperimentButtonColor = new Color32 (0x84,0xCC,0x77,0XFF);

		TutorialPanelColor = new Color32 (0x03,0x74,0xAB,0xC8);
		TutorialButtonColor = new Color32 (0x3F,0xB7,0xF6,0xFF);


		stompDetector = GameObject.FindObjectOfType<StompDetector> ();
		UnityEngine.Assertions.Assert.IsNotNull(stompDetector );

		keywordDetector = GameObject.FindObjectOfType<KeywordDetector> ();
		UnityEngine.Assertions.Assert.IsNotNull (keywordDetector);

//		blinkDetector = GameObject.FindObjectOfType<BlinkDetector> ();
//		UnityEngine.Assertions.Assert.IsNotNull (blinkDetector);
//

		Config.init (teleportSpeed, useVection,TeleportMethod.Controller,GameState.Pause,
			stompDetector.stompSensitivity,5,1000,
			dwellDuration, false, GameMode.Tutorial);


		mainCamera=GameObject.FindGameObjectWithTag ("MainCamera");
		canvasYOffset = Math.Abs (transform.position.y -
			methodSelectionCanvas.transform.position.y);
		updateCanvasUI (true);

	}
	void Update () {
		// toggle canvas when app button is pressed on the controller.
		if ( (GvrController.AppButtonDown && !TeleportScript.ismoving) || 
			WayPointManager.TrialComplete ) {

			if (WayPointManager.TrialComplete) {
				WayPointManager.TrialComplete = false;
			}
			bool canvasWasVisible = methodSelectionCanvas.activeInHierarchy;
			updateCanvasUI (!canvasWasVisible);
			startGame (canvasWasVisible?1:0);
		} 

	}	

	// sync the UI with the model
	void updateCanvasUI(bool update){
		
		if (update) {
			
			Config currentConfig = Config.currentConfig ();
			speedSlider.value = currentConfig.TeleportSpeed;
			vectionToggle.isOn = currentConfig.UseVection;

			stompSlider.value = currentConfig.StompSensitivity;
			stompSliderValue.text = stompSlider.value.ToString ();

			blinkSlider.value = currentConfig.LightThreshold;
			blinkSliderValue.text = blinkSlider.value.ToString ();

			dwellSlider.value = currentConfig.DwellDuration;
			dwellSliderValue.text = dwellSlider.value.ToString ();

			LogDataToggle.isOn = currentConfig.LogData;

			currentConfig.Gamestate = GameState.Pause;

			showCanvas ();
		}
	}

	public void selectTutorialExperiment(int t) {
	
		bool selectExperiment = t == 2;
		bool selectTutorial = !selectExperiment;
		Color pc;
		Color bc;
		LogDataToggle.enabled = true;
		if (selectTutorial) {
			pc = TutorialPanelColor;
			bc = TutorialButtonColor;
		} else {
			pc = ExperimentPanelColor;
			bc = ExperimentButtonColor;
			LogDataToggle.isOn = true;
			LogDataToggle.enabled = false;
		}
		PanelImage.color = pc;
		foreach (Image i in ButtonImages) {
			i.color = bc;
		}
		Config.currentConfig ().GameMode = (GameMode) t;
	}


	private  void showCanvas() {

		ControllerParabolicPointer.SetActive (false);
		GazeParabolicPointer.SetActive (false);
		controller.SetActive (true);
		laserPointer.SetActive (true);
		methodSelectionCanvas.SetActive (true);

		Vector3 f = mainCamera.transform.forward;
		f.y = 1.0f;
		Vector3 lookatPoint = transform.position +
			Vector3.Scale(f,new Vector3(4.5f,canvasYOffset,4.5f));
		methodSelectionCanvas.transform.position =
			transform.position + Vector3.Scale(f, new Vector3(3.5f,canvasYOffset,3.5f));
		methodSelectionCanvas.transform.LookAt (lookatPoint);

		/*Hide the waypoints when showing the canvas*/
		if (!GvrController.AppButtonDown) {
			ExperimentWaypointManager.gameObject.SetActive (false);
			TutorialWaypointManager.gameObject.SetActive (false);	
		}

	}

	public void updateSliders () {
		blinkSliderValue.text = blinkSlider.value.ToString ();
		stompSliderValue.text = stompSlider.value.ToString ();
		dwellSliderValue.text = dwellSlider.value.ToString ();
	}


	// update the model from the UI
	void updateConfig(bool update, int t){

		if (update) {
			Config curretConfig = Config.currentConfig ();
			if(t>1)
				Config.currentConfig ().Teleportmethod =(TeleportMethod)t;
			curretConfig.TeleportSpeed= speedSlider.value;;
			curretConfig.UseVection = vectionToggle.isOn;
			curretConfig.LogData = LogDataToggle.isOn;
			stompDetector.StompSensitivity = curretConfig.StompSensitivity = stompSlider.value;
			curretConfig.DwellDuration = dwellSlider.value;
			curretConfig.Gamestate=GameState.Play;
			methodSelectionCanvas.SetActive (false);
		}
	}

	public void startGame(int t){
		
		if (t>0) {
			
			updateConfig(true,t);
			UnityEngine.Debug.Log (Config.currentConfig().toString());
			TeleportMethod method = Config.currentConfig ().Teleportmethod;

			if (!GvrController.AppButtonDown) {
				
				int posIndex = ((int)method) - 2;
				string startposition = startPositionsTutorial [posIndex];
				Transform startpositionTransform = null;
				if (GameConfig.currentConfig ().GameMode == GameMode.Experiment) {
					startposition = startPositionsExperiment [posIndex];
					startpositionTransform = 
						ExperimentWaypointManager.gameObject.transform.Find (startposition);
				} else {
					startpositionTransform =
						TutorialWaypointManager.gameObject.transform.Find(startposition);
				}
				UnityEngine.Debug.Log (startpositionTransform.position);
				fader.fade (rePositionPlayer, startpositionTransform);

				int nextStart = 0;
				int.TryParse(startposition, out nextStart);

				/*Enable the right set of waypoints (Tutorial or Experiment). */

				bool enableExperimentWaypoint = Config.currentConfig ().GameMode ==
					GameMode.Experiment;
				bool enableTutorialWaypoint = !enableExperimentWaypoint;

				TutorialWaypointManager.reset (enableTutorialWaypoint, nextStart+1);
				ExperimentWaypointManager.reset (enableExperimentWaypoint, nextStart+1);

				TutorialWaypointManager.gameObject.SetActive (enableTutorialWaypoint);
				ExperimentWaypointManager.gameObject.SetActive (enableExperimentWaypoint);

			}
			/* Deactivate all pointers (laser, GazeParabolic, ControllerParabolic),
			* we will enable below as required per teleportation method.
			*/
			laserPointer.SetActive (false);
			ControllerParabolicPointer.SetActive (false);
			GazeParabolicPointer.SetActive (false);

			/* Remove all action listeners (StompAction, SpeechAction, BlinkAction),
			 * 	We will add action listeners below as required, per teleportation method.
			*/
			stompDetector.RemoveStompAction (TeleportScript.OnStomp);
			keywordDetector.OnKeyword -= TeleportScript.OnVoice;

			switch (method) {

			case TeleportMethod.GazeStomp:
				stompDetector.AddStompAction(TeleportScript.OnStomp);
				controller.SetActive (false);
				GazeParabolicPointer.SetActive (true);
				TeleportScript.Pointer = GazeParabolicPointer.GetComponent<ParabolicPointer> ();
				return;
			case TeleportMethod.GazeVoice:
				keywordDetector.OnKeyword += TeleportScript.OnVoice;
				controller.SetActive (false);
				GazeParabolicPointer.SetActive (true);
				TeleportScript.Pointer = GazeParabolicPointer.GetComponent<ParabolicPointer> ();
				return;
			case TeleportMethod.GazeBlink:
				blinkDetector.OnBlink += TeleportScript.OnBlink;
				blinkDetector.StartListening ();
				controller.SetActive (false);
				GazeParabolicPointer.SetActive (true);
				TeleportScript.Pointer = GazeParabolicPointer.GetComponent<ParabolicPointer> ();
				return;
			case TeleportMethod.GazeDwell:
				controller.SetActive (false);
				GazeParabolicPointer.SetActive (true);
				TeleportScript.Pointer = GazeParabolicPointer.GetComponent<ParabolicPointer> ();
				return;	
			default :
				controller.SetActive (true);
				GazeParabolicPointer.SetActive (false);
				ControllerParabolicPointer.SetActive (true);
				TeleportScript.Pointer = ControllerParabolicPointer.GetComponent<ParabolicPointer> ();
				return;
			}


		}

	}

	public static void rePositionPlayer(Transform t) {
		Vector3 pos = t.position;
		pos.y = 1.8f;
		playerTransform.transform.position = pos;
	}
}

