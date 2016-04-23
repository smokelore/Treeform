using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostProcessDepthGrayscale : MonoBehaviour 
{
	public Material mat;

	void Start () 
	{
		this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source,destination,mat);
		//mat is the material which contains the shader
		//we are passing the destination RenderTexture to
	}
}
