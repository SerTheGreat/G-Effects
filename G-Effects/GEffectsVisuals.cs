using System;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// Handles visual part of G-effects
	/// </summary>
	public class GEffectsVisuals
	{
		
		Configuration conf;
		
		static Texture2D blackoutTexture = null;
		Texture2D intensifier = new Texture2D(1, 1);
		Texture2D gLocOverlay = new Texture2D(1, 1);
		Color visualsColor = new Color();
		Color intensifierColor = new Color();
		Color gLocColor = new Color();
		
		public GreyoutCameraFilter flightCameraFilter;
		public GreyoutCameraFilter internalCameraFilter;
		
		private GEffectsVisuals()
		{
		}
		
		public static GEffectsVisuals initialize(Configuration conf) {
			if (blackoutTexture == null) {
				blackoutTexture = GameDatabase.Instance.GetTexture("G-Effects/blackout", false);
			}
			GEffectsVisuals instance = new GEffectsVisuals();
			instance.conf = conf;
			instance.flightCameraFilter = GreyoutCameraFilter.initializeCameraFilter(FlightCamera.fetch.mainCamera, conf.mainCamGreyout);
			instance.internalCameraFilter = GreyoutCameraFilter.initializeCameraFilter(InternalCamera.Instance.GetComponent<Camera>(), conf.IVAGreyout);
			return instance;
		}
		
		public void doGreyout(KerbalGState gState) {
			if (gState.cumulativeG > 0) {
				float greyout = Mathf.Pow(Mathf.Clamp(gState.getSeverity() / 0.5f, 0f, 1.0f), 2); //Severity is divided by a percent of the total blackout at which greyout should be complete
				flightCameraFilter.setMagnitude(greyout);
				internalCameraFilter.setMagnitude(greyout);
			} else {
				flightCameraFilter.setMagnitude(0.0f);
				internalCameraFilter.setMagnitude(0.0f);
			}
		}
		
		public void disableCameraFilters() {
			flightCameraFilter.setBypass(true);
			internalCameraFilter.setBypass(true);
		}
		
		public void drawGEffects(KerbalGState kerbalGData)
		{
			float severity = kerbalGData.getSeverityWithThreshold(2.0f * (float)conf.gResistance);
			if (Math.Abs(severity) > 0.0001) {
				//Apply positive or negative visual effect
				if (kerbalGData.cumulativeG > 0) {
					visualsColor = Color.black;
					visualsColor.a = (float)(severity * 1.2);
				} else {
					visualsColor.r = conf.redoutRGB.r;
					visualsColor.g = conf.redoutRGB.g;
					visualsColor.b = conf.redoutRGB.b;
					visualsColor.a = (float)(severity * 1.2);
				}
			
				//We'll need to draw an additional solid texture over the blackout for intensification effect at the end
				intensifierColor.r = visualsColor.r;
				intensifierColor.g = visualsColor.g;
				intensifierColor.b = visualsColor.b;
				intensifierColor.a = (float)Math.Pow(severity, 4); //this will intensify blackout/redout effect at the very end
			
				intensifier.SetPixel(0, 0, intensifierColor);
				intensifier.Apply();

				GUI.color = visualsColor;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackoutTexture);
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), intensifier);
			}

			if (kerbalGData.gLocFadeAmount > 0) {
				//The following will fade out in overlay whatever is diplayed if losing consciousness or fade in on wake up
				gLocColor = Color.black;
				gLocColor.a = (float)((kerbalGData.gLocFadeAmount * kerbalGData.gLocFadeAmount) / (G_Effects.MAX_GLOC_FADE * G_Effects.MAX_GLOC_FADE));

				gLocOverlay.SetPixel(0, 0, gLocColor);
				gLocOverlay.Apply();

				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), gLocOverlay);
			}
		}
		
	}
}
