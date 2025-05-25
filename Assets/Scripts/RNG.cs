using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG
{
    // return true or false semirandomly
    public static bool RandBool()
    {
        int randInt = Random.Range(0, 2);
        if (randInt == 1)
        {
            return true;
        }
        return false;
    }
}
