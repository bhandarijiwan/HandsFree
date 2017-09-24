using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlinkDetection {

	public interface IBlinkDetector {

		#region
		void setupDetector ();

		void setThrottleTime (int milliseconds);

		void setLightThreshold (float luminance_threshold);

		void setLogContinous (bool l);

		void StartListening ();

		void StopListening ();
		#endregion


	}


}