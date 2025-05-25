using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetGO;
    [SerializeField] private float spawnDelay;          // delay between spawns, in seconds

    private float curDelay;
    // Start is called before the first frame update
    void Start()
    {
        curDelay = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        curDelay -= Time.deltaTime;

        if (curDelay < 0 )
        {
            // spawn enemy
            GameObject newTarget = Instantiate(targetGO);
            newTarget.transform.position = transform.position;
            curDelay = spawnDelay;
        }

    }
}
