using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    public float spreadMultiplier = 0.5f;
    private const float MAX_LENGTH = 50f;
    public Transform origin;
    public GameObject end;                                  // this is to allow access of various information from objects such as DamageableObjects
    public bool isOn = true;
    public float width = 0.05f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        origin = transform;
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            // raycast to get 
            lineRenderer.SetPosition(0, origin.position);
            RaycastHit2D hit = Physics2D.Raycast(origin.position,origin.right);
            if (hit.collider != null)
            {
               // Debug.Log("Found game object " + hit.collider.gameObject);
                end = hit.collider.gameObject;
                lineRenderer.SetPosition(1, hit.point);
            } else
            {
                end = null;
                lineRenderer.SetPosition(1, origin.right * MAX_LENGTH);
            }
            
        } else
        {
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero,Vector3.zero});
        }
    }
}
