using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
// this class allows GameObjects to be attached to NPCManagers.
// it will also add other universal features for NPCs 
public class NPCGeneric : MonoBehaviour
{
    
    public NPCManager manager = null;
    private LineOfSight los;

    public void Awake()
    {
        //manager = FindNPCManager(gameObject.tag);
        los = GetComponent<LineOfSight>();
    }
    // find the NPCManager with string tagName. Enemies should
    // find the one named "Enemy", while playerAllies
    /*private NPCManager FindNPCManager(string tagName)
    {
        NPCManager[] npcManagers = FindObjectsByType<NPCManager>(FindObjectsSortMode.None);

        foreach(NPCManager n in npcManagers)
        {
            if(n.npcTags.Contains(gameObject.tag))
            {
                return n;
            }
        }
        return null;
    }*/

    // get the number of gos (stored in this instances manager field) within radius of this gameObject.
    // if includeThis is set to true, include this instance in the count
    // may 
    public int GetNPCsInRadius(float radius, bool includeThis = false, bool requireLOS = true)
    {
        if (radius <= 0f)
        {
            throw new UnityException("GetNPCsInRadius: radius must be greater than 0");
        }
        int count = 0;
        if (manager != null)
        {
            // test each gameObject in its managers gos list.
            foreach (GameObject go in manager.gos)
            {
                if (go != null && Vector2.Distance(transform.position,go.transform.position) <= radius)
                {
                    if (!requireLOS)
                    {
                        count++;
                    }
                    else if (los.CanSeeGO(go,radius))
                    {
                        count++;
                    }

                }
            }
            if (!includeThis)
            {
                count--;
            }
        }
        
        return count;
    }


}
