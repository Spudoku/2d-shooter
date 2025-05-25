using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class 
public class NPCManager : MonoBehaviour
{
    //public string npcmName;
    public List<string> npcTags;
    public List<GameObject> gos;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        gos = new List<GameObject>();
        foreach(string tag in npcTags)
        {
            List<GameObject> tempList = new(GameObject.FindGameObjectsWithTag(tag));
            foreach(GameObject go in tempList)
            {
                gos.Add(go);
                if (go.TryGetComponent<NPCGeneric>(out var npc) && npc.manager == null)
                {
                    npc.manager = this;
                }
            }
        }
    }
}
