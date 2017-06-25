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
		
		private static bool BundleLoaded = false;
		private static Material material;
		
		bool bypass = true;
		float magnitude = 0;
		
		//In case actualFilter = false the filter is just a dummy
		public static GreyoutCameraFilter initializeCameraFilter(Camera camera, bool actualFilter) {
			if (actualFilter && material == null) {
				material = createMaterial("G-Effects/Greyout");
			}
			GreyoutCameraFilter filter;		
			if (actualFilter) {
				filter = camera.gameObject.GetComponent<GreyoutCameraFilter>();
				if (filter == null) {
					filter = camera.gameObject.AddComponent<GreyoutCameraFilter>();
				}
			} else {
				filter = new GreyoutCameraFilter();
			}
			return filter;
		}
		
		public void setBypass(bool bypass) {
			this.bypass = bypass;
		}
		
		public void setMagnitude(float magnitude) {
			this.magnitude = magnitude;
		}
		
		void OnRenderImage(RenderTexture source, RenderTexture dest) {
			if ((material != null) && !bypass) {
				material.SetFloat("_Magnitude", magnitude);
				Graphics.Blit(source, dest, material);
			}
		}
		
		private static Material createMaterial(string shaderName) {
			string path = KSP.IO.IOUtils.GetFilePathFor(typeof(G_Effects), "shaders.bundle").Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
			return new Material(LoadBundle(path, shaderName));
		}
		
		//Old way to create a material
		/*private static Material loadShader(string fileName) {
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
		}*/
		
		public static Shader LoadBundle(string path, string assetName)
		{
			if (BundleLoaded) {
				return null;
			}
			
			using (WWW www = new WWW ("file://" + path)) {
				Debug.Log("===================== Path=" + path);
				if (www.error != null) {
					Debug.Log ("G-Effects: Shader bundle not found!");
					return null;
				}
	
				AssetBundle bundle = www.assetBundle;
				Debug.Log("===================== BUNDLE=" + bundle);
				Shader shader = bundle.LoadAsset<Shader> ("greyout");
				Debug.Log("===================== Shader=" + shader);
				bundle.Unload (false);
				www.Dispose ();
			
				BundleLoaded = true;
				return shader;
			}
		}
		
	}
}
