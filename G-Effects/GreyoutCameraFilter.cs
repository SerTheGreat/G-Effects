using System;
using System.IO;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// Camera filter that provides picture gray out effect
	/// </summary>
	public class GreyoutCameraFilter : MonoBehaviour
	{
		
		static Material material;
		bool bypass = true;
		float magnitude = 0;
		
		public GreyoutCameraFilter() : this(true) {
		}
		
		//In case actualEffect = false the filter is just a dummy 
		public GreyoutCameraFilter(bool actualEffect)
		{
			if (actualEffect && material == null) {
				material = loadShader("greyout.shader");
			}
		}
		
		public static GreyoutCameraFilter initializeCameraFilter(Camera camera, bool actualFilter) {
			GreyoutCameraFilter filter;		
			if (actualFilter) {
				filter = camera.gameObject.GetComponent<GreyoutCameraFilter>();
				if (filter == null) {
					filter = camera.gameObject.AddComponent<GreyoutCameraFilter>();
				}
			} else {
				filter = new GreyoutCameraFilter(false);
			}
			return filter;
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
