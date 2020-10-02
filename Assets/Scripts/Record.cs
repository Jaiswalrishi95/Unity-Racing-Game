using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Record : MonoBehaviour
{
    [System.Serializable]
    //simple class for values we're going to record
    public class GoVals
    {
        public Vector3 position;
        public Quaternion rotation;

        //constructor
        public GoVals(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    //a list of recorded values
    List<GoVals> vals = new List<GoVals>();
    List<GoVals> vals1 = new List<GoVals>();
    //are we recording?
    bool recording = false;
    //...are we replaying?
    bool replaying = false;
    //while replaying, an int to keep track of which frame we're on
    int replayFrame = 0;

    //cache of our transform
    Transform tf;
    Transform tq;

    void Start()
    {
        //cache it...
        tf = GameObject.FindWithTag("Mercedes").transform;
        tq = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Records();
        Replay();
    }

    private void Records()
    {
        if (!recording) return;

        //add a new value to our recorder list
        vals.Add(new GoVals(tf.position, tf.rotation));
        vals1.Add(new GoVals(tq.position, tq.rotation));
    }

    void Replay()
    {
        if (!replaying) return;

        //if the frame we're going to try to replay exceeds available replayable frames...
        if (replayFrame >= vals.Count && replayFrame >= vals1.Count)
        {
            replayFrame = 0;
            replaying = false;
            //uncomment the next line if you want it to make a new recording next time, otherwise it will continue from where it left off
            //vals = new List<GoVals>() ;
            return;
        }
        //set our transform values
        tf.position = vals[replayFrame].position;
        tf.rotation = vals[replayFrame].rotation;
        tq.position = vals1[replayFrame].position;
        tq.rotation = vals1[replayFrame].rotation;
        //increment our frame
        replayFrame++;
    }

    void OnGUI()
    {
        if (!replaying)
        {
            if (GUILayout.Button(recording ? "Stop Recording" : "Record"))
                recording = !recording;
        }

        if (!recording)
        {
            if (GUILayout.Button(replaying ? "Stop Replay" : "Replay"))
                replaying = !replaying;
        }
    }
}