using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LookAtMouse();
    }
    private void LookAtMouse()
    {
        Camera cam = Camera.main;
        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = transform.position.z - cam.transform.position.z;

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));

        float angle = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg;
        //float angle = (180 / Mathf.PI) * AngleRad;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
