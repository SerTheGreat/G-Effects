using System;
using System.IO;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// Camera filter that provides picture gray out effect
	/// </summary>
	public class GrayoutCameraFilter : MonoBehaviour
	{
		
		static Material material;
		bool bypass = true;
		float magnitude = 0;
		
		public GrayoutCameraFilter()
		{
			if (material == null) {
				material = loadShader("grayout.shader");
			}
		}
		
		public void setBypass(bool bypass) {
			this.bypass = bypass;
		}
		
		public void setMagnitude(float magnitude) {
			this.magnitude = magnitude;
		}
		
		private static Material loadShader(string fileName) {
			try {
				string path = KSPUtil.ApplicationRootPath.Replace(@"\", "/") + "/GameData/G-Effects/Shaders/" + fileName;
		        StreamReader reader = new StreamReader(path);
		        string shader = reader.ReadToEnd();
		        reader.Close();
		        return new Material(shader);
			} catch (Exception ex) {
		        Debug.Log(string.Format("G-Effects: loadShader exception: {0}", ex.Message));
		        return null;
		    }
		}
		
		void OnRenderImage(RenderTexture source, RenderTexture dest) {
			if ((material != null) && !bypass) {
				material.SetFloat("_Magnitude", magnitude);
				Graphics.Blit(source, dest, material);
			}
		}
		
	}
}
