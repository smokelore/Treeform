using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

	#region Vectors
	public static Vector2 Reverse(this Vector2 self){
		float y = self.y;
		self.y = self.x;
		self.x = y;
		return self;
	}

	public static Vector3 Reverse(this Vector3 self){
		float y = self.y;
		self.y = self.x;
		self.x = y;
		return self;
	}

	public static Vector3 Abs(this Vector3 self){
		return new Vector3(Mathf.Abs(self.x), Mathf.Abs(self.y), Mathf.Abs(self.z));
	}

	public static Vector3 Snap(this Vector3 self, float snap = 1.0f){
		self.Set(Mathf.Round(self.x / snap) * snap, Mathf.Round(self.y / snap) * snap, Mathf.Round(self.z / snap) * snap);
		return self;
	}

	public static Vector3 Flatten(this Vector3 self){
		self.Set(self.x, 0f, self.z);
		return self;
	}

	public static Vector2 Flatten2D(this Vector3 self){
		return new Vector2(self.x, self.y);
	}	

	#endregion

	#region Colliders
	
	public static Vector3 RandomPointInBounds(this Collider self){
		Vector3 point = self.bounds.min;
		point += Vector3.right * self.bounds.size.x * Random.value;
		point += Vector3.up * self.bounds.size.y * Random.value; 
		point += Vector3.forward * self.bounds.size.z * Random.value;
		return point;
	}
	
	public static Vector2 RandomPointInBounds(this Collider2D c){
		Vector2 point = c.bounds.min;
		point += Random.value * c.bounds.size.x * Vector2.right;
		point += Random.value * c.bounds.size.y * Vector2.up;
		return point;
	}

	#endregion

	#region Numbers
	public static bool InRange(this float self, float min, float max){
		return self >= min && self <= max;
	}	
	#endregion

	#region Generic
	public static T RandomElement<T>(this ICollection<T> self, int seed = -1){
		T element = default(T);
		if (seed >= 0){
			Random.seed = seed;
		}

		int number = Random.Range(0, self.Count);
		int count = 0;
		foreach (T el in self){
			if (count == number){
				element = el;
				break;
			}
			count++;
		}
		return element;
	}

	public static void Swap<T>(this IList<T> self, int i, int j){
		T temp = self[i];
		self[i] = self[j];
		self[j] = temp;
	}

	public static void Shuffle<T>(this IList<T> self, int seed = -1){
		if (seed >= 0){
			Random.seed = seed;
		}
		for (int i = self.Count - 1; i > 0; i--){
			int j = Random.Range(0, i + 1);
			self.Swap(i,j);
		}
	}

	public static GameObject[] GetChildrenByTag(this GameObject self, string tag){
		List<GameObject> tagged = new List<GameObject>();
		getChildrenWithTagRecursive(self.transform, tag, tagged);
		return tagged.ToArray();
	}

	static void getChildrenWithTagRecursive(Transform self, string tag, List<GameObject> list){
		for (int i = 0; i < self.childCount; i++){
			if (self.GetChild(i).CompareTag(tag)){
				list.Add(self.GetChild(i).gameObject);
			}
			if (self.GetChild(i).childCount > 0){
				getChildrenWithTagRecursive(self.GetChild(i), tag, list);
			}
		}
	}

	public static GameObject GetChildByName(this GameObject self, string name){
		Queue<GameObject> toSearch = new Queue<GameObject>();
		toSearch.Enqueue(self);

		while (toSearch.Count > 0){
			GameObject search = toSearch.Dequeue();
			if (search.name == name){
				return search;
			}
			for (int i = 0; i < search.transform.childCount; i++){
				toSearch.Enqueue(search.transform.GetChild(i).gameObject);
			}
		}
		return null;
	}
	#endregion

	#region Colors

	public static Color Transparent(this Color self){
		self.a = 0f;
		return self;
	}
	
	public static Color Opaque(this Color self){
		self.a = 1f;
		return self;
	}
	
	#endregion
}