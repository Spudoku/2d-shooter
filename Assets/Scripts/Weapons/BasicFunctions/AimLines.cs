using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
// This class uses a LineRenderer to draw a line
// that shows 
public class AimLines : MonoBehaviour
{
    
    public Weapon weapon;
    [SerializeField] private LineRenderer lineRenderer;
    public float width = 0.05f;
    [SerializeField] Vector3 mouseWorldPos;
    [SerializeField] Vector3 origin;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private Vector3 point1;
    [SerializeField] private Vector3 point2;
    public Vector3[] points;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon != null)
        {
            CalcPoints(weapon.curSpread);

        } else
        {
            point1 = new();
            point2 = new();
        }
        points = new Vector3[] {point1, point2};
        
        lineRenderer.SetPositions(points);
        //Debug.DrawLine(point1, point2, Color.red, 2f);
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    private void CalcPoints(float angle)
    {
        // 
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        origin = weapon.gameObject.transform.position;
        Vector3 normal = origin - mouseWorldPos;
        float length = Vector3.Distance(origin, mouseWorldPos) * Mathf.Tan(Mathf.Deg2Rad * angle);
        //Debug.Log("Aim line info; angle " + angle + "; distance from origin to mouseWorldPos: " + Vector3.Distance(origin, mouseWorldPos) + "; calculated length: " + length) ;
        point1 = new Vector3(normal.y, -normal.x,0).normalized * length + mouseWorldPos;
        point2 = new Vector3(-normal.y, normal.x,0).normalized * length + mouseWorldPos;
        point1.z = -1;
        point2.z = -1;

    }
}
