using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TargetAreaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetGO;
    [SerializeField] private float spawnDelay;          // delay between spawns, in seconds
    [SerializeField] private float maxDistance;         // maximum radius from center
    [SerializeField] private bool isSquare;
    private float prefabRadius;
    private float curDelay;
    [SerializeField] private Vector2 center;
    // Start is called before the first frame update
    void Start()
    {
        curDelay = 0;
        prefabRadius = targetGO.transform.localScale.x;
        
    }

    // Update is called once per frame
    void Update()
    {
        center = transform.position;
        curDelay -= Time.deltaTime;

        if (curDelay < 0 )
        {
            Vector2 point = transform.position;
            bool isValid = false;
            while (!isValid)
            {
                if (isSquare)
                {
                    point = new Vector2(Random.Range(-maxDistance, maxDistance), Random.Range(-maxDistance, maxDistance)) + center;
                }
                else
                {
                    point = center + (Random.insideUnitCircle * maxDistance);
                }

                if (!Physics2D.OverlapCircle(point, prefabRadius))
                {
                    isValid = true;
                }
            }
            PlaceEnemy(point);
            curDelay = spawnDelay;
        }

    }
    private void PlaceEnemy(Vector2 point)
    {
        GameObject newEnemy = Instantiate(targetGO);
        newEnemy.transform.position = point;
    }
}
