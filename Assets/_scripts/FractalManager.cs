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
	public AnimationCurve colorFadeCurve;

	public float trailDefaultStartWidth;
	public float trailDefaultEndWidth;

	public float 			childOffsetError;
	public AnimationCurve 	childOffsetErrorCurve;

	public float childRotationSpeed;
	public float childRetractSpeed;
	public float childRetractAmount;

	public AnimationCurve 	childDeathCurve;
	public float 			childDeathProb;

	public bool debugDrawBranches;

    public EventMicBeat micBeat;

    [HideInInspector]
    public int prevMaxDepth;

	void Start () 
	{
        micBeat = this.gameObject.GetComponent<EventMicBeat>();

		prevMaxDepth = maxDepth;

		if (maxDepth > 0)
		{
			StartCoroutine(root.CreateChildren());
		}
	}

	void Update () 
	{
        if (prevMaxDepth != maxDepth && maxDepth < 1)
        {
            // remove all nodes except root
            StartCoroutine(root.DeleteChildren());
        }
        else if (maxDepth > prevMaxDepth)
        {
		    // add depths of nodes
            StartCoroutine(root.GrowTree());
        }
        else if (maxDepth < prevMaxDepth)
        {
            // remove depths of nodes
            StartCoroutine(root.ShrinkTree());
        }

        prevMaxDepth = maxDepth;  

        if (micBeat != null && micBeat.IsReady())
        {
            Debug.Log(micBeat.GetBeatPeriod());
        }
	}

	public Color GetDepthColor(FractalNode node)
	{
		float value = colorFadeCurve.Evaluate(((float)(node.depth)) / ((float)(maxDepth)));

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
