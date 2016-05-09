using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace G_Effects
{
	/// <summary>
	/// Handles all audio initialization and playback
	/// </summary>
	public class GEffectsAudio {
		
		bool DONT_USE_FILTERS = true;
		
		float gruntsVolume = 0;
		float breathVolume = 0;
		
		GameObject audioPlayer = new GameObject();
		AudioClip[] gruntClips;
		AudioClip heartClip;
		AudioClip[] breathClips;
        //These will be left null if disabled in config (volume set to 0)
        AudioSource gruntAudio = null;
        AudioSource breathAudio = null;
        AudioSource heartAudio = null;
       	double gruntTimer = 0;
        
        bool heartBeatsPlaying = false;
        float femaleVoicePitch = 1.3f;
        float breathSoundPitch = 1.8f;
        bool initialized = false;
        bool audioEnabled = true;
        bool boundToTransform = false;
        
        //This dictionary is used to remember the used audio filters with "true" marking the ones added by this mod   
        Dictionary<AudioLowPassFilter, bool> filters = new Dictionary<AudioLowPassFilter, bool>();
        
		public void initialize(float gruntsVolume, float breathVolume, float heartBeatsVolume, float femaleVoicePitch, float breathSoundPitch) {
			this.breathSoundPitch = breathSoundPitch;
        	this.femaleVoicePitch = femaleVoicePitch;
			this.gruntsVolume = gruntsVolume;
			this.breathVolume = breathVolume;
			
        	if (gruntsVolume > 0.001f) {
				
        		gruntClips = new AudioClip[] {
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_01"),
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_02"),
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_03"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_04"),
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_05"),
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_06"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_07"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_08"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_09"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_10"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_11"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_12"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_13"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_14"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_15"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_16"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_17"),
        			//GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_18"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_19"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_20"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_21"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_22"),
        			GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/grunts/grunt_23")        			
        		};
				
        		gruntAudio = audioPlayer.AddComponent<AudioSource>();
        		gruntAudio.bypassListenerEffects = true;
        		gruntAudio.priority = 20;
        		gruntAudio.volume = GameSettings.VOICE_VOLUME * gruntsVolume;
        		gruntAudio.dopplerLevel = 0;
        		gruntAudio.bypassEffects = true;
        		gruntAudio.loop = false;
        		gruntAudio.rolloffMode = AudioRolloffMode.Linear;
			}
        		        		
        	if (breathVolume > 0.001f) {
        			
        		breathClips = new AudioClip[] { 
					GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/breaths/breath1"),
					GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/breaths/breath2"),
					GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/breaths/breath3"),
					GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/breaths/breath4")
    			};
        			
        		breathAudio = audioPlayer.AddComponent<AudioSource>();
        		breathAudio.bypassListenerEffects = true;
        		breathAudio.volume = GameSettings.VOICE_VOLUME * breathVolume;
        		breathAudio.dopplerLevel = 0;
        		breathAudio.bypassEffects = true;
        		breathAudio.loop = false;
        		breathAudio.rolloffMode = AudioRolloffMode.Linear;
        		breathAudio.pitch = breathSoundPitch;
        	}
			
        	if (heartBeatsVolume > 0.001f) {
				heartClip = GameDatabase.Instance.GetAudioClip("G-Effects/Sounds/heart-beat");
        		heartAudio = audioPlayer.AddComponent<AudioSource>();
        		heartAudio.bypassListenerEffects = true;
        		heartAudio.priority = 10;
        		heartAudio.volume = GameSettings.VOICE_VOLUME * heartBeatsVolume;
        		heartAudio.dopplerLevel = 0;
        		heartAudio.bypassEffects = true;
        		heartAudio.rolloffMode = AudioRolloffMode.Linear;
        		heartAudio.loop = false;
        	}
			
			initialized = true;
		}
        
        public bool isInitialized() {
        	return initialized;
        }
        
        public void bindToTransform(Transform transform) {
        	audioPlayer.transform.SetParent(transform);
        	boundToTransform = true;
        }
        
        public bool isBoundToTransform() {
        	return boundToTransform;
        }
        
        public bool isAudioEnabled() {
        	return audioEnabled;
        }
        
        public void setAudioEnabled(bool enable) {
        	audioEnabled = enable;
        	if (!audioEnabled) {
        		stopAllSounds();
        		clearAllSounds();
        		removeFilters();
        	}
        }
        
        public void applyFilters(float percent) {
        	if (DONT_USE_FILTERS) {
        		return;
        	}
        	if (!audioEnabled) {
        		return;
        	}
        	AudioListener[] listeners = MonoBehaviour.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
        	foreach (AudioListener listener in listeners) {
        		AudioLowPassFilter filter = listener.gameObject.GetComponent<AudioLowPassFilter>();
        		if (filter == null) {
        			filter = listener.gameObject.AddComponent<AudioLowPassFilter>();
        			filters.Add(filter, true);
        		} else {
        			if (!filters.ContainsKey(filter)) {
        				filters.Add(filter, false);
        			}
        		}
        		filter.cutoffFrequency = 22000 * percent; 
				filter.lowpassResonanceQ = 1;
        	}
        }
       
        public void removeFilters() {
        	if (DONT_USE_FILTERS) {
        		return;
        	}
        	foreach (AudioLowPassFilter filter in filters.Keys) {
        		bool ownFilter = false;
        		//Destroying only our own filters setting others to nominal value
        		if (filters.TryGetValue(filter, out ownFilter)) {
        			MonoBehaviour.Destroy(filter);
        		} else {
        			filter.cutoffFrequency = 22000;
        		}
        	}
        	filters.Clear();
        }
        
        public int getGruntsCount() {
        	return gruntClips.Length;
        }
        
        public void stopAllSounds() {
        	stopGrunt();
        	stopBreath();
        	stopHeartBeats();
        }
        
        public void clearAllSounds() {
        	clearAudio(gruntAudio);
        	clearAudio(breathAudio);
        	clearAudio(heartAudio);
        }
        
        public void pauseAllSounds(bool pause) {
        	pauseGrunts(pause);
        	pauseBreath(pause);
        	pauseHeartBeats(pause);
        }
        
        public bool isHeartBeatsPlaying() {
        	return heartBeatsPlaying;
        }
        
        public void playGrunt(bool female, float severity) {
        	int range = severity < 0 ? getGruntsCount() : 3; //severity  < 0 means don't use severity based selection
        	double rnd = UnityEngine.Random.Range(7f, 30f);
        	if (audioEnabled && (gruntAudio != null) && (Planetarium.GetUniversalTime() - gruntTimer > rnd)) {
        		gruntTimer = Planetarium.GetUniversalTime();
        		int index = (int)((getGruntsCount() - range) * severity) + UnityEngine.Random.Range(0, range);
        		playAudio(gruntAudio, gruntClips[index], GameSettings.VOICE_VOLUME * gruntsVolume, (female ? femaleVoicePitch : 1.0f));
        	}
        }
        
        public void pauseGrunts(bool pause) {
        	pauseAudio(gruntAudio, pause);
        }
        
        public void stopGrunt() {
        	stopAudio(gruntAudio);
        }
        
        public void clearAudio(AudioSource audio) {
        	if (audio != null) {
        		audio.clip = null;
        	}
        }
        
        public void playHeartBeats() {
        	if (!heartBeatsPlaying) {
        		playLoopedAudio(heartAudio, heartClip);
        		heartBeatsPlaying = true;
        	}
        }
        public void setHeartBeatsVolume(float volume) {
        	if ((heartAudio != null) && heartBeatsPlaying) {
        		heartAudio.volume = GameSettings.VOICE_VOLUME * volume;
        	}
        }
        
        public void pauseHeartBeats(bool pause) {
        	pauseAudio(heartAudio, pause);
        }
        
        public void stopHeartBeats() {
        	stopLoopedAudio(heartAudio);
        	heartBeatsPlaying = false;
        }
        
        //Plays breath sound if it is not playing yet and returns true on success to count successfully played clips
        public bool tryPlayBreath(bool female, float volume, float c) {
        	if ((breathAudio != null) && !breathAudio.isPlaying && ((gruntAudio == null) || (!gruntAudio.isPlaying))) {
        		playAudio(breathAudio, breathClips[UnityEngine.Random.Range(0, breathClips.Length-1)], volume, breathSoundPitch * c); /*female ? BREATH_PITCH * femaleVoicePitch : BREATH_PITCH*/
      	    	return true;
        	}
        	return false;
        }
        
        public void pauseBreath(bool pause) {
        	pauseAudio(breathAudio, pause);
        }
        
        public void stopBreath() {
        	stopAudio(breathAudio);
        }
        
        public void playAudio(AudioSource audio, AudioClip clip, float volume, float pitch) {
        	if (audioEnabled && (audio != null)) {
        		audio.volume = volume;
        		audio.pitch = pitch;
        		audio.clip = clip;
        		audio.Play();
        	}
        }
        
        public void pauseAudio(AudioSource audio, bool pause) {
        	if (audio != null) {
        		if (pause) {
        			audio.Pause();
        		} else {
        			audio.Pause();
        		}
        	}
        }
        
        public void stopAudio(AudioSource audio) {
        	if (audio != null) {
        		audio.Stop();
        		audio.clip = null; //This is neccessary because KSP seems to play all of the loaded sounds when switching to the map view and back after the load of a save game (seems to be a KSP bug)
        	}
        }
        
        public void muteAudio(AudioSource audio, bool mute) {
        	if (audio != null) {
        		audio.mute = mute;
        	}
        }
        
        public void playLoopedAudio(AudioSource audio, AudioClip clip) {
        	if (audioEnabled && (audio != null) && !audio.isPlaying) {
	       		audio.clip = clip;
        		audio.loop = true;
        		audio.Play();
        	}
        }
        
        public void stopLoopedAudio(AudioSource audio) {
			if (audio != null) {
        		audio.Stop();
        		audio.clip = null;
        		audio.loop = false;
        	}        	
        }
		
	}
}
