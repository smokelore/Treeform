using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractalManager : Singleton<FractalManager>
{	
	protected FractalManager () {}

	public FractalNode root;
	public int maxDepth;
	public FloatRange depthSpawnDelay;

	public Color startColor, endColor;
	public AnimationCurve debugColorFade;

	public float trailDefaultStartWidth;
	public float trailDefaultEndWidth;

	public float childOffsetError;
	public AnimationCurve childOffsetErrorCurve;

	public float childRotationSpeed;
	public float childRetractSpeed;
	public float childRetractAmount;

	public AnimationCurve materialFadeCurve;
	public int materialFadeMaxDist;

	public AnimationCurve childDeathCurve;
	public float childDeathProb;

	public bool debugDrawBranches;

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
		float value = ((float)(node.depth)) / ((float)(maxDepth));

		return Color.Lerp(startColor, endColor, value);
	}

	public float GetRandomChildOffsetError(FractalNode node)
	{
		float rand = Random.Range(0.0f, 1.0f);

		return (childOffsetError * childOffsetErrorCurve.Evaluate(rand));
	}

	public bool ShouldIReproduce(FractalNode node)
	{
		float deathProb = childDeathCurve.Evaluate((float)(node.depth)) / ((float)(maxDepth));
		bool died = (Random.Range(0.0f, childDeathProb) <= deathProb);

		return !died;
	}
}
