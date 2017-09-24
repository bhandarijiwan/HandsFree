using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BlinkDetection {

	public class BlinkDetector : MonoBehaviour, IBlinkEvents{

		private static string TAG ="BlinkDetection";

		public event Action OnBlink;

		public float lightThreshold;

		public int throttleTime;

		public Text lightValue;


		public bool logContinuous;

		public bool LogContinuous {
			get { 
				return logContinuous;
			}
			set {
				logContinuous = value;
				if (_blinkDetectorInst != null) {
					_blinkDetectorInst.setLogContinous (logContinuous);
				}
			}
		}

		#region private fields
		private  IBlinkDetector _blinkDetectorInst;
		#endregion 

		public void setupDetector() {
			if (_blinkDetectorInst == null) {
				return;
			}
			_blinkDetectorInst.setupDetector ();
			_blinkDetectorInst.setLogContinous (logContinuous);
		}

		public float LightThreshold {
			get { 
				return lightThreshold;
			}
			set { 
				lightThreshold = value;
				if(_blinkDetectorInst != null)
					_blinkDetectorInst.setLightThreshold (value);
			}
		}
		public int ThrottleTime {
			get {
				return throttleTime;
			}
			set {
				throttleTime = value;
				if(_blinkDetectorInst != null)
				_blinkDetectorInst.setThrottleTime (value);
			}
		}

		public void StartListening() {
			if (_blinkDetectorInst == null) {
				return;
			}
			_blinkDetectorInst.StartListening ();
		}

		public void StopListening() {
			if (_blinkDetectorInst == null) {
				return;
			}
			_blinkDetectorInst.StopListening ();
		}
	
		#region MonoBehaviour methods

		void Awake() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			_blinkDetectorInst = new BlinkDetector_Android();
			#else
			#endif
		}

		// Use this for initialization
		void Start () {
			setupDetector ();
		}

		// Update is called once per frame
		void Update () {

		}
		#endregion

		#region logging the data
		public void OnNewValue (string payload) {
			lightValue.text = payload;
		}
		#endregion


		#region public methods
		public void OnBlinkDetected (string payload) {
			Debug.logger.Log (TAG, "Blink Detected Unity");

			if (OnBlink !=null) {
				OnBlink ();
			}
		}
		#endregion
	}

}
