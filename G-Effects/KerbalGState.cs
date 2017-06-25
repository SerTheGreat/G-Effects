using System;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// Holds personal values of G effects per kerbal
	/// </summary>
	public class KerbalGState
	{
		
		/// This is the cumulative effect of G's which is increasingly negative to represent a redout condition 
        /// and increasingly positive to represent a blackout condition. As G forces continue, the cumulativeG
        /// value is increased, by larger amounts for higher current G levels, to allow for longer periods of G
        /// force tolerance at lower G levels.
        public double cumulativeG = 0;
        public double downwardG = 0;
        public double forwardG = 0;
        public Vector3d previousAcceleration = Vector3d.zero; //this is used to track sudden G peaks caused by the imperfectness of physics
		public double gLocFadeAmount = 0;
		
		private int breathNeeded = 0;
		private double agsmStart = -1;
	
		private Configuration configuration;
		
		public KerbalGState(Configuration configuration) {
			this.configuration = configuration;			
		}
		
		public void startAGSM(double time) {
			if (!isAGSMStarted()) {
				breathNeeded = 0;
				agsmStart = time;
			}
		}
		
		public void stopAGSM(double time) {
			if (isAGSMStarted()) {
				double deltaTime = getSeverity() * (time - agsmStart);
				if (deltaTime > configuration.breathThresholdTime) {
					breathNeeded = (int)Mathf.Clamp((float)(2 + (deltaTime - configuration.breathThresholdTime) / 2), (float)configuration.minBreaths, (float)configuration.maxBreaths);
				} else {
					breathNeeded = 0;
				}
				agsmStart = -1;
			}
		}
		
		public bool isAGSMStarted() {
			return agsmStart > 0;			
		}
		
		public int getBreathNeeded() {
			return breathNeeded;
		}
		
		public int takeBreath() {
			return breathNeeded > 0 ? breathNeeded-- : 0;
		}
		
		public void resetBreath() {
			breathNeeded = 0;
		}
		
		public bool isGLocCondition() {
			return Math.Abs(cumulativeG) > configuration.GLOC_CUMULATIVE_G;			
		}
		
		public bool isCriticalCondition() {
			return configuration.gDeathEnabled && (Math.Abs(cumulativeG) > (configuration.gLocStartCoeff + 0.8 * (configuration.gDeathCoeff - configuration.gLocStartCoeff)) * configuration.MAX_CUMULATIVE_G);
		}
		
		public bool isDeathCondition() {
			return configuration.gDeathEnabled && (Math.Abs(cumulativeG) > configuration.gDeathCoeff * configuration.MAX_CUMULATIVE_G);
		}
		
		public float getSeverity() {
			return (float)(Math.Abs(cumulativeG) / configuration.MAX_CUMULATIVE_G);
		}

		public float getSeverityWithThreshold(float threshold) {
			float abs = (float)Math.Abs(cumulativeG);
			return (float)(Mathf.Clamp(abs - threshold, 0, abs) / (configuration.MAX_CUMULATIVE_G - threshold));
		}

	}
}
