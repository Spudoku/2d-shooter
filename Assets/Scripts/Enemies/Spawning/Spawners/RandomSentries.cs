using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSentries : MonoBehaviour
{
    [SerializeField] private int num;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2 center;
    [SerializeField] private bool usePosition = false;
    [SerializeField] private float maxDistance;         // maximum radius from center
    [SerializeField] private bool isSquare;
    private int curNum;
    // Start is called before the first frame update
    void Start()
    {
        
        float prefabRadius = enemyPrefab.transform.localScale.x; 
        curNum = 0;
        if (usePosition)
        {
            center = transform.position;
        }
        Vector2 point = transform.position;
        while (curNum < num)
        {
            
            bool isValid = false;
            while (!isValid)
            {
                if (isSquare)
                {
                    point = new Vector2(Random.Range(-maxDistance, maxDistance ), Random.Range(-maxDistance, maxDistance)) + center;
                } else
                {
                    point = center + (Random.insideUnitCircle * maxDistance);
                }
                
                if (!Physics2D.OverlapCircle(point,prefabRadius))
                {
                    isValid = true;
                }
            }
            PlaceEnemy(point);
            curNum++;
            
            
        }
    }
    private void Update()
    {
        center = transform.position;
    }
    private void PlaceEnemy(Vector2 point)
    {
        GameObject newEnemy=  Instantiate(enemyPrefab);
        newEnemy.transform.position = point;
    }
}
