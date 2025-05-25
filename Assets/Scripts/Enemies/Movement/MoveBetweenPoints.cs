using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [SerializeField] private Vector2[] points;
    [SerializeField] private float speed;

    private int target;
    // Start is called before the first frame update
    void Start()
    {
        target = 1;
        transform.position = points[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (target > points.Length - 1) {
            target = 0;
        }
        float step = speed * Time.deltaTime;

        //Debug.Log("Moving towards point " + target + "...");
        transform.position = Vector3.MoveTowards(transform.position, points[target], step);
        if (Vector3.Distance(transform.position, points[target]) < step)
        {
            target++;
        }
    }
}
