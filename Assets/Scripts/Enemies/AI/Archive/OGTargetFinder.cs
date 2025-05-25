/*using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
// used by enemies to find the player or their allies
public class TargetFinder : MonoBehaviour
{
    public float searchRange;               // radius to search in
    public GameObject target;
    public bool canSeeTarget = false;
    public string[] targetTags;
    private LineOfSight los;

    [SerializeField] private Dictionary<GameObject, bool> possibleTargets = new Dictionary<GameObject, bool>();        // a list of targets (tagged as "Player" or "PlayerAlly"), and whether they can currently be seen or not.

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
        target = GetNearestTarget(possibleTargets);
    }

    // All targets must have collider2ds in order to be found
    private void UpdateTargets()
    {
        // clear possibleTargets of keys too far from this object
        Dictionary<GameObject, bool> oldPosTargets = new Dictionary<GameObject, bool>(possibleTargets);
        foreach (KeyValuePair<GameObject, bool> keyValue in oldPosTargets)
        {
            if (keyValue.Key != null && Vector2.Distance(transform.position, keyValue.Key.transform.position) > searchRange)
            {
                possibleTargets.Remove(keyValue.Key);
            }
        }

        // add new targets from all nearby colliders
        Collider2D[] allNearby = Physics2D.OverlapCircleAll(transform.position, searchRange);
        foreach (Collider2D collider in allNearby)
        {

            if (IsTaggedTarget(collider.gameObject) && !possibleTargets.ContainsKey(collider.gameObject))
            {
                possibleTargets.Add(collider.gameObject, false);

            }
        }


        // update true/false values of possibleTargets

        oldPosTargets = new Dictionary<GameObject, bool>(possibleTargets);
        foreach (KeyValuePair<GameObject, bool> keyValue in oldPosTargets)
        {
            // draw a ray towards the key GO and check to see if something is in the way
            // (a different GO/collider than expected);
            if (keyValue.Key != null)
            {
                //Debug.DrawRay(transform.position, (keyValue.Key.transform.position - transform.position), Color.blue, 5f);
                //Debug.DrawRay(, keyValue.Key.transform.position - transform.position , Color.green, 5f);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + 1.1f * transform.localScale.x * (keyValue.Key.transform.position - transform.position).normalized, (keyValue.Key.transform.position - transform.position) + (keyValue.Key.transform.position - transform.position) * transform.localScale.x, searchRange);
                if (hit.collider != null && hit.collider.gameObject.Equals(keyValue.Key))
                {
                    //Debug.Log(gameObject.name + ": can see target " + keyValue.Key.name);
                    possibleTargets[keyValue.Key] = true;
                }
                else
                {
                    //Debug.Log(gameObject.name + ": cannot see target " + keyValue.Key.name + "; actual object seen was " + hit.collider.gameObject.name);
                    possibleTargets[keyValue.Key] = false;
                }
            }

        }

    }

    // returns the nearest target that can currently be seen
    private GameObject GetNearestTarget(Dictionary<GameObject, bool> d)
    {
        float curDistance = Mathf.Infinity;
        GameObject curTarget = null;
        foreach (KeyValuePair<GameObject, bool> keyValue in d)
        {

            if (keyValue.Key != null)
            {
                //Debug.Log(gameObject.name + ": looking at object " + keyValue.Key.name + " as a target!");
                float keyValueDist = Vector2.Distance(transform.position, keyValue.Key.transform.position);
                if (keyValue.Value && keyValueDist < curDistance)
                {
                    curDistance = keyValueDist;
                    curTarget = keyValue.Key;
                }
            }

        }

        return curTarget;
    }

    private bool IsTaggedTarget(GameObject go)
    {
        foreach (string tag in targetTags)
        {
            if (go.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}
*/