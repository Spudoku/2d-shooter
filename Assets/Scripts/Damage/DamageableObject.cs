using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class DamageableObject : MonoBehaviour
{
    public float maxHP;
    public float hp;         // how much damage this object can take
    
    [SerializeField] protected float armor;      // how much damage (flat amount) 
    public const float MIN_DAMAGE = 1f;  // the minimum amount of damage any DamageableObject can take
    public const float RECENT_HIT_TIME = 3f;   // amount of time since the last hit for it to be considered 'recent'
    public float timeSinceLastHit;
    public float lastHitDmg;

    // auto-healing
    [SerializeField] protected float healAmount = 0f;
    [SerializeField] protected float healInterval = 10f;
    [SerializeField] protected float timeToHeal;
    // Sounds
    [SerializeField] protected float genericVolume;
    [SerializeField] protected RandomSFX destructionSounds;
    [SerializeField] protected RandomSFX dmgSounds;
    [SerializeField] protected RandomSFX healSounds;
    [SerializeField] protected const float MIN_TIME_BETWEEN_HITSOUNDS = 0.05f;
    protected float curSoundDelay;

    protected void Awake()
    {
        hp = maxHP;
        curSoundDelay = 0f;
        timeSinceLastHit = RECENT_HIT_TIME;
    }
    public void ChangeHP(float amount)
    {
        if (amount == 0)
        {
            // do nothing?
        } else if (amount < 0 && Mathf.Abs(amount) < armor)
        {
            hp -= MIN_DAMAGE;
            timeSinceLastHit = 0f;
            lastHitDmg = MIN_DAMAGE;
        } else
        {
            if (amount > 0)
            {
                // Healing health
                if (healSounds != null)
                {
                    AudioSource.PlayClipAtPoint(healSounds.GetRandomClip(), transform.position, genericVolume);
                }
                hp += amount;
                if (hp > maxHP)
                {
                    hp = maxHP;
                }
            } else 
            {
                // Taking damage
                if (curSoundDelay <= 0)
                {
                    if (dmgSounds != null)
                    {
                        AudioSource.PlayClipAtPoint(dmgSounds.GetRandomClip(), transform.position, genericVolume);
                    }
                    
                    curSoundDelay = MIN_TIME_BETWEEN_HITSOUNDS;
                }
                timeSinceLastHit = 0f;
                lastHitDmg = armor + amount;
                hp += (armor + amount);
            }
            
            
        }

        if (hp <= 0)
        {
            LethalFX();
            if (destructionSounds != null)
            {
                AudioSource.PlayClipAtPoint(destructionSounds.GetRandomClip(), transform.position, genericVolume);
            }
            

            if (this is PlayerManager)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } else
            {
                Destroy(gameObject);
            }
            
        }
    }
    protected void Update()
    {
        // auto-healing
        if (Time.time > timeToHeal)
        {
            ChangeHP(healAmount);
            timeToHeal = Time.time + healInterval;
        }

        curSoundDelay -= Time.deltaTime;
        if (curSoundDelay <= 0)
        {
            curSoundDelay = 0;
        }
        timeSinceLastHit += Time.deltaTime;
    }

    // 
    protected abstract void LethalFX();
    

    
}
