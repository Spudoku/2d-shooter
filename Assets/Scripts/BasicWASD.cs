using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWASD : MonoBehaviour
{
    
    public float speed = 1.5f;          // units per second
    public Vector2 mousePos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        // horizontal input for left/right, vertical input for forward/backward or up/down
        float horiz = Input.GetAxis("Horizontal") * speed;
        float vert = Input.GetAxis("Vertical") * speed;
        //Debug.Log("Horiz: " + horiz + "; Vert: " + vert);
        Vector2 movement = new Vector2(horiz, vert);
        movement = Vector2.ClampMagnitude(movement,speed) * Time.deltaTime;
        transform.Translate(movement);
        
        // input
        
    }

    
}
