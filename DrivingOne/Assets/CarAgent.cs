using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class CarAgent : Agent
{
    Rigidbody r;

    public List<WheelCollider> throttleWheels; 
    public List<WheelCollider> steeringWheels;
    public float strengthCoefficient = 20000;
    public float maxTurn = 20f;
    public Vector3 initposcar;
    public Vector3 initpostarg;
    public Transform target;
    public DateTime startOfEp;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        initposcar = r.position;
        initpostarg = target.position;
        startOfEp = DateTime.UtcNow;
    }

    // public void Awake(){
    //     Monitor.SetActive(true);
    //     Monitor.Log("Reward",GetReward(), null);
    // }

    public override void AgentReset(){
        this.r.angularVelocity = Vector3.zero;
        this.r.velocity = Vector3.zero;
        r.position = initposcar;
        r.rotation = Quaternion.identity;
        target.position = new Vector3(UnityEngine.Random.Range(-35,35),initpostarg.y,UnityEngine.Random.Range(-35,35));
    }

    public override void CollectObservations()
    {
        Vector2 targTemp = new Vector2(target.position.x,target.position.z);
        Vector2 carTemp = new Vector2(this.transform.position.x,this.transform.position.z);
        // this is to make the position of car to be relative wrt the target
        carTemp = carTemp - targTemp;
        //AddVectorObs(carTemp);
        carTemp.x = carTemp.x;
        carTemp.y = carTemp.y;
        AddVectorObs(carTemp);

        Vector3 cardir = this.transform.forward;
        Vector3 tempVec = r.position-target.position;
        float sign = (r.position.z < target.position.z)? -1.0f : 1.0f;
        float ang = Vector3.Angle(cardir , tempVec);
        AddVectorObs((180-ang));


        // Agent velocity
        AddVectorObs(r.velocity.x);
        AddVectorObs(r.velocity.z);
        //Debug.Log(carTemp+" "+r.velocity.x % 1f+" "+r.velocity.y % 1f);
    }

    public override void AgentAction(float[] act)
    {

        int throtact =(int) act[0];
        int steeract =(int) act[1];
        int throtfactor=0;
        int steerfactor=0;

        if(throtact==1)
            throtfactor=    1;
        else if(throtact==2)
            throtfactor=   -1;
        if(steeract==1)
            steerfactor=    1;
        else if(steeract==2)
            steerfactor=   -1;

        float torq = strengthCoefficient * Time.deltaTime * throtfactor;
        float steerforce = maxTurn * steerfactor;

        foreach( WheelCollider wheel in throttleWheels){
            wheel.motorTorque = torq;
        }
        foreach( WheelCollider wheel in steeringWheels){
            wheel.steerAngle = steerforce;
        }

                // CODE FOR TIME TAKEN LOSS
        float Time_loss = -0.25f;

        //CODE FOR MEAN SQ ERROR LOSS
        float temp = Vector3.Distance(r.position,target.position);
        float MSE_loss = -temp*temp/10000;

        //CODE FOR ANGULAR DIFF LOSS
        Vector3 cardir = this.transform.forward;
        Vector3 tempVec = r.position-target.position;
        float sign = (r.position.z < target.position.z)? -1.0f : 1.0f;
        float ang = Vector3.Angle(cardir , tempVec);
        float Ang_loss = -(180f-ang)/360;

        AddReward(Time_loss);
        AddReward(2.2f*MSE_loss);
        AddReward(Ang_loss);

        // Rewards
        // float distanceToTarget = Vector3.Distance(this.transform.position,
        //                                         target.position);

        // Reached target
        if (temp < 3f){
            startOfEp = DateTime.UtcNow;
            AddReward(150.0f);
            Done();
        }

        TimeSpan difference = -startOfEp.Subtract(DateTime.UtcNow);

        if(difference.TotalSeconds>12){
            startOfEp = DateTime.UtcNow;
            Done();
        }

        // Fell off platform
        if (this.transform.position.y < initposcar.y-2f){
            AddReward(-300f);
            startOfEp = DateTime.UtcNow;
            Done();
        }

    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Vertical");
        action[1] = Input.GetAxis("Horizontal");
        if(action[0]==-1)
            action[0]=2;
        else if(action[1]==-1)
            action[1]=2;
        return action;
    }


}
