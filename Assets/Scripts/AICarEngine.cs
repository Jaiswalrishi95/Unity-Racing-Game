using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 30f;
    public WheelCollider FL;
    public WheelCollider FR;
    private List<Transform> nodes;
    private int currentNode = 0;
    public int bhp;
    public float torque;
    public int brakeTorque;

    public float[] gearRatio;
    public int currentGear;
    public WheelCollider RL;
    public WheelCollider RR;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.3f;
    public float frontSensorAngle = 30f;
    public float currentSpeed;
    public float maxSpeed;
    public int maxRevSpeed;
    public float engineRPM;
    public float gearUpRPM;
    public float gearDownRPM;
    private GameObject COM;
    private float pitch = 0.8f;
    private bool avoiding = false;

    // Start is called before the first frame update
    private void Start()
    {
        FL = GameObject.Find("wheelss 1 col").GetComponent<WheelCollider>();
        FR = GameObject.Find("wheelss col").GetComponent<WheelCollider>();
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        RL = GameObject.Find("wheelss 3 col").GetComponent<WheelCollider>();
        RR = GameObject.Find("wheelss 2 col").GetComponent<WheelCollider>();
        COM = GameObject.Find("COM");
        GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.transform.localPosition.x * transform.localScale.x, COM.transform.localPosition.y * transform.localScale.y, COM.transform.localPosition.z * transform.localScale.z);
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(FL, frontDriverT);
        UpdateWheelPose(FR, frontPassengerT);
        UpdateWheelPose(RL, rearDriverT);
        UpdateWheelPose(RR, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ApplySteer();
        AutoGears();
        Accelerate();
        UpdateWheelPoses();
        CheckWayPointDistance();
        Sensors();

        //Defenitions.
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        engineRPM = Mathf.Round(((RL.rpm + RR.rpm) / 2) * gearRatio[currentGear]);
        torque = bhp * gearRatio[currentGear];
        pitch = currentSpeed / maxSpeed;
        GetComponent<AudioSource>().pitch = pitch;
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;
        
        //Right Sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
        }
        
        //Right Angle Sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * -transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }


        //Left Sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }
        

        //Left Angle Sensor
       else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * -transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        //Center Sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Track"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if(hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }

                }
            }
        }

        if (avoiding)
        {
            FL.steerAngle = maxSteerAngle * avoidMultiplier;
            FR.steerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

   public void Accelerate()
    {

        if (currentSpeed < maxSpeed && currentSpeed > maxRevSpeed && engineRPM <= gearUpRPM)
        {

            RL.motorTorque = torque * -100;
            RR.motorTorque = torque * -100;
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }
        else
        {

            RL.motorTorque = 0;
            RR.motorTorque = 0;
            RL.brakeTorque = brakeTorque;
            RR.brakeTorque = brakeTorque;
        }
    }

    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        FL.steerAngle = -newSteer;
        FR.steerAngle = -newSteer;
    }

    void AutoGears()
    {

        int AppropriateGear = currentGear;

        if (engineRPM >= gearUpRPM)
        {

            for (var i = 0; i < gearRatio.Length; i++)
            {
                if (RL.rpm * gearRatio[i] < gearUpRPM)
                {
                    AppropriateGear = i;
                    break;
                }
            }
            currentGear = AppropriateGear;
        }

        if (engineRPM <= gearDownRPM)
        {
            AppropriateGear = currentGear;
            for (var j = gearRatio.Length - 1; j >= 0; j--)
            {
                if (RL.rpm * gearRatio[j] > gearDownRPM)
                {
                    AppropriateGear = j;
                    break;
                }
            }
            currentGear = AppropriateGear;
        }
    }

    private void CheckWayPointDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            if(currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            } else
            {
                currentNode++;
            }
        }
    }
}
