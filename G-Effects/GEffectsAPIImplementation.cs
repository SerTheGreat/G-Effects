using System;
using System.Collections.Generic;

namespace G_Effects
{
	/// <summary>
	/// Class that provides implementations for G-Effects API calls
	/// </summary>
	public class GEffectsAPIImplementation
	{
		
		static GEffectsAPIImplementation inst; 
		
		Dictionary<string, KerbalGState> kerbalStates;
		
		private GEffectsAPIImplementation() { }
		
		public static GEffectsAPIImplementation instance() {
			if (inst == null) {
				inst = new GEffectsAPIImplementation();
			}
			return inst;
		}
		
		internal void setKerbalStates(Dictionary<string, KerbalGState> kerbalStates) {
			this.kerbalStates = kerbalStates;
		}
		
		KerbalGState getKerbalState(string kerbalName) {
			if (kerbalStates == null) {
				return null;
			}
			KerbalGState kerbalState;
			return kerbalStates.TryGetValue(kerbalName, out kerbalState) ? kerbalState : null;
		}
		
		public bool isKerbalGStateAvailable(string kerbalName) {
			return getKerbalState(kerbalName) != null;
		}
		
		public double? getDownwardG(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (double?)kerbalGState.downwardG : null;
		}
		
		public double? getForwardG(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (double?)kerbalGState.forwardG : null;
		}
		
		public bool? isAGSMStarted(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (bool?)kerbalGState.isAGSMStarted() : null;
		}
		
		public int? getBreathNeeded(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (int?)kerbalGState.getBreathNeeded() : null;
		}
		
		public bool? isGLocCondition(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (bool?)kerbalGState.isGLocCondition() : null;
		}
		
		public bool? isCriticalCondition(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (bool?)kerbalGState.isCriticalCondition() : null;
		}
		
		public bool? isDeathCondition(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (bool?)kerbalGState.isDeathCondition() : null;
		}
		
		public float? getSeverity(string kerbalName) {
			KerbalGState kerbalGState = getKerbalState(kerbalName);
			return kerbalGState != null ? (float?)kerbalGState.getSeverity() : null;
		}
		
	}
}
