using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractalManager : Singleton<FractalManager>
{	
	protected FractalManager () {}

	public FractalNode root;
	public int maxDepth;
	public float depthSpawnDelay;

	public Color startColor, endColor;
	public AnimationCurve debugColorFade;

	void Start () 
	{
		if (maxDepth > -1)
		{
			StartCoroutine(root.CreateChildren());
		}
	}

	void Update () 
	{
	
	}

	public Color GetDepthColor(FractalNode node)
	{
		float value = ((float)(node.depth)) / ((float)(maxDepth + 1));

		return Color.Lerp(startColor, endColor, value);
	}
}
