//Source code of this class is derived from PortraitStats mod by DMagic according to its MIT license.
//Since it was modified please address all your questions relative to its function in the present shape to G-Effects mod author.

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace G_Effects
{

	public class PortraitAgent
	{

		static readonly int LINE_HEIGHT = 18;
		
		private Dictionary<string, PortraitText> currentCrew = new Dictionary<string, PortraitText>();
		private List<PortraitText> activeCrew = new List<PortraitText>();
		private bool reload;
		private int index;
		private Vector2 screenPos = new Vector2();
		private KerbalGUIManager manager;
		private static Texture2D blackTexture = null;
		private static GUIStyle centeredLabelStyle = null;
		private bool textEnabled = false;
		
		internal void Start()
		{
			if (blackTexture == null) {
				blackTexture = GameDatabase.Instance.GetTexture("G-Effects/blackportrait", false);
			}

			GameEvents.onVesselWasModified.Add(vesselCheck);
			GameEvents.onVesselChange.Add(vesselCheck);
			manager = findKerbalGUIManager();

			if (manager == null) {
				OnDestroy();
			}

			reload = true;
			RenderingManager.AddToPostDrawQueue(5, drawLabels);
		}
		internal void OnDestroy()
		{
			GameEvents.onVesselWasModified.Remove(vesselCheck);
			GameEvents.onVesselChange.Remove(vesselCheck);

			RenderingManager.RemoveFromPostDrawQueue(5, drawLabels);
		}
		
		internal void enableText(bool enable) {
			textEnabled = enable;			
		}
		
		internal void setKerbalPortraitText(string kerbalName, string text, int leftOffest, int topOffset, Color textColor, int blinkFreq, int alphaBlendMode) {
			PortraitText portraitText;
			if (currentCrew.ContainsKey(kerbalName)) {
				portraitText = currentCrew[kerbalName];
			} else {
				portraitText = new PortraitText();
				currentCrew.Add(kerbalName, portraitText);
			}
			portraitText.text = text;
			portraitText.leftOffset = leftOffest;
			portraitText.topOffset = topOffset;
			portraitText.textColor = textColor;
			portraitText.blinkFreq = blinkFreq;
			portraitText.alphaBlendMode = alphaBlendMode;
			portraitText.drawtext = true;
		}
		
		internal void disableKerbalPortraitText(string kerbalName) {
			if (currentCrew.ContainsKey(kerbalName)) {
				currentCrew[kerbalName].drawtext = false;
			}
		}

		internal void LateUpdate()
		{
			if (!FlightGlobals.ready)
				return;

			if (KerbalGUIManager.ActiveCrew.Count <= 0)
				return;
			
			if (reload)
			{
				foreach (Kerbal k in KerbalGUIManager.ActiveCrew)
				{
					if (currentCrew.ContainsKey(k.name))
						continue;

					currentCrew.Add(k.name, new PortraitText());
				}

				float button = KerbalGUIManager.ActiveCrew.Count > 3 ? 27 : -1;

				screenPos.x = Screen.width - manager.AvatarSpacing - manager.AvatarSize - button;
				screenPos.y = Screen.height - manager.AvatarSpacing - manager.AvatarSize - manager.AvatarTextSize;

				index = int.MaxValue;
				reload = false;
			}
			
			if (index != manager.startIndex)
			{
				index = manager.startIndex;

				activeCrew.Clear();
			
				for (int i = index + 2; i >= index; i--)
				{
					if (i >= KerbalGUIManager.ActiveCrew.Count)
						continue;
					
					string name = KerbalGUIManager.ActiveCrew[i].name;
					if (currentCrew.ContainsKey(name))
					{
						activeCrew.Add(currentCrew[name]);
					}
				}
			}
			
		}

		private void drawLabels()
		{
			if (!textEnabled) {
				return;
			}
			
			int crewCount = KerbalGUIManager.ActiveCrew.Count;

			if (crewCount <= 0)
				return;

			switch (CameraManager.Instance.currentCameraMode)
			{
				case CameraManager.CameraMode.Map:
				case CameraManager.CameraMode.Internal:
				case CameraManager.CameraMode.IVA:
					return;
			}

			Color oldColor = GUI.color;
			
			if (centeredLabelStyle == null) {
				centeredLabelStyle = new GUIStyle(GUI.skin.label);
	    		centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			}

			for(int i = 0; i < activeCrew.Count; i++)
			{
				PortraitText pText = activeCrew[i];
				if (!pText.drawtext) {
					continue;
				}
				
				float leftOffset;

				/* This lovely bit of nonsense is due to the fact that KSP orders the crew portraits
				 * differently based on how many Kerbals are present. Crews with 2 or 3 Kerbals require
				 * special cases...
				 */
				if (crewCount == 2)
					leftOffset = screenPos.x - ((i == 0 ? 1 : 0) * (manager.AvatarSpacing + manager.AvatarSize));
				else if (crewCount == 3)
				{
					int j = i;
					if (j == 1)
						j = 2;
					else if (j == 2)
						j = 1;
					leftOffset = screenPos.x - (j * (manager.AvatarSpacing + manager.AvatarSize));
				}
				else
					leftOffset = screenPos.x - (i * (manager.AvatarSpacing + manager.AvatarSize));
				
				if (pText.alphaBlendMode > 0) {
					if (pText.alphaBlendMode == 0) {
						GUI.color = new Color(0,0,0,0f);
					} else if (pText.alphaBlendMode == 1) {
						GUI.color = new Color(0,0,0,0.85f);
					} else {
						GUI.color = new Color(0,0,0,1f);
					}
					GUI.DrawTexture(new Rect(leftOffset, screenPos.y, manager.AvatarSize - 1, manager.AvatarSize), blackTexture, ScaleMode.ScaleToFit, true);
				}
				/*foreach (Font f in Resources.FindObjectsOfTypeAll (typeof (Font))) {
					ScreenMessages.PostScreenMessage(f.name);
				}*/
				if (!(pText.blinkFreq > 0) || ((long)(Planetarium.GetUniversalTime() * 1000f) % (1000 / pText.blinkFreq) < (1000 / pText.blinkFreq) / 2) ){
					printLabel(pText.text, leftOffset + pText.leftOffset, pText.topOffset, pText.textColor);
				}
				
			}
			GUI.color = oldColor;
		}
		
		private void printLabel(string text, float leftOffset, float topOffset, Color color) {
			string[] lines = text.Split('\n');
			for (int i=0; i < lines.Length; i++) {
				float lineOffset = (i - lines.Length / 2) * LINE_HEIGHT;
				Rect r = new Rect(leftOffset, screenPos.y + manager.AvatarSize / 2 + topOffset + lineOffset, manager.AvatarSize, LINE_HEIGHT);
				GUI.color = color;
				GUI.Label(r, lines[i], centeredLabelStyle);
			}
		}

		private void vesselCheck(Vessel v)
		{
			reload = true;
		}

		private KerbalGUIManager findKerbalGUIManager()
		{
			FieldInfo[] fields = typeof(KerbalGUIManager).GetFields(BindingFlags.NonPublic | BindingFlags.Static);

			if (fields == null)
				return null;

			if (fields[0].GetValue(null).GetType() != typeof(KerbalGUIManager))
				return null;

			return (KerbalGUIManager)fields[0].GetValue(null);
		}
		
		class PortraitText {
			internal string text;
			internal int leftOffset;
			internal int topOffset;
			internal Color textColor;
			internal int blinkFreq;
			internal int alphaBlendMode = 0; //0 - don't darken portrait, 1 - darken, >1 - make totally black
			internal bool drawtext = false;
			internal PortraitText() {	}
			internal PortraitText(string text, int leftOffset, int topOffset, Color textColor, int blinkFreq, int alphaBlendMode) {
				this.text = text;
				this.leftOffset = leftOffset;
				this.topOffset = topOffset;
				this.textColor = textColor;
				this.blinkFreq = blinkFreq;
				this.alphaBlendMode = alphaBlendMode;
				this.drawtext = true;
			}
		}

	}
}
