using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BlinkDetection {	
	
	public class BlinkDetector_Android: IBlinkDetector {

		private static string  TAG ="BlinkDetection";

		#region private fields
		private AndroidJavaObject _blinkDetectorObj;
		#endregion

		private void CallBlinkDetectorMethod(string methodName, params System.Object[] args)
		{
			using (AndroidJavaClass BlinkDetectorClass = new AndroidJavaClass ("edu.cmu.pocketsphinx.unityWrap.BlinkDetector")) {
			
				using (_blinkDetectorObj = BlinkDetectorClass.CallStatic<AndroidJavaObject> ("getInstance")) {
				
					_blinkDetectorObj.Call (methodName, args);
					Debug.logger.Log ("BlinkDetection", "Called "+ methodName + "().");
				}

			}
		}


		#region constructor 
		public BlinkDetector_Android()
		{
			using (AndroidJavaClass BlinkDetectorClass = new AndroidJavaClass("edu.cmu.pocketsphinx.unityWrap.BlinkDetector"))
			{
				Debug.logger.Log ("BlinkDetection", "Got android blink detectoion class");
				_blinkDetectorObj = BlinkDetectorClass.CallStatic<AndroidJavaObject> ("getInstance");
				var pl_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = pl_class.GetStatic<AndroidJavaObject>("currentActivity");
				_blinkDetectorObj.Call ("setActivityContext", currentActivity);
				Debug.logger.Log ("BlinkDetection", "BlinkDetector_Android java object created");

			}
		}
		#endregion 

		#region public methods 
		public void setupDetector() {
			Debug.logger.Log (TAG," setupDetector() called.");
			CallBlinkDetectorMethod ("setupDetector");
		}

		public void setThrottleTime(int time) {
			Debug.logger.Log (TAG, "setThrottleTime() called.");
			CallBlinkDetectorMethod ("setThrottleTime", time );
		}

		public void setLightThreshold(float threshold) {
			Debug.logger.Log (TAG, "setLightThreshold() called");
			CallBlinkDetectorMethod ("setLightThreshold", threshold);
		}

		public void StartListening() {
			Debug.logger.Log (TAG, "StartListening() called");
			CallBlinkDetectorMethod ("StartListening");
		}

		public void setLogContinous( bool l) {
			Debug.logger.Log (TAG, "LogContinuous() called");
			CallBlinkDetectorMethod ("setLogContinous", l);
		}

		public void StopListening(){
			Debug.logger.Log (TAG, "StopListening() called");
			CallBlinkDetectorMethod ("StopListening");
		}

		#endregion

	}


}

