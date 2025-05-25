using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
// note: abbreviated as "PU"
// This class represents 
public abstract class Pickup : MonoBehaviour
{
    public string myName;                               // used for telling the player what they picked up
    public RandomSFX puSFX;                             // sound(s) to play when picked up
    public bool isStealable = false;                    // if true, enemies can pick up this PU
    // Start is called before the first frame update
    void Awake()
    {
        int LayerPickup = LayerMask.NameToLayer("Pickup");
        gameObject.layer = LayerPickup;
        transform.position = new(transform.position.x, transform.position.y, 0.1f);
    }

    protected abstract void PickupPU(GameObject go);
    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (isStealable)
        {
            if (go.TryGetComponent<DamageableObject>(out var DO))
            {
                PickupPU(go);
                Debug.Log($"{go} picked up {myName}!");
                AudioSource.PlayClipAtPoint(puSFX.GetRandomClip(), transform.position);
                Destroy(gameObject);
            }
        } else if (go.TryGetComponent<PlayerManager>(out var DO))
        {
            PickupPU(go); Debug.Log($"{go} picked up {myName}!");
            AudioSource.PlayClipAtPoint(puSFX.GetRandomClip(), transform.position);
            Destroy(gameObject);
        }
        
    }
}
