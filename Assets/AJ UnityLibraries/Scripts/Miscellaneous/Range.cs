using UnityEngine;
using System.Collections;

[System.Serializable]
public class Range<T> {
	public T Min, Max;
	public T Start{
		get {
			return Min;
		}
		set {
			Min = value;
		}
	}
	public T End{
		get {
			return Max;
		}
		set {
			Max = value;
		}
	}

	public T Random {
		get {
			if (randomSet){
				return random;
			}
			return RandomSample();
		}
	}
	T random;
	bool randomSet = false;

	public Range(T min, T max){
		Min = min;
		Max = max;
	}

	public T RandomSample(){
		random = LinearSample(UnityEngine.Random.value);
		randomSet = true;
		return random;
	}

	public T LinearSample(float t){
		T linear = Min;
		System.Type objectType = Start.GetType();
		if (objectType == typeof(int)) {
			linear = (T)(object) (int)Mathf.Lerp((int)(object)Min, (int)(object)Max, t);
		}
		else if (objectType == typeof(float)) {
			linear = (T)(object) Mathf.Lerp((float)(object)Min, (float)(object)Max, t);
		}
		else {
			throw new System.InvalidOperationException("Non-numeric types cannot be sampled.");
		}
		return linear;
	}

	public bool Contains(T item){
		bool contains = false;

		System.Type objectType = Start.GetType();
		if (objectType == typeof(int)) {
			contains = ((int)(object) item) >= ((int)(object) Min) && ((int)(object) item) <= ((int)(object) Max);
		}
		else if (objectType == typeof(float)) {
			contains = ((float)(object) item) >= ((float)(object) Min) && ((float)(object) item) <= ((float)(object) Max);
		}
		else {
			throw new System.InvalidOperationException("Non-numeric types cannot contain.");
		}
		return contains;
	}
}

[System.Serializable]
public class IntRange : Range<int> {
	public IntRange (int start, int end) : base(start, end){
	}
}

[System.Serializable]
public class FloatRange : Range<float> {
	public FloatRange (float start, float end) : base(start, end){
	}

	public float GetClampedPercent(float value){
		return Mathf.Clamp01(GetPercent(value));
	}

	public float GetPercent(float value){
		return (value - Min) / (Max - Min);
	}
}