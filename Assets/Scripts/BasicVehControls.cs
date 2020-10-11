using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicVehControls : MonoBehaviour
{
    
    public int bhp;
    public float torque;
    public int brakeTorque;
    public Record ms;
    public float[] gearRatio;
    public int currentGear;
    public GameObject canvas;
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    
    public float currentSpeed;
    public float maxSpeed;
    public int maxRevSpeed;

    public float SteerAngle;

    public float engineRPM;
    public float gearUpRPM;
    public float gearDownRPM;
    private GameObject COM;
    public bool handBraked;
    private float pitch = 0.8f;

    void Start()
    {
        pitch = 0.8f;
        FL = GameObject.Find("wheels 1 col").GetComponent<WheelCollider>();
        FR = GameObject.Find("wheels col").GetComponent<WheelCollider>();
        RL = GameObject.Find("wheels 3 col").GetComponent<WheelCollider>();
        RR = GameObject.Find("wheels 2 col").GetComponent<WheelCollider>();
        COM = GameObject.Find("COM");
        GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.transform.localPosition.x * transform.localScale.x, COM.transform.localPosition.y * transform.localScale.y, COM.transform.localPosition.z * transform.localScale.z);
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


    void FixedUpdate()
    {
        //Functions to access.
        Steer();
        AutoGears();
        Accelerate();
        UpdateWheelPoses();
        
        //Defenitions.
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        engineRPM = Mathf.Round(((RL.rpm + RR.rpm) / 2) * gearRatio[currentGear]);
        torque = bhp * gearRatio[currentGear];
        pitch = currentSpeed / maxSpeed;
        GetComponent<AudioSource>().pitch = pitch;

        if (Input.GetButton("Jump"))
        {
            HandBrakes();
        }
        if (Input.GetKey(KeyCode.R))
        {

            transform.position = new Vector3(499.508f, 194.402f, 321.193f);
            transform.rotation = new Quaternion(0f, 110f, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.C))
        {
            ms.stop_recording();
            canvas.SetActive(true);
        }
    }

    //Function
   public void Accelerate()
    {

        if (currentSpeed < maxSpeed && currentSpeed > maxRevSpeed && engineRPM <= gearUpRPM)
        {

            RL.motorTorque = torque * -Input.GetAxis("Vertical");
            RR.motorTorque = torque * -Input.GetAxis("Vertical");
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

        if (engineRPM > 0 && Input.GetAxis("Vertical") < 0 && engineRPM <= gearUpRPM)
        {

            FL.brakeTorque = torque * -Input.GetAxis("Vertical");
            FR.brakeTorque = torque * -Input.GetAxis("Vertical");
        }
        else
        {
            FL.brakeTorque = 0;
            FR.brakeTorque = 0;
        }
    }

    void Steer()
    {

        if (currentSpeed < 100)
        {
            SteerAngle = 15;
        }
        else
        {
            SteerAngle = 15;
        }

        FL.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
        FR.steerAngle = SteerAngle * Input.GetAxis("Horizontal");
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

    void HandBrakes()
    {

        RL.brakeTorque = brakeTorque;
        RR.brakeTorque = brakeTorque;
        FL.brakeTorque = brakeTorque;
        FR.brakeTorque = brakeTorque;
    }

}