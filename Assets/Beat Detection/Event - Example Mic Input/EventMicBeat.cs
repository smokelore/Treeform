using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventMicBeat : MonoBehaviour {

    public GameObject AudioBeat;

    public string selectedDevice { get; private set; }  //Mic selected
    private bool micSelected = false;                   //Mic flag
    private bool started = false;                       //Flaf to see if detection has started		
    private int minFreq, maxFreq; 						//Max and min frequencies window

    public int recordedBeatWindow;
    public List<float> recordedBeatTimes;

    public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                //StartCoroutine(showText(energy, genergy));
                RecordBeat();
                break;
            case BeatDetection.EventType.HitHat:
                //StartCoroutine(showText(hithat, ghithat));
                break;
            case BeatDetection.EventType.Kick:
                //StartCoroutine(showText(kick, gkick));
                break;
            case BeatDetection.EventType.Snare:
                //StartCoroutine(showText(snare, gsnare));
                break;
        }
    }

    public void RecordBeat()
    {
        recordedBeatTimes.Add(Time.time);

        // moving window of 64 beats
        if (recordedBeatTimes.Count > recordedBeatWindow)
        {
            recordedBeatTimes.RemoveRange(0, recordedBeatTimes.Count - recordedBeatWindow);
        }
    }

    public bool IsReady()
    {
        return (recordedBeatTimes.Count >= recordedBeatWindow);
    }

    public float GetBeatPeriod()
    {
        float sumBeatTimes = 0;
        float firstBeatTime = recordedBeatTimes[0];

        foreach (float beatTime in recordedBeatTimes.GetRange(1, recordedBeatTimes.Count-1))
        {
            sumBeatTimes += beatTime;
        }

        return (sumBeatTimes / (recordedBeatTimes.Count - 1));
    }

    // Use this for initialization
    void Start()
    {
        //Register the beat callback function
        AudioBeat.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;

        //Set up mic
        started = false;
        selectedDevice = Microphone.devices[0].ToString();
        micSelected = true;
        GetMicCaps();
        setUptMic();
        StartCapture();
        //End setup mic
    }

    //Start Mic
    public void StartCapture()
    {
        if (started)
            return;

        //start capture volume
        StartMicrophone();
        started = true;
    }

    //Stop Mic
    public void StopCapture()
    {
        StopMicrophone();
        started = false;
    }

    //Setup mic as device
    void setUptMic()
    {
        AudioBeat.GetComponent<AudioSource>().clip = null;
        AudioBeat.GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
        AudioBeat.GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it
    }

    //Get mic capabilities
    public void GetMicCaps()
    {
        Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
        if ((minFreq + maxFreq) == 0)
            maxFreq = 44100;
    }

    //True start mic
    public void StartMicrophone()
    {
        AudioBeat.GetComponent<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, maxFreq); //Starts recording
        while (!(Microphone.GetPosition(selectedDevice) > 0)) { } // Wait until the recording has started
        AudioBeat.GetComponent<AudioSource>().Play(); // Play the audio source!
    }

    //True stop mic
    public void StopMicrophone()
    {
        AudioBeat.GetComponent<AudioSource>().Stop();//Stops the audio
        Microphone.End(selectedDevice);//Stops the recording of the device	
    }
}
