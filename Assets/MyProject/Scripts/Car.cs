using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Agent
{
    private float _speed;

    [SerializeField]
    private Transform _spawnPosition;

    [SerializeField]
    private Transform _goalPosition;

    private RayPerception3D _rayPerception;

    private float _time;

    private void Awake()
    {
        _speed = 10f;
        _rayPerception = GetComponentInChildren<RayPerception3D>();

    }

    // Update is called once per frame
    void Update()
    {
        //GetCumulativeReward().ToString("0.00");
        _time += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "wall":
                AddReward(-20f);
                AgentReset();
                break;

        }


    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {


            case "end":
                Debug.Log("lap completed");
                break;
        }
        AddReward(10f);

    }

    public override void AgentReset()
    {
        transform.position = _spawnPosition.position;
        transform.rotation = Quaternion.identity;
        _time = 0;
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Convert actions to axis values
        float forward = 1;
        float leftOrRight = 0f;
        if (vectorAction[0] == 1f)
        {
            leftOrRight = -1f;
        }
        else if (vectorAction[0] == 2f)
        {
            leftOrRight = 1f;
        }

        moveCar(forward, leftOrRight);

        // Tiny negative reward every step
    }

    public override void CollectObservations()
    {


        // Direction penguin is facing
        AddVectorObs(transform.forward);

        // RayPerception (sight)
        // ========================
        // rayDistance: How far to raycast
        // rayAngles: Angles to raycast (0 is right, 90 is forward, 180 is left)
        // detectableObjects: List of tags which correspond to object types agent can see
        // startOffset: Starting height offset of ray from center of agent
        // endOffset: Ending height offset of ray from center of agent
        float rayDistance = 5f;
        float[] rayAngles = { 30f, 60f, 90f, 120f, 150f };
        string[] detectableObjects = { "wall" , "checkpoint"};
        AddVectorObs(_rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0, 0f));


    }

    private void moveCar(float pForward, float pLeftOrRight)
    {
        Vector3 inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, pForward * 11.4f), 0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;

        transform.eulerAngles += new Vector3(0, (pLeftOrRight * 90) * 0.02f, 0);

        AddReward(0.01f);
    }



}
