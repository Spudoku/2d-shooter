using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
// used by enemies to find the player or their allies
public class TargetFinder : MonoBehaviour
{
    public float searchRange;               // radius to search in
    public GameObject target;
    public bool canSeeTarget = false;
    public string[] targetTags;
    public string[] allyTags;
    private LineOfSight los;

    //[SerializeField] private Dictionary<GameObject,bool> possibleTargets = new Dictionary<GameObject,bool>();        // a list of targets (tagged as "Player" or "PlayerAlly"), and whether they can currently be seen or not.
    [SerializeField] private HashSet<GameObject> possibleTargets = new();

    // Start is called before the first frame update
    void Start()
    {
        //
        los = GetComponent<LineOfSight>();
        los.lookRange = searchRange;
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
        HashSet<GameObject> oldPosTargets = new(possibleTargets);
        foreach (GameObject go in oldPosTargets)
        {
            if (go == null)
            {
                //Debug.Log(gameObject + " update targets: " +  go  + " == null (removing " + go + ")");
                possibleTargets.Remove(go);
            }
            else if (Vector2.Distance(transform.position, go.transform.position) > searchRange)
            {
                // Debug.Log(gameObject + " update targets: " + go + " is too far! (removing " + go + ")");
                possibleTargets.Remove(go);
            }
            else if (!los.CanSeeGO(go, searchRange))
            {
                //Debug.Log(gameObject + " update targets: " + go + " is obscured! (removing " + go + ")");
                possibleTargets.Remove(go);
            }
        }

        // add new targets from all nearby colliders
        Collider2D[] allNearby = Physics2D.OverlapCircleAll(transform.position, searchRange);
        foreach (Collider2D collider in allNearby)
        {
            GameObject go = collider.gameObject;
            if (IsTaggedTarget(go) && los.CanSeeGO(go, searchRange))
            {
                possibleTargets.Add(go);

            }
        }




    }

    // returns the nearest target that can currently be seen
    private GameObject GetNearestTarget(HashSet<GameObject> s)
    {
        GameObject curTarget = target;
        if (s.Count == 0)
        {
            //Debug.Log(gameObject + " target check: s.count == 0! (returning null)!");
            return null;
        }
        else if ((target != null && !s.Contains(target)))
        {
            //Debug.Log(gameObject + " target check: target is not null and s does not contain target! (returning null)!");
            return null;
        }
        else
        {
            float curDistance = Mathf.Infinity;
            Vector2 curPos = transform.position;
            foreach (GameObject go in s)
            {
                Vector2 dirToGO = (Vector2)go.transform.position - curPos;
                float distance = dirToGO.sqrMagnitude;
                if (distance < curDistance)
                {
                    curDistance = distance;
                    curTarget = go;
                }
            }
        }
        //Debug.Log(gameObject + " target check: returning " + curTarget + "!");
        return curTarget;
    }

    private GameObject FindNearbyAlly()
    {
        return null;
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
