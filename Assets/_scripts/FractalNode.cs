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
		this.SetParameters(parent.childCount, parent.childOffset, parent.childScale, parent.childSpawnDelay);
	}
		
	public FractalNode CreateChild()
	{
		FractalNode child = FractalNode.Create(this, true);

		child.gameObject.name = this.gameObject.name + " > " + child.depth + "." + child.index;

		//child.gameObject.AddComponent<MeshFilter>().mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
		//child.gameObject.AddComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().material;

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
		yield return new WaitForSeconds(FractalManager.Instance.depthSpawnDelay);

		while (!ParentDepthHasBeenCreated())
		{
			yield return null;
		}

		for (int i = 0; i < childCount; i++)
		{
			FractalNode child = CreateChild();
			if (depth < FractalManager.Instance.maxDepth)
			{
				StartCoroutine(child.CreateChildren());
				//yield return new WaitForSeconds(childSpawnDelay);
			}
		}
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
		yield return new WaitForEndOfFrame();

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
		DebugDrawBranches();

		if (children != null && children.Count >= childCount)
		{
			if (lastPosition != this.transform.localPosition && parent != null)
			{
				parent.childOffset[index-1] = this.transform.localPosition;

				StartCoroutine(parent.UpdateChildPositions());

				lastPosition = this.transform.localPosition;
			}
		}
	}
}