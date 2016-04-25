using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FractalNode : MonoBehaviour 
{
	public FractalNode 			parent;
	public List<FractalNode> 	children;
	public int 					depth;
	public int 					index;

	public int 				childCount;
	public List<Vector3> 	childOffset;
	public List<float> 		childScale;
	public float 			childSpawnDelay;

	private Vector3 lastPosition;
	private Vector3 lastScale;

	public bool				debugCos;

	void Awake()
	{
		this.children = new List<FractalNode>();
		this.lastPosition = this.transform.localPosition;
		this.lastScale = this.transform.localScale;
	}

	public static FractalNode Create()
	{
		GameObject go = new GameObject();
		FractalNode component = go.AddComponent<FractalNode>();
		component.gameObject.name = "0";
		component.depth = 0;

		return component;
	}

	public static FractalNode Create(FractalNode parentNode, bool bInheritParameters)
	{
		GameObject go = new GameObject();
		FractalNode component = go.AddComponent<FractalNode>();

		if (parentNode == null)
		{
			go.name = "0";
			component.depth = 0;
		}
		else
		{
			int nodeIndex = 0;

			if (parentNode.children != null)
			{
				nodeIndex = parentNode.children.Count;
			}

			if (nodeIndex < parentNode.childCount)
			{
				component.parent = parentNode;
				go.transform.parent = parentNode.transform;
				go.transform.localScale = Vector3.one * parentNode.childScale[nodeIndex];
				go.transform.localPosition = parentNode.childOffset[nodeIndex];//new Vector3(parentNode.childOffset[nodeIndex].x * go.transform.localScale.x, parentNode.childOffset[nodeIndex].y * go.transform.localScale.y, parentNode.childOffset[nodeIndex].z * go.transform.localScale.z);
				component.depth = parentNode.depth + 1;
				component.index = parentNode.children.Count + 1;

				if (bInheritParameters)
				{
					component.InheritParameters();
				}

				parentNode.children.Add(component);
			}
			else
			{
				Debug.Log("Error: FractalNode " + parentNode.gameObject.name + " can't receive " + (parentNode.children.Count + 1) + " children.");
				return null;
			}
		}

		return component;
	}

	public void SetParameters(int childCount, List<Vector3> childOffset, List<float> childScale, float childSpawnDelay)
	{
		this.childCount = childCount;
		this.childOffset = childOffset;
		this.childScale = childScale;
		this.childSpawnDelay = childSpawnDelay;
	}

	public void InheritParameters()
	{
		List<Vector3> erroredOffset = parent.childOffset;
		for (int i = 0; i < erroredOffset.Count; i++)
		{
			Vector3 offset = erroredOffset[i];

			offset.x += FractalManager.Instance.GetRandomChildOffsetError(this);
			offset.y += FractalManager.Instance.GetRandomChildOffsetError(this);
			offset.z += FractalManager.Instance.GetRandomChildOffsetError(this);

			erroredOffset[i] = offset;
		}

		this.SetParameters(parent.childCount, erroredOffset, parent.childScale, parent.childSpawnDelay);
	}
		
	public FractalNode CreateChild()
	{
		FractalNode child = FractalNode.Create(this, true);

		child.gameObject.name = this.gameObject.name + " > " + child.depth + "." + child.index;

		//child.gameObject.AddComponent<MeshFilter>().mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
		//child.gameObject.AddComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().material;
		TrailRenderer newTrail = child.gameObject.AddComponent<TrailRenderer>();
		newTrail.materials = this.GetComponent<TrailRenderer>().materials;
		newTrail.startWidth = 0;// * child.transform.localScale.x/2;
		newTrail.endWidth = 0;// * child.transform.lossyScale.x/2;
		newTrail.materials[0].color = FractalManager.Instance.GetDepthColor(child);

		MaterialFadeInDistance newFade = child.gameObject.AddComponent<MaterialFadeInDistance>();

		return child;
	}

	public bool ParentDepthHasBeenCreated()
	{
		if (parent != null)
		{
			return ((parent.children != null && parent.children.Count >= parent.childCount) && parent.ParentDepthHasBeenCreated());
		}
		else
		{
			return true;
		}
	}

	public IEnumerator CreateChildren()
	{
		yield return new WaitForSeconds(FractalManager.Instance.depthSpawnDelay.RandomSample());

		while (!ParentDepthHasBeenCreated())
		{
			yield return null;
		}
			
		int deadBabies = 0;
		for (int i = 0; i < childCount; i++)
		{
			if (!FractalManager.Instance.ShouldIReproduce(this))
			{
				FractalNode child = CreateChild();
				if (depth < FractalManager.Instance.maxDepth)
				{
					StartCoroutine(child.CreateChildren());
					//yield return new WaitForSeconds(childSpawnDelay);
				}
			}
			else
			{
				deadBabies++;
			}
		}

		childCount -= deadBabies;
	}

	public void DebugDrawBranches()
	{
		for (int i = 0; i < children.Count; i++)
		{
			Debug.DrawLine(this.transform.position, children[i].transform.position, FractalManager.Instance.GetDepthColor(this));
		}
	}

	public IEnumerator UpdateChildPositions()
	{
		yield return new WaitForSeconds(FractalManager.Instance.depthSpawnDelay.RandomSample());

		for (int i = 0; i < children.Count; i++)
		{
			FractalNode child = children[i];
			child.transform.localPosition = childOffset[i];
			child.childOffset = childOffset;

			StartCoroutine(child.UpdateChildPositions());
		}
	}

	void Update()
	{
		if (FractalManager.Instance.debugDrawBranches)
		{
			DebugDrawBranches();
		}

		if (children != null && children.Count >= childCount)
		{
//			if (lastPosition != this.transform.localPosition && parent != null)
//			{
//				parent.childOffset[index-1] = this.transform.localPosition;
//
//				//StartCoroutine(parent.UpdateChildPositions());
//
//				lastPosition = this.transform.localPosition;
//			}
		}

		if (parent != null)
		{
			this.transform.RotateAround(parent.transform.position, (this.transform.position - parent.transform.position).normalized, FractalManager.Instance.childRotationSpeed * Time.deltaTime);
			if (parent.parent != null)
			{
				float retractAmount = FractalManager.Instance.childRetractAmount;
				float retractSpeed = FractalManager.Instance.childRetractSpeed;
				float offsetFactor = 0.75f * ((1.0f - retractAmount) + retractAmount * Mathf.Cos(2 * Mathf.PI * Time.time * retractSpeed));
				if (debugCos)
					Debug.Log(offsetFactor);
				this.transform.localPosition = offsetFactor * parent.childOffset[index - 1];
			}
		}

		if (gameObject.GetComponent<TrailRenderer>() != null)
		{
			gameObject.GetComponent<TrailRenderer>().startWidth = FractalManager.Instance.trailDefaultStartWidth;// * transform.lossyScale.x;
			gameObject.GetComponent<TrailRenderer>().endWidth = FractalManager.Instance.trailDefaultEndWidth;// * transform.lossyScale.x;
			gameObject.GetComponent<TrailRenderer>().materials[0].color = FractalManager.Instance.GetDepthColor(this);
		}
	}
}