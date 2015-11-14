using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace G_Effects
{
	/// <summary>
	/// The G-Effects plugin for Kerbal Space Program
	/// 
	///Copyright (C) 2015 Ser
	///This program is free software; you can redistribute it and/or
	///modify it under the terms of the GNU General Public License
	///as published by the Free Software Foundation; either version 2
	///of the License, or (at your option) any later version.
	///
	///This program is distributed in the hope that it will be useful,
	///but WITHOUT ANY WARRANTY; without even the implied warranty of
	///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	///GNU General Public License for more details.
	/// 
	///You should have received a copy of the GNU General Public License
    ///along with this program.  If not, see <http://www.gnu.org/licenses/>.
	/// </summary>

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class G_Effects : MonoBehaviour
	{

		//TODO find a way to disable EVA button on G-LOC
		//TODO simulate orientation loss on G-LOC
		
		GEffectsAPIImplementation gEffectsApiImpl = GEffectsAPIImplementation.instance();
		KeepFit.KeepFitAPI keepFitAPI = new KeepFit.KeepFitAPI();
		GrayoutCameraFilter flightCameraFilter;
		GrayoutCameraFilter internalCameraFilter;
		
		const string APP_NAME = "G-Effects";
		const string CONTROL_LOCK_ID = "G_EFFECTS_LOCK";
		const int MAX_GLOC_FADE = 100;
		const double G_CONST = 9.81;
				
		static Texture2D blackoutTexture = null;//new Texture2D(32, 32, TextureFormat.ARGB32, false);
		static Texture2D fillTexture = new Texture2D(1, 1);
		Color colorOut = new Color();
		Color colorFill = new Color();
		ProtoCrewMember commander = null;
		double downwardG;
		double forwardG;
		bool playEffects = false;
		bool paused = false;
		readonly static PortraitAgent PORTRAIT_AGENT = new PortraitAgent();

		Configuration conf = new Configuration();
		GEffectsAudio gAudio = new GEffectsAudio();
		
		//Specialization priority to be a commander
		Dictionary<string, int> priorities = new Dictionary<string, int>() {
			{"pilot", 3}, {"engineer", 2}, {"scientist", 2}, {"tourist", 0}
		};
		
		//This is for G effects persistance
		Dictionary<string, KerbalGState> kerbalGDict = new Dictionary<string, KerbalGState>();
		
		protected void Start()
		{	
			gEffectsApiImpl.setKerbalStates(kerbalGDict);
			if (keepFitAPI.initialize()) {
				writeLog("KeepFit mod detected. Working in conjunction.");
			}
			/*string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Effects/blackout.png";
			byte[] texture = File.ReadAllBytes(path);
			blackoutTexture.LoadImage(texture);*/
			if (blackoutTexture == null) {
				blackoutTexture = GameDatabase.Instance.GetTexture("G-Effects/blackout", false);
			}

			// Hook into the rendering queue to draw the G effects
			RenderingManager.AddToPostDrawQueue(3, new Callback(drawGEffects));
			GameEvents.onGamePause.Add(onPause);
			GameEvents.onGameUnpause.Add(onUnPause);
			GameEvents.onCrewKilled.Add(onCrewKilled);
			GameEvents.onVesselChange.Add(onVesselChange);
			// Add another rendering queue hook for the GUI
			//RenderingManager.AddToPostDrawQueue(4, new Callback(drawGUI));
			PORTRAIT_AGENT.Start();
		}
		
		protected void OnDestroy() {
			RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGEffects));
			GameEvents.onGamePause.Remove(onPause);
			GameEvents.onGameUnpause.Remove(onUnPause);
			GameEvents.onCrewKilled.Remove(onCrewKilled);
			GameEvents.onVesselChange.Remove(onVesselChange);
			PORTRAIT_AGENT.OnDestroy();
		}
		
		protected void LateUpdate() {
			PORTRAIT_AGENT.LateUpdate();
		}
		
		protected void FixedUpdate() {
			if (InputLockManager.GetControlLock(CONTROL_LOCK_ID) != ControlTypes.None) {
				FlightGlobals.ActiveVessel.ctrlState.NeutralizeStick();
			}
		}
		
		void onPause() {
			gAudio.pauseAllSounds(true);
		}
		
		void onUnPause() {
			gAudio.pauseAllSounds(false);
		}
		
		void onCrewKilled(EventReport eventReport) {
			resetValues();
			gAudio.stopAllSounds();
		}
		
		void onVesselChange(Vessel vessel) {
			resetValues();
			gAudio.stopAllSounds();
		}
		
		void resetValues() {
			commander = null;
			kerbalGDict.Clear(); //kerbalGDict is for persistence actually but if not cleared the G effects on crew will be "frozen" on switch out/in vessel
			InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
			PORTRAIT_AGENT.enableText(false);
			flightCameraFilter.setBypass(true);
			internalCameraFilter.setBypass(true);
		}
		
		GrayoutCameraFilter initializeCameraFilter(Camera camera) {
			GrayoutCameraFilter filter = camera.gameObject.GetComponent<GrayoutCameraFilter>();
			if (filter == null) {
				filter = camera.gameObject.AddComponent<GrayoutCameraFilter>();
			}
			return filter;
		}
		
		protected void Awake() {
			conf.loadConfiguration(APP_NAME.ToUpper());
			
			if (!gAudio.isInitialized()) {
				gAudio.initialize(conf.gruntsVolume, conf.breathVolume, conf.heartBeatVolume, conf.femaleVoicePitch, conf.breathSoundPitch);
			}
			
			flightCameraFilter = initializeCameraFilter(FlightCamera.fetch.mainCamera);
			internalCameraFilter = initializeCameraFilter(InternalCamera.Instance.camera);
		}
		
		public void Update() {

			if (paused) {
				return;
			}
			
			Vessel vessel = FlightGlobals.ActiveVessel;
			Part referencePart = vessel.GetReferenceTransformPart();
			
			commander = null; //the commander is recalculated every update
			playEffects = false; //this changes to true later if all necessary conditions are met
			
			//Return without any effect if the vessel hasn't any crew
			if ((vessel.GetCrewCount() == 0)) {
				return;
			}
			
			if (!gAudio.isBoundToTransform()) {
				gAudio.bindToTransform(FlightCamera.fetch.mainCamera.transform);
			}
			
			if (vessel.isEVA) {
				commander = vessel.GetVesselCrew()[0];
			} else if (referencePart.CrewCapacity > 0) { //otherwise the vessel is controlled via probe core
				//Find a crew member that most likely is controlling the vessel. So he is the one who sees the effects.
				foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()) {
					if (crewMember.seat.part.isControlSource) {
						commander = bestCommander(commander, crewMember);
					}
				}
			}
			if (commander == null) { //if there's still no commander in the vessel then control lock must be removed because it is probably a probe core that has control at the moment
				InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
			}
			
			//Calcualte g-effects for each crew member
			foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()) {

				if ( isRosterDead(crewMember)) {
					continue;
				}
				
				bool isIVA = CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA;

				playEffects = 
					crewMember.Equals(commander) &&
					(!conf.IVAOnly || isIVA) &&
					!MapView.MapIsEnabled;
				
				flightCameraFilter.setBypass(isIVA || !playEffects);
				internalCameraFilter.setBypass(!isIVA || !playEffects);
				gAudio.setAudioEnabled(playEffects);
				PORTRAIT_AGENT.enableText(!playEffects);
				KerbalGState gState;
				if (!kerbalGDict.TryGetValue(crewMember.name, out gState)) {
					gState = new KerbalGState(conf);
					kerbalGDict.Add(crewMember.name, gState);
				}
				//Calculate modifer by Kerbal individual charateristics
				float kerbalModifier = 1;
				conf.traitModifiers.TryGetValue(crewMember.experienceTrait.Title, out kerbalModifier);
				if (crewMember.gender == ProtoCrewMember.Gender.Female) {
					kerbalModifier *= conf.femaleModifier;
				}
				float? keepFitFitnessModifier = keepFitAPI.getFitnessGeeToleranceModifier(crewMember.name);
				if (keepFitFitnessModifier != null) {
					kerbalModifier *= (float)keepFitFitnessModifier;
				}
				
				//Calculate G forces
				Vector3d gAcceleration = FlightGlobals.getGeeForceAtPosition(vessel.GetWorldPos3D()) - vessel.acceleration;
				Vector3d cabinAcceleration = vessel.transform.InverseTransformDirection(gAcceleration); //vessel.transform is an active part's transform
				writeDebug("crew=" + crewMember.name + " cabinAcceleration=" + cabinAcceleration);
				cabinAcceleration = dampAcceleration(cabinAcceleration, gState.previousAcceleration);
				gState.previousAcceleration = cabinAcceleration;
				gState.downwardG = cabinAcceleration.z / G_CONST; //These are true G values
				gState.forwardG = cabinAcceleration.y / G_CONST;
				downwardG = gState.downwardG * (gState.downwardG-1 > 0 ? conf.downwardGMultiplier : conf.upwardGMultiplier); //These are modified G values for usage in further calculations
				forwardG = gState.forwardG * (gState.forwardG > 0 ? conf.forwardGMultiplier : conf.backwardGMultiplier);
				
				gState.cumulativeG -= Math.Sign(gState.cumulativeG) * conf.gResistance * kerbalModifier;
				//gAudio.applyFilter(1 - Mathf.Clamp01((float)(1.25 * Math.Pow(Math.Abs(gData.cumulativeG) / conf.MAX_CUMULATIVE_G, 2) - 0.2)));
				doGrayout(gState);
				if ((downwardG > conf.positiveThreshold) || (downwardG < conf.negativeThreshold) || (forwardG > conf.positiveThreshold) || (forwardG < conf.negativeThreshold)) {
					
					double rebCompensation = conf.gResistance * kerbalModifier - conf.deltaGTolerance * conf.deltaGTolerance / kerbalModifier; //this is calculated so the rebound is in equilibrium with cumulativeG at the very point of G threshold
					gState.cumulativeG += Math.Sign(downwardG-1+forwardG) * rebCompensation + (Math.Abs(downwardG-1)*(downwardG-1) + Math.Abs(forwardG) * forwardG) / kerbalModifier;
					
					gAudio.stopBreath();
					gState.resetBreath();
					
					if (gState.isGLocCondition()) {
						loseConsciousness(crewMember, gState, crewMember.Equals(commander), playEffects);
						gAudio.stopHeartBeats();
					} else {
						//Positive and frontal G sound effects
						if( ((downwardG > conf.positiveThreshold) || (forwardG > conf.positiveThreshold)) && (gState.cumulativeG > 0.1 * conf.MAX_CUMULATIVE_G)) {
							//gData.needBreath = Mathf.Max(gData.needBreath, (gData.cumulativeG > 0.6 * conf.MAX_CUMULATIVE_G) ? (int)(MAX_BREATHS * gData.getSeverity() - 1) : 0);
							gState.startAGSM(Planetarium.GetUniversalTime());
							gAudio.playGrunt(commander.gender.Equals(ProtoCrewMember.Gender.Female), -1f /*(float)((Math.Max(Math.Max(downwardG, forwardG), 10.0 + conf.positiveThreshold) - conf.positiveThreshold) / 10.0)*/);
							//Negative G sound effects
						} else if (gState.cumulativeG < -0.1 * conf.MAX_CUMULATIVE_G) {
							if (gAudio.isHeartBeatsPlaying()) {
								gAudio.setHeartBeatsVolume(Math.Min((float)(2 * Math.Abs(gState.cumulativeG + 0.1 * conf.MAX_CUMULATIVE_G) /(1 - 0.1) / conf.MAX_CUMULATIVE_G * conf.heartBeatVolume * GameSettings.VOICE_VOLUME), GameSettings.VOICE_VOLUME * conf.heartBeatVolume));
							} else {
								gAudio.playHeartBeats();
							}
						}
					}
					
				} else {
					//Breath back sound effect
					gState.stopAGSM(Planetarium.GetUniversalTime());
					int breathNeeded = gState.getBreathNeeded();
					if ((breathNeeded > 0) && (gState.gLocFadeAmount == 0)) {
						if (gAudio.tryPlayBreath(commander.gender.Equals(ProtoCrewMember.Gender.Female),
						                         UnityEngine.Random.Range((float)Mathf.Clamp(breathNeeded - 2, 1, conf.maxBreaths), (float)breathNeeded) / (float)conf.maxBreaths * conf.breathVolume * GameSettings.VOICE_VOLUME,
						                         1f - 0.2f * (1 - (float)breathNeeded / (float)conf.maxBreaths))) {
							gState.takeBreath();
						}
					}
					if (Math.Abs(gState.cumulativeG) < 0.1 * conf.MAX_CUMULATIVE_G) {
						reboundConsciousness(crewMember, gState, crewMember.Equals(commander));
					}
				}
				
				gAudio.applyFilters(1 - gState.gLocFadeAmount / MAX_GLOC_FADE);
				writeDebug("crew=" + crewMember.name + " cumulativeG=" + gState.cumulativeG);
				
				//If out of danger then stop negative G sound effects
				if (gState.cumulativeG > -0.3 * conf.MAX_CUMULATIVE_G) {
					gAudio.stopHeartBeats();
				}

				//Display current health status on portrait or die the crew member
				if (gState.isCriticalCondition()) {
					PORTRAIT_AGENT.setKerbalPortraitText(crewMember.name, "CRITICAL\nCONDITION", 0, 0, new Color(1f, 0f, 0f, 1), 4, 2);
				} else if (gState.isGLocCondition()) {
					PORTRAIT_AGENT.setKerbalPortraitText(crewMember.name, "RESPONSE\nMINIMAL",  ((int)(Planetarium.GetUniversalTime() * 10) % 4 - 2), 0, new Color(1f, 1f, 0f, 1), 0, 1);
				} else {
					PORTRAIT_AGENT.disableKerbalPortraitText(crewMember.name);					
				}
				if (gState.isDeathCondition()) {
					crewMember.Die();
					PORTRAIT_AGENT.disableKerbalPortraitText(crewMember.name);
					kerbalGDict.Remove(crewMember.name);
				}
			}
		}
		
		//Damps acceleration peak if it is detected for the current frame.
		//Most likely the peaks are caused by imperfect physics and need to be damped for not causing unnatural effects on crew.
		//(Acceleration of a kerbal going EVA is 572G)
		Vector3d dampAcceleration(Vector3d current_acc, Vector3d prev_acc) {
			double magnitude = (current_acc - prev_acc).magnitude;
			if ((current_acc - prev_acc).magnitude > conf.gDampingThreshold * G_CONST) {
				writeLog("Peak detected. G="+((current_acc - prev_acc).magnitude / G_CONST) + " threshold="+conf.gDampingThreshold);
				return prev_acc;
			} else {
				return current_acc;
			}
		}
		
		//Determines who among of the two crew members is most likely in command 
		ProtoCrewMember bestCommander(ProtoCrewMember current, ProtoCrewMember candidate) {
			if (current == null) {
				return candidate;
			} else if (candidate == null) {
				return current;
			}
			//None of crewMemebers are null further
			
			int current_pr = -1;
			int candidate_pr = -1;
			
			priorities.TryGetValue(current.experienceTrait.Title.ToLower(), out current_pr);
			priorities.TryGetValue(candidate.experienceTrait.Title.ToLower(), out candidate_pr);
			if (current_pr > candidate_pr) { //return the one with the highest priority
				return current;
			} else if (candidate_pr > current_pr) {
				return candidate;
			} else { //or the one with the highest experience
				if (candidate.experienceLevel > current.experienceLevel) {
					return candidate;
				} else {
					return current;
				}
			}
		}
		
		void doGrayout(KerbalGState gState) {
			float grayout = Mathf.Clamp(2 * gState.getSeverity(), 0f, 1.0f);
			if (gState.cumulativeG > 0) {
				flightCameraFilter.setMagnitude(grayout);
				internalCameraFilter.setMagnitude(grayout);
			} else {
				flightCameraFilter.setMagnitude(0.0f);
				internalCameraFilter.setMagnitude(0.0f);
			}
		}
		
		void loseConsciousness(ProtoCrewMember crewMember, KerbalGState kerbalGData, bool isCommander, bool outputAllowed) {
			kerbalGData.stopAGSM(0);
			kerbalGData.resetBreath();
			kerbalGData.gLocFadeAmount += conf.gLocFadeSpeed;
			if (kerbalGData.gLocFadeAmount > MAX_GLOC_FADE) {
				kerbalGData.gLocFadeAmount = MAX_GLOC_FADE;
				if (isCommander) {
					Vessel vessel = crewMember.KerbalRef.InVessel;
					if (!hasProbeCore(vessel)) {
					    vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
					}
					InputLockManager.SetControlLock(ControlTypes.ALL_SHIP_CONTROLS, CONTROL_LOCK_ID);
				}
				if ( outputAllowed && (conf.gLocScreenWarning != null) && (conf.gLocScreenWarning.Length > 0) ) {
					ScreenMessages.PostScreenMessage(conf.gLocScreenWarning);
				}
			}
		}
		
		
		void reboundConsciousness(ProtoCrewMember crewMember, KerbalGState kerbalGData, bool isCommander) {
			kerbalGData.gLocFadeAmount -= conf.gLocFadeSpeed;
			if (kerbalGData.gLocFadeAmount <= 0) {
				kerbalGData.gLocFadeAmount = 0;
				if (isCommander) {
					InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
				}
			}
		}
		
		bool hasProbeCore(Vessel vessel) {
			foreach (Part part in vessel.Parts) {
				if (part.isControlSource && (part.CrewCapacity == 0))
					return true;
			}
			return false;
		}
		
		bool isRosterDead(ProtoCrewMember crewMember) {
			return crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Dead) || crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Missing);
		}
		
		void drawGEffects()
		{
			if (!playEffects) {
				return;
			}
			
			KerbalGState kerbalGData;
			if ((commander == null) || !kerbalGDict.TryGetValue(commander.name, out kerbalGData)) {
				return;
			}
			
			double severity = kerbalGData.getSeverity();
			//Apply positive or negative visual effect
			if (kerbalGData.cumulativeG > 0) {
				colorOut = Color.black;
				colorOut.a = (float)(severity * 1.2);
			} else {
				colorOut.r = conf.redoutRGB.r;
				colorOut.g = conf.redoutRGB.g;
				colorOut.b = conf.redoutRGB.b;
				colorOut.a = (float)(severity * 1.2);
			}
			
			//We'll need to draw an additional solid texture over the blackout for both intensification effect at the end and G-LOC fade in/out
			colorFill.r = colorOut.r;
			colorFill.g = colorOut.g;
			colorFill.b = colorOut.b;
			colorFill.a = (float)Math.Pow(severity, kerbalGData.cumulativeG > 0 ? 8 : 4); //this will intensify blackout/redout effect at the very end
			
			//The following will fade out in overlay whatever is diplayed if losing consciousness or fade in on wake up
			float fade = (float)(kerbalGData.gLocFadeAmount * kerbalGData.gLocFadeAmount) / (float)(MAX_GLOC_FADE * MAX_GLOC_FADE);
			colorFill.r -= fade;
			colorFill.g -= fade;
			colorFill.b -= fade;
			colorFill.a += fade;
			
			fillTexture.SetPixel(0, 0, colorFill);
			fillTexture.Apply();
			
			//if (FlightGlobals.ActiveVessel.horizontalSrfSpeed > 50 & FlightGlobals.ActiveVessel.situation != Vessel.Situations.LANDED) // Dirty hack to stop the 'drunk prograde marker' causing weird G effects when stopped
			{
				GUI.color = colorOut;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackoutTexture);
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fillTexture);
			}
		}
		
		void writeDebug(string text) {
			if (conf.enableLogging) {
				writeLog(text);
			}
		}
		
		void writeLog(string text) {
				KSPLog.print(APP_NAME + ": " + text);
		}

	}
}
