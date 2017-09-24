using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VRStep;

namespace StompDetection {

	public class StompDetector : MonoBehaviour {

		public float stompSensitivity;
	
		public float StompSensitivity {
			get { 
				return stompSensitivity;
			}
			set { 
				stompSensitivity = value;
				stepDetector.stepMinThreshold = value;
			}
		}

		private StepDetector stepDetector;

		public StepHandler OnStomp;

		void Start() {
			stepDetector = StepDetector.instance;
			stepDetector.stepMinThreshold = stompSensitivity;
		}

		public void AddStompAction(StepHandler  h) {
			stepDetector.OnStepDetected += h;
		}

		public void RemoveStompAction (StepHandler h) {
			stepDetector.OnStepDetected -=h;
		}

		void Update () {
		
		}
	}
}
