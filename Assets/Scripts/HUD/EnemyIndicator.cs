using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    public const float SIGHT_RANGE = 80f;
    public GameObject target;                   // the object to indicate
    public float alpha = 1.0f;                  // how visible this object is
    public LineOfSight los;                     // the LineOfSight object to use (and its parent GO)
    private const float DIM_RATE = 3.0f;         // how much of alpha to deplete per second when target can no longer be seen
    private Vector2 targetOldPos;
    private Material material;
    [SerializeField] private Color visibleColor;
    private Color invisibleColor;
    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        //invisibleColor = visibleColor;
        //invisibleColor.a = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (los.CanSeeGO(target, SIGHT_RANGE))
        {
            alpha = 1.0f;
        }
        UpdateAlpha();
    }

    private void Dim()
    {

    }

    public void InitializeEI(GameObject go, LineOfSight los)
    {
        alpha = 0f;
        target = go;
        this.los = los;
    }

    private void UpdateAlpha()
    {
        Color newColor = visibleColor;
        newColor.a = alpha;
        material.SetColor("_Color",newColor);
    }
}
