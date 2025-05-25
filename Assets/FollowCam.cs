using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private const float MIN_DIST_FROM_TARGET = 15f;             // how far from the target mousePos must be before moving the camera
    private float curMax = 15f;
    Camera cam;
    [SerializeField] private Vector3 screenPosition;
    [SerializeField] private Vector3 mousePos;
    [SerializeField] private float camMoveSpeed = 20f;          // how fast the camera will move towards the mouse


    // *** THESE FIELDS FOR TESTING ONLY *** //
    //[SerializeField] private Vector3 mousePosAfterAdjust;
    //[SerializeField] private Vector3 centerOfScreen;
    //[SerializeField] private Vector3 dirTest;
    [SerializeField] private float distTest;
    private void Start()
    {
        cam = Camera.main;
    }

    
    // Update is called once per frame
    void LateUpdate()
    {
        if (curMax < MIN_DIST_FROM_TARGET)
        {
            curMax = MIN_DIST_FROM_TARGET;
        }
        // move camera to target
        Vector3 targetPos = new(target.transform.position.x, target.transform.position.y, -10f);
        transform.position = targetPos;

        screenPosition = Input.mousePosition;
        mousePos = cam.ScreenToWorldPoint(new(screenPosition.x, screenPosition.y, 0f));

        // adjust mousePos so the camera doesn't move too far.
        float mouseDist = Vector3.Distance(targetPos, mousePos);
        Debug.Log($"FollowCam: distance from mouse to target is {mouseDist}");
        if (mouseDist > curMax)
        {
            Vector3 dir = (mousePos - targetPos).normalized;
            transform.position = targetPos + dir * curMax;
        }
        else
        {
            transform.position = mousePos;
        }

    }
    public void SetCurDist(float amt)
    {
        if (amt >= MIN_DIST_FROM_TARGET)
        {
            curMax = amt;
        } else
        {
            curMax = MIN_DIST_FROM_TARGET;
        }
    }


    // pre: v must be the mouse's SCREEN position
    private float CalcDistFromScreenEdge(Vector3 v)
    {
        Vector3 adjustedMousPos = v;

        float halfHeight;

        if (Screen.height > Screen.width)
        {
            halfHeight = Screen.width / 2f;
        } else
        {
            halfHeight = Screen.height / 2f;
        }
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0);
        
        Vector3 dir = (v - screenCenter).normalized;
        //dirTest = dir;
        if (Vector3.Distance(v,screenCenter) > halfHeight)
        {
            adjustedMousPos = screenCenter + dir * halfHeight;
        }
        //Debug.DrawLine(Camera.main.ScreenToWorldPoint(v), Camera.main.ScreenToWorldPoint(screenCenter), Color.red, 5f);
        //Debug.DrawLine(Camera.main.ScreenToWorldPoint(adjustedMousPos), Camera.main.ScreenToWorldPoint(screenCenter),Color.green,5f);
        

        
        

        return Vector3.Distance(adjustedMousPos,screenCenter) / halfHeight;
    }
}




// Old versions of camera following the mouse:
// *** OLD VERSION *** //
/*if (curMax < MIN_DIST_FROM_TARGET)
{
    curMax = MIN_DIST_FROM_TARGET;
}
// move camera to target
Vector3 targetPos = new(target.transform.position.x, target.transform.position.y, -10f);
transform.position = targetPos;

screenPosition = Input.mousePosition;
mousePos = cam.ScreenToWorldPoint(new(screenPosition.x, screenPosition.y, 0f));

// adjust mousePos so the camera doesn't move to far.
float mouseDist = Vector3.Distance(targetPos, mousePos);
Debug.Log($"FollowCam: distance from mouse to target is {mouseDist}");
if (mouseDist > curMax)
{
    Vector3 dir = (mousePos - targetPos).normalized;
    transform.position = targetPos + dir * curMax;
}
else
{
    transform.position = mousePos;

}*/
// *** END OLD VERSION *** //


// *** BEGIN VERSION 2 *** //

/*if (curMax < MIN_DIST_FROM_TARGET)
{
    curMax = MIN_DIST_FROM_TARGET;
}
// step 1: move camera to target postion
Vector3 targetPos = new(target.transform.position.x, target.transform.position.y, transform.position.z);
//transform.position = targetPos;


// step 2: determine how far from the edge of the screen
screenPosition = Input.mousePosition;
float dist = CalcDistFromScreenEdge(screenPosition);        // distance of the MOUSE from center of the screen as a percent (0 to 1)

distTest = dist;
// calculate direction from the mouse to the target
Vector3 dir = (Camera.main.ScreenToWorldPoint(screenPosition) - targetPos).normalized;
// calculate a point for the camera to move towards
Vector3 movePoint = targetPos + (curMax * dist * dir);
// move the camera in that direction
transform.position = movePoint;
//Vector3.MoveTowards(transform.position, movePoint, camMoveSpeed * Time.deltaTime);
if (dist > 0.25f)
{

} else
{
    //Vector3.MoveTowards(transform.position, targetPos, camMoveSpeed * Time.deltaTime);

}*/


// *** END VERSION 2 *** //

// *** BEGIN VERSION 3 *** //

/*if (curMax < MIN_DIST_FROM_TARGET)
{
    curMax = MIN_DIST_FROM_TARGET;
}
// step 1: move camera to target postion
Vector3 targetPos = new(target.transform.position.x, target.transform.position.y, transform.position.z);
//transform.position = targetPos;


// step 2: determine how far from the edge of the screen
screenPosition = Input.mousePosition;
float dist = CalcDistFromScreenEdge(screenPosition);        // distance of the MOUSE from center of the screen as a percent (0 to 1)

distTest = dist;

if (dist > 0.25f)
{
    // calculate direction from the mouse to the target
    Vector3 dir = (Camera.main.ScreenToWorldPoint(screenPosition) - targetPos).normalized;
    // calculate a point for the camera to move towards
    Vector3 movePoint = targetPos + (curMax * dist * dir);
    // move the camera in that direction
    transform.position = movePoint;
    //Vector3.MoveTowards(transform.position, movePoint, camMoveSpeed * Time.deltaTime);
}
else
{
    Vector3.MoveTowards(transform.position, targetPos, camMoveSpeed * Time.deltaTime);

}*/