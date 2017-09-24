using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlinkDetection {

	public interface IBlinkEvents 
	{
		#region public methods
		void OnBlinkDetected (string payload);

		void OnNewValue (string payload);
		#endregion
	}

}
	

