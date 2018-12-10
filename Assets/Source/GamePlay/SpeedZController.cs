using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Using function speed = speedLimit * ( 1 - 1/(timeCoefficient^timeExponential + 1) to calculate speed
 */
public class SpeedZController : MonoBehaviour
{

    public float timeExponential = 1;
    public float timeCoefficient = .1f;
    public float speedLimit = 5;

    void Update()
    {
        GetComponent<MovementComponent>().RequestMoveForwardAuto(ComputeSpeed());
    }

    private float ComputeSpeed()
    {
        return speedLimit * (1 - 1 / (timeCoefficient * Mathf.Pow(Time.time, timeExponential) + 1));
    }
}
