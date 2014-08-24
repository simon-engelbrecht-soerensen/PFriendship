using UnityEngine;
using System.Collections;

public class HiResScreenShotsUnity : MonoBehaviour {
	private int resWidth = 2550; 
	private int resHeight = 3300;
	public int size = 4;
	private bool takeHiResShot = false;
	
	public static string ScreenShotName(int width, int height) {
		return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
		                     Application.dataPath, 
		                     width, height, 
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
	
	public void TakeHiResShot() {
		takeHiResShot = true;
	}
	
	void LateUpdate() {
		takeHiResShot |= Input.GetKeyDown("k");
		//		takeHiResShot |= Input.GetButtonDown(Ke);
		if (takeHiResShot) {
			Application.CaptureScreenshot(ScreenShotName(resWidth, resHeight), size);
			//			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			//			camera.targetTexture = rt;
			//			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			//			camera.Render();
			//			RenderTexture.active = rt;
			//			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			//			camera.targetTexture = null;
			//			RenderTexture.active = null; // JC: added to avoid errors
			//			Destroy(rt);
			//			byte[] bytes = screenShot.EncodeToPNG();
			//			string filename = ScreenShotName(resWidth, resHeight);
			//			System.IO.File.WriteAllBytes(filename, bytes);
			//			Debug.Log(string.Format("Took screenshot to: {0}", filename));
			takeHiResShot = false;
		}
	}
}