using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialFadeInDistance : MonoBehaviour 
{
	public List<Material> materials = new List<Material>();
	public AnimationCurve fadeCurve;
	public int maxDist;

	public void Initialize()
	{
//		if (this.GetComponent<MeshRenderer>() != null && this.GetComponent<MeshRenderer>().material != null)
//		{
//			materials.Add(this.GetComponent<MeshRenderer>().material);
//		}

		if (this.GetComponent<LineRenderer>() != null)
		{
			for (int i = 0; i < this.GetComponent<LineRenderer>().materials.Length; i++)
			{
				materials.Add(this.GetComponent<LineRenderer>().materials[i]);
			}
		}
	}
	
	void Update () 
	{
		if (materials.Count < 1)
		{
			Initialize();
		}
		else
		{
			for (int i = 0; i < materials.Count; i++)
			{
				Color newColor = materials[i].color;
				newColor.a = fadeCurve.Evaluate((Camera.main.transform.position - this.transform.position).magnitude / maxDist);
				materials[i].color = newColor;
			}
		}
	}
}
