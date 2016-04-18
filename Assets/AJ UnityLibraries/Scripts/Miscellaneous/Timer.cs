using UnityEngine;
using System.Collections;

[System.Serializable]
public class Timer {

	public float Interval;
	public bool Stopped {get; private set;}

	bool realTime;
	float startTime;
	int frame = 0;

	public Timer(float interval, bool realTime = false){
		this.realTime = realTime;
		this.Interval = interval;
		startTime = GetTime();
		frame = Time.frameCount;
	}

	public bool IsFinished(){
		if (!Stopped){
			frame = Time.frameCount;
			if (Percent() >= 1){
				Stop();
				return true;
			}
			else {
				return false;	
			}
		}
		else {
			if (Time.frameCount == frame){
				return Percent() >= 1;
			}
			return false;
		}
	}

	public void SetInterval(float interval){
		this.Interval = interval;
	}

	public void AddTime(float amount){
		this.startTime += amount;
	}

	public float RawPercent(){
		return (GetTime() - startTime)/Interval;
	}

	public float Percent(){
		float t = Mathf.Clamp01((GetTime() - startTime)/Interval);
		return t;
	}	
	
	float GetTime(){
		if (realTime){
			return Time.realtimeSinceStartup;
		}
		else{
			return Time.time;
		}
	}

	public void Restart(){
		Stopped = false;
		startTime = GetTime();
	}

	public void Stop(){
		Stopped = true;
	}

	public void SetDone() {
		startTime = Time.time - Interval;
	}
}
