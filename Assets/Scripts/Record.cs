using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Record : MonoBehaviour
{

    [System.Serializable]
    //simple class for values we're going to record
    public class GoVals
    {
        public SerializableQuaternion rotation;
        public SerializableVector3 position;

        //constructor
        public GoVals() { }
        public GoVals(SerializableVector3 position, SerializableQuaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    //a list of recorded values
    List<GoVals> vals = new List<GoVals>();
    List<GoVals> vals1 = new List<GoVals>();
    List<GoVals> vals2 = new List<GoVals>();
    List<GoVals> vals3 = new List<GoVals>();

    List<GoVals> recieved_vals = new List<GoVals>();
    List<GoVals> recieved_vals1 = new List<GoVals>();
    List<GoVals> recieved_vals2 = new List<GoVals>();
    List<GoVals> recieved_vals3 = new List<GoVals>();

    //are we recording?
    public bool is_recording = false;
    //...are we replaying?
    public bool is_replaying = false;
    //while replaying, an int to keep track of which frame we're on
    int replayFrame = 0;

    //cache of our transform
    Transform tf;
    Transform tq;
    Transform tr;
    Transform ts;
    string dir;
    string serializationFile;
    void Start()
    {
        //cache it...
        tf = GameObject.FindWithTag("Mercedes").transform;
        tq = GameObject.FindWithTag("Player").transform;
        tr = GameObject.FindWithTag("Player1").transform;
        ts = GameObject.FindWithTag("Player2").transform;
        is_recording = false;
        is_replaying = false;

        dir = Application.persistentDataPath;
        serializationFile = System.IO.Path.Combine(dir, "record.bin");
    }

    void FixedUpdate()
    {
        Records();
        Replay();
    }



    public void Records()
    {
        if (!is_recording)
        {
            return;
        }
            //add a new value to our recorder list
            vals.Add(new GoVals(tf.position, tf.rotation));
            vals1.Add(new GoVals(tq.position, tq.rotation));
            vals2.Add(new GoVals(tr.position, tr.rotation));
            vals3.Add(new GoVals(ts.position, ts.rotation));
    }

    public void Replay()
    {
        if (!is_replaying) return;
        
            //if the frame we're going to try to replay exceeds available replayable frames...
            if (replayFrame >= recieved_vals.Count && replayFrame >= recieved_vals1.Count && replayFrame >= recieved_vals2.Count && replayFrame >= recieved_vals3.Count)
            {
                replayFrame = 0;
                is_replaying = false;
                //uncomment the next line if you want it to make a new recording next time, otherwise it will continue from where it left off
                //vals = new List<GoVals>() ;
                return;
            }
            //set our transform values
            tf.position = recieved_vals[replayFrame].position;
            tf.rotation = recieved_vals[replayFrame].rotation;
            tq.position = recieved_vals1[replayFrame].position;
            tq.rotation = recieved_vals1[replayFrame].rotation;
            tr.position = recieved_vals2[replayFrame].position;
            tr.rotation = recieved_vals2[replayFrame].rotation;
            ts.position = recieved_vals3[replayFrame].position;
            ts.rotation = recieved_vals3[replayFrame].rotation;
        //increment our frame
        replayFrame++;
    }
    public void stop_recording()
    {
        is_recording = false;

        List<List<GoVals>> sent_combined_val_list = new List<List<GoVals>>();
        sent_combined_val_list.Add(vals);
        sent_combined_val_list.Add(vals1);
        sent_combined_val_list.Add(vals2);
        sent_combined_val_list.Add(vals3);
        //serialize
        using (Stream stream = File.Open(serializationFile, FileMode.Create))
         {
             var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

             bformatter.Serialize(stream, sent_combined_val_list);
             Debug.Log("Sent" + bformatter);
         }

        /*  using (Stream stream = File.Open(serializationFile, FileMode.Create))
         {
             XmlSerializer serializer = new XmlSerializer(typeof(GoVals));

             serializer.Serialize(stream, sent_combined_val_list);
             Debug.Log("Sent" + serializer);
         }*/


        return;
    }
    public void start_recording()
    {
        is_recording = true;
    }
    public void stop_replay()
    {
        is_replaying = false;
    }
    public void start_replay()
    {
        //deserialize
         using (Stream stream = File.Open(serializationFile, FileMode.Open))
         {
             var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

             List<List<GoVals>> received_combined_val_list = (List<List<GoVals>>)bformatter.Deserialize(stream);

             Debug.Log("Received" + received_combined_val_list);
             recieved_vals = received_combined_val_list[0];
             recieved_vals1 = received_combined_val_list[1];
             recieved_vals2 = received_combined_val_list[2];
             recieved_vals3 = received_combined_val_list[3];
        }

        /*using (Stream stream = File.Open(serializationFile, FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GoVals));
            List<List<GoVals>> received_combined_val_list = (List<List<GoVals>>)serializer.Deserialize(stream);
            Debug.Log("Received" + received_combined_val_list);
            recieved_vals = received_combined_val_list[0];
            recieved_vals1 = received_combined_val_list[1];
        }*/
        is_replaying = true; 
    }

    /*void OnGUI()
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
    }*/
}