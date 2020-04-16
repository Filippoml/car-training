using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAcademy : Academy
{
    [HideInInspector]
    public float Punishment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void AcademyReset()
    {
        base.AcademyReset();
        Punishment = resetParameters["wall_punishment"];
    }
}
