using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// The G-Effects modification for Kerbal Space Program
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
	/// </summary>

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class G_Effects : MonoBehaviour
	{
		
		//TODO turn off SAS on G-LOC
		//TODO simulate orientation loss on G-LOC
		
		readonly string APP_NAME = "G-Effects";
		readonly string CONTROL_LOCK_ID = "G_EFFECTS_LOCK";
		readonly int MAX_GLOC_FADE = 100;
		readonly double G_CONST = 9.81;
		
		Texture2D blackoutTexture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		Texture2D fillTexture = new Texture2D(1, 1);
		Color colorOut = new Color();
		Color colorFill = new Color();
		ProtoCrewMember commander = null;
		double downwardG;
		double forwardG;
		bool playEffects = true;
		bool paused = false;

		Configuration conf = new Configuration();
		GEffectsAudio gAudio = new GEffectsAudio();
		
		//This is for G effects persistance
		public Dictionary<string, KerbalGData> kerbalGDict = new Dictionary<string, KerbalGData>();
		
		protected void Start()
		{
			string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Effects/blackout.png";
			byte[] texture = File.ReadAllBytes(path);
			blackoutTexture.LoadImage(texture);

			// Hook into the rendering queue to draw the G effects
			RenderingManager.AddToPostDrawQueue(3, new Callback(drawGEffects));
			GameEvents.onGamePause.Add(onPause);
			GameEvents.onGameUnpause.Add(onUnPause);
			GameEvents.onCrewKilled.Add(onCrewKilled);
			GameEvents.onVesselChange.Add(onVesselChange);
			// Add another rendering queue hook for the GUI
			//RenderingManager.AddToPostDrawQueue(4, new Callback(drawGUI));
			
		}
		
		void onPause() {
			paused = true;
			gAudio.pauseAllSounds(paused);
		}
		
		void onUnPause() {
			paused = false;
			gAudio.pauseAllSounds(paused);
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
			kerbalGDict.Clear(); //kerbalGDict is for persistance, actually, but if not cleared the G effects on crew will be "frozen" on switch out/in vessel
			InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
		}
		
		protected void Awake() {
			conf.loadConfiguration(APP_NAME.ToUpper());
			
			if (!gAudio.isInitialized()) {
				gAudio.initialize(conf.gruntsVolume, conf.breathVolume, conf.heartBeatVolume, conf.femaleVoicePitch, conf.breathSoundPitch);
			}
			
		}
		
		public void Update() {
			
			if (paused) {
				return;
			}
			
			Vessel vessel = FlightGlobals.ActiveVessel;
			Part referencePart = vessel.GetReferenceTransformPart();
			
			//Return without any effect if the vessel isn't controlled by any crew
			if ((referencePart.CrewCapacity == 0) || (referencePart.protoModuleCrew.Count == 0)) {
				return;
			}
			
			if (!gAudio.IsBoundToTransform()) {
				gAudio.bindToTransform(FlightCamera.fetch.mainCamera.transform);
			}
			
			//Find a crew member that most likely is controlling the vessel. So he is the one who sees the effects.
			foreach (ProtoCrewMember crewMember in referencePart.protoModuleCrew) {
				if (crewMember.experienceTrait.Title.ToLower().Equals("pilot")) {
					commander = crewMember;
					break;
				}
			}
			if (commander == null) {
				commander = referencePart.protoModuleCrew.FirstOrDefault();
			}
			
			//Calcualte g-effects for each crew member
			foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()) {
				
				if ( (crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Dead)) || (crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Missing)) ) {
					if (crewMember.Equals(commander)) {
						playEffects = false;
					}
					continue;
				}
				
				playEffects = 
					crewMember.Equals(commander) &&
					(!conf.IVAOnly || (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA)) &&
					!MapView.MapIsEnabled;
				
				gAudio.SetAudioEnabled(playEffects);

				KerbalGData gData;
				if (!kerbalGDict.TryGetValue(crewMember.name, out gData)) {
					gData = new KerbalGData();
					kerbalGDict.Add(crewMember.name, gData);
				}
				
				//Calculate modifer by Kerbal individual charateristics
				float kerbalModifier = 1;
				conf.traitModifiers.TryGetValue(commander.experienceTrait.Title, out kerbalModifier);
				if (commander.gender == ProtoCrewMember.Gender.Female) {
					kerbalModifier *= conf.femaleModifier;
				}
				
				//Calculate G forces
				Vector3d gAcceleration = FlightGlobals.getGeeForceAtPosition(vessel.GetWorldPos3D()) - vessel.acceleration;
				Vector3d cabinAcceleration = vessel.transform.InverseTransformDirection(gAcceleration); //vessel.transform is an active part's transform
				downwardG = cabinAcceleration.z / G_CONST * (downwardG-1 > 0 ? conf.downwardGMultiplier : conf.upwardGMultiplier);
				forwardG = cabinAcceleration.y / G_CONST * (forwardG > 0 ? conf.forwardGMultiplier : conf.backwardGMultiplier);
				
				gData.cumulativeG -= Math.Sign(gData.cumulativeG) * conf.gResistance * kerbalModifier;
				
				if ((downwardG > conf.positiveThreshold) || (downwardG < conf.negativeThreshold) || (forwardG > conf.positiveThreshold) || (forwardG < conf.negativeThreshold)) {
					
					double rebCompensation = conf.gResistance * kerbalModifier - conf.deltaGTolerance * conf.deltaGTolerance / kerbalModifier; //this is calculated so the rebound is in equilibrium with cumulativeG at the very point of G threshold
					gData.cumulativeG += Math.Sign(downwardG-1+forwardG) * rebCompensation + (Math.Abs(downwardG-1)*(downwardG-1) + Math.Abs(forwardG) * forwardG) / kerbalModifier;
					
					gAudio.stopBreath();
					
					if (Math.Abs(gData.cumulativeG) > conf.GLOC_CUMULATIVE_G) {
						loseConscience(gData, crewMember.Equals(commander));
						gData.needBreath = 0;
						gAudio.stopHeartBeats();
					} else {
						//Positive and frontal G sound effects
						if( ((downwardG > conf.positiveThreshold) || (forwardG > conf.positiveThreshold)) && (gData.cumulativeG > 0.1 * conf.MAX_CUMULATIVE_G)) {
							gData.needBreath = (int)(2 + 4 * gData.cumulativeG / conf.MAX_CUMULATIVE_G);
							gAudio.playGrunt(commander.gender.Equals(ProtoCrewMember.Gender.Female), -1f /*(float)((Math.Max(Math.Max(downwardG, forwardG), 10.0 + conf.positiveThreshold) - conf.positiveThreshold) / 10.0)*/);
							//Negative G sound effects
						} else if (gData.cumulativeG < -0.1 * conf.MAX_CUMULATIVE_G) {
							if (gAudio.isHeartBeatsPlaying()) {
								gAudio.setHeartBeatsVolume( Math.Min((float)(Math.Abs(gData.cumulativeG + 0.1 * conf.MAX_CUMULATIVE_G) /(1 - 0.1) / conf.MAX_CUMULATIVE_G * conf.heartBeatVolume * GameSettings.VOICE_VOLUME), GameSettings.VOICE_VOLUME * conf.heartBeatVolume));
							} else {
								gAudio.playHeartBeats();
							}
						}
					}
					
				} else {
					//Breath back sound effect
					if ((gData.needBreath > 0) && (gData.gLocFadeAmount == 0)) {
						if (gAudio.tryPlayBreath(commander.gender.Equals(ProtoCrewMember.Gender.Female))) {
							gData.needBreath -= 1;
						}
					}
					if (Math.Abs(gData.cumulativeG) < 0.1 * conf.MAX_CUMULATIVE_G) {
						reboundConscience(gData, crewMember.Equals(commander));
					}
				}
				
				//If out of danger then stop negative G sound effects
				if (gData.cumulativeG > -0.3 * conf.MAX_CUMULATIVE_G) {
					gAudio.stopHeartBeats();
				}
			}
		}
		
		
		void loseConscience(KerbalGData kerbalGData, bool isCommander) {
			if (isCommander) {
				InputLockManager.SetControlLock(ControlTypes.ALL_SHIP_CONTROLS, CONTROL_LOCK_ID);
			}
			kerbalGData.gLocFadeAmount += conf.gLocFadeSpeed;
			if (kerbalGData.gLocFadeAmount > MAX_GLOC_FADE) {
				kerbalGData.gLocFadeAmount = MAX_GLOC_FADE;
			}
			if ( (conf.gLocScreenWarning != null) && (conf.gLocScreenWarning.Length > 0) ) {
				ScreenMessages.PostScreenMessage(conf.gLocScreenWarning);
			}
		}
		
		
		void reboundConscience(KerbalGData kerbalGData, bool isCommander) {
			if (isCommander) {
				kerbalGData.gLocFadeAmount -= conf.gLocFadeSpeed;
			}
			if (kerbalGData.gLocFadeAmount <= 0) {
				InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
				kerbalGData.gLocFadeAmount = 0;
			}
		}
		
		
		void drawGEffects()
		{
			if (!playEffects) {
				return;
			}
			
			KerbalGData kerbalGData;
			
			if ((commander == null) || !kerbalGDict.TryGetValue(commander.name, out kerbalGData)) {
				return;
			}
			
			//Apply positive or negative visual effect
			if (kerbalGData.cumulativeG > 0) {
				colorOut = Color.black;
				colorOut.a = (float)(Math.Abs(kerbalGData.cumulativeG) / conf.MAX_CUMULATIVE_G);
			} else {
				colorOut.r = conf.redoutRGB.r;
				colorOut.g = conf.redoutRGB.g;
				colorOut.b = conf.redoutRGB.b;
				colorOut.a = (float)(Math.Abs(kerbalGData.cumulativeG) / conf.MAX_CUMULATIVE_G);
			}
			
			//We'll need to draw an additional solid texture over the blackout for both intensification effect at the end and G-LOC fade in/out
			colorFill.r = colorOut.r;
			colorFill.g = colorOut.g;
			colorFill.b = colorOut.b;
			colorFill.a = (float)Math.Pow(colorOut.a, 8f); //this will intensify blackout/redout effect at the very end
			
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

	}
}
