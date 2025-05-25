using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAngle : MonoBehaviour
{
    // rotates the vector to within +/- anglevari degrees
    public static Vector3 GetTrajectory(Vector3 start, float angleVari)
    {
        return Quaternion.Euler(0, 0, PlusMinus(angleVari)) * start;

    }

    // rotates the given vector by angleAdjust
    public static Vector3 RotateVector(Vector3 start, float angleAdjust)
    {
        return Quaternion.Euler(0, 0, angleAdjust) * start;
    }

    // rotates a gameobject by +/- anglevari degrees
    public static Vector3 GetProjectileTrajectory(Vector3 start, float angleVari)
    {
        return Quaternion.AngleAxis(UnityEngine.Random.Range(-angleVari, angleVari), Vector3.forward) * start;

    }

    // returns a semirandom float between -variance and +variance inclusive
    public static float PlusMinus(float variance)
    {
        return Random.Range(-variance, variance);
    }
}
