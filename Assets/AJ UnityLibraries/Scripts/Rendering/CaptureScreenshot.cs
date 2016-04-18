using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureScreenshot : MonoBehaviour {

	public bool CaptureBackground = true;

	void Update(){
		/*
		if (Input.GetKeyDown(KeyCode.P)){
			StartCoroutine(screenShot());
		}
		*/
	}

	public IEnumerator screenShot(){
		yield return new WaitForEndOfFrame();
		Texture2D tex = new Texture2D(Screen.width, Screen.height);
		tex.ReadPixels(GetComponent<Camera>().pixelRect, 0, 0);
		if (CaptureBackground){
			for (int i = 0; i < tex.width; i++){
				for (int j = 0; j < tex.height; j++){
					Color c = tex.GetPixel(i,j);
					c.a = 1f;
					tex.SetPixel(i,j,c);
				}
			}
		}
		tex.Apply();
		FileStream file = File.Create(Application.dataPath + "\\..\\Screen Shots\\" + getFilename());
		BinaryWriter writer = new BinaryWriter(file);
		writer.Write(tex.EncodeToPNG());
		file.Close();
	} 

	string getFilename(){
		return "Screenshot_" + System.DateTime.Now.Year + "_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Day + " " + System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + "-" + System.DateTime.Now.Second + ".png";
	}
}