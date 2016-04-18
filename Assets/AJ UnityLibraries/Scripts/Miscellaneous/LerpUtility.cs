using UnityEngine;
using System.Collections;

/// <summary>
/// A class the provides alternate t-values for different Lerp transitions. More will be added later
/// </summary>
public static class LerpUtility {

	public delegate float LerpFunction(float time);
	public delegate void FloatCallback(ref float property);

	public static float Linear(float time){
		return time;
	}

	public static float Spherical(float time){
		return (1-(Mathf.Cos(time * Mathf.PI)))/2f;
	}

	public static float AntiSpherical(float time){
		float temp = Mathf.Sin(time * Mathf.PI)/2f;
		return Mathf.Abs(time) > .5f ? Mathf.Sign(time) - temp : temp;
	}

	public static float HemiSpherical(float time){
		return Mathf.Sin(time * Mathf.PI / 2f);
	}
	
	public static float AntiHemiSpherical(float time){
		return Mathf.Sign(time) * (1 - Mathf.Sin(time * Mathf.PI / 2f + Mathf.PI /2f));
	}

	public static float Quadratic(float time){
		return time*time;	
	}

	public static float Undershoot(float time, float amp){
		return time * time * time - time * amp * Mathf.Sin(Mathf.PI * time);
	}

	public static float Undershoot(float time){
		return Undershoot(time, 1f);
	}
	
	public static float Overshoot(float time, float s){
		return ((time-1) * time * ((s + 1) * time + s) + 1);
		//return time * time * ((s + 1) * time - s);
	}

	public static float Overshoot(float time){
		return Overshoot(time, 1.70158f);
	}

	public static float Elastic(float time, float amp){
		return Mathf.Pow(2,-10*time) * Mathf.Sin((time-amp/4)*(2*Mathf.PI)/amp) + 1;
		//return 1 - (1- time) * (1-time) * (1-time) + (1-time) * amp * Mathf.Sin(Mathf.PI * time);
	}
	
	public static float Elastic(float time){
		return Elastic(time, .5f);
	}

	public static float UnLerp(Vector3 start, Vector3 end, Vector3 position){
		Vector3 dir = end - start;
		return Mathf.Clamp01(Vector3.Dot(dir.normalized, (position - start)/dir.magnitude));
	}

	public static Vector3 Extrap(Vector3 start, Vector3 end, float time){
		return start + (end - start) * time;
	}

	public static float Extrap(float start, float end, float time){
		return start + (end - start) * time;
	}


	public static IEnumerator LerpColor(this Renderer renderer, Color start, Color end, Timer timer, LerpFunction function){
		return lerpColor(renderer, start, end, timer, function);
	}

	static IEnumerator lerpColor(Renderer renderer, Color start, Color end, Timer timer, LerpFunction function){
		while (!timer.IsFinished ()){
			renderer.material.color = Color.Lerp(start, end, function(timer.Percent()));
			yield return 0;
		}
		renderer.material.color = end;
	}

	public static IEnumerator LerpColor(this Material material, Color start, Color end, Timer timer, LerpFunction function){
		return lerpColor(material, start, end, timer, function);
	}

	static IEnumerator lerpColor(Material material, Color start, Color end, Timer timer, LerpFunction function){
		while (!timer.IsFinished ()){
			material.color = Color.Lerp(start, end, function(timer.Percent()));
			yield return 0;
		}
		material.color = end;
	}

	public static IEnumerator LerpPosition(this Transform transform, Vector3 start, Vector3 end, Timer timer, LerpFunction function){
		return lerpPosition(transform, start, end, timer, function);
	}

	static IEnumerator lerpPosition(Transform transform, Vector3 start, Vector3 end, Timer timer, LerpFunction function){
		while (!timer.IsFinished ()){
			transform.position = Vector3.Lerp(start, end, function(timer.Percent()));
			yield return 0;
		}
		transform.position = end;
	}

	public static IEnumerator LerpScale(this Transform transform, Vector3 start, Vector3 end, Timer timer, LerpFunction function){		
		return lerpScale(transform, start, end, timer, function);
	}

	static IEnumerator lerpScale(Transform transform, Vector3 start, Vector3 end, Timer timer, LerpFunction function){		
		while (!timer.IsFinished ()){
			transform.localScale = Vector3.Lerp(start, end, function(timer.Percent()));
			yield return 0;
		}
		transform.localScale = end;
	}

	public static IEnumerator LerpRotation(this Transform transform, Quaternion start, Quaternion end, Timer timer, LerpFunction function){
		return lerpRotation(transform, start, end, timer, function);
	}

	static IEnumerator lerpRotation(Transform transform, Quaternion start, Quaternion end, Timer timer, LerpFunction function){
		while (!timer.IsFinished ()){
			transform.rotation = Quaternion.Lerp(start, end, function(timer.Percent()));
			yield return 0;
		}
		transform.rotation = end;
	}
}