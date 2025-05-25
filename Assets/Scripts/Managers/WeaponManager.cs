using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(WeaponSwapper))]
[RequireComponent(typeof(AimLines))]
[RequireComponent(typeof(LineOfSight))]
public class WeaponManager : MonoBehaviour
{
    public Weapon[] weapons;
    private WeaponSwapper swapper;
    [SerializeField] private Weapon selected;
    [ SerializeField] private PlayerManager playerManager;
    [SerializeField] private TMP_Text ammoTextUI;
    [SerializeField] private TMP_Text selectedWeaponUI;
    [SerializeField] private TMP_Text playerHPUI;
    [SerializeField] private KeyCode reloadKey;
    private AimLines aimLines;

    private IEnumerator reloadCoroutine;

    // call allies vars
    [SerializeField] private float callDist = 15f;
    [SerializeField] private float callInterval = 5f;
    [SerializeField] private RandomSFX callSFX;
    private float callTimeTrack;
    [SerializeField] private KeyCode CallKey;

    private LineOfSight los;
    // Start is called before the first frame update
    void Start()
    {
        swapper = GetComponent<WeaponSwapper>();
        aimLines = GetComponent<AimLines>();
        callTimeTrack = callInterval;
        los = GetComponent<LineOfSight>();
    }

    // Update is called once per frame
    void Update()
    {
        callTimeTrack += Time.deltaTime;

        swapper.ChangeSelectedIndex(Mathf.RoundToInt(Input.mouseScrollDelta.y));
        // update UI
        

        
        selected = swapper.GetSelected();
        aimLines.SetWeapon(selected);
        UpdateUI();
        if (selected.GetType() == typeof(MagBased) && !selected.isReloading)
        {
            MagBased magBased = (MagBased)selected;
            
            if (magBased.isAutomatic && Input.GetButton("Fire1"))
            {
                if (magBased.ammoLoaded >= 1)
                {
                    magBased.Fire();
                    if (magBased.ammoLoaded <= 0)
                    {

                        //StartCoroutine(ReloadWeapon(magBased, magBased.GetReloadTime()));
                        magBased.ReloadInit();
                    }
                }
                else
                {
                    //StartCoroutine(ReloadWeapon(magBased, magBased.GetReloadTime()));
                    magBased.ReloadInit();
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                if (magBased.ammoLoaded >= 1)
                {
                    magBased.Fire();
                    if (magBased.ammoLoaded <= 0)
                    {

                        //StartCoroutine(ReloadWeapon(magBased, magBased.GetReloadTime()));
                        magBased.ReloadInit();
                    }
                }
                else
                {
                    //StartCoroutine(ReloadWeapon(magBased, magBased.GetReloadTime()));
                    magBased.ReloadInit();
                }

            }

            // reloading
            if (Input.GetKeyDown(reloadKey))
            {
                if (magBased.ammoCarried == 0)
                {
                    Debug.Log("Reload impossible; no ammo carried for " + magBased.weaponName);
                }
                else if (magBased.ammoLoaded < magBased.magSize)
                {
                    //reloadCoroutine = ReloadWeapon(magBased, magBased.GetReloadTime());
                    //StartCoroutine(reloadCoroutine);
                    magBased.ReloadInit();
                    
                }
                else
                {
                    Debug.Log("Reload of " + magBased.weaponName + " aborted; magazine already full");
                }

            }
        }
        else
        {
            // non mag-based weapons, such as miniguns, flamethrowers, etc
        }

        // laser pointers
        if (selected.gameObject.TryGetComponent<LaserPointer>(out var lp))
        {
            lp.isOn = true;
        }

        if (Input.GetKeyDown(CallKey) && callTimeTrack > callInterval)
        {
            CallAllies();
        }
        if (Camera.main.TryGetComponent<FollowCam>(out var fc))
        {
            //Debug.Log($"Setting camera's aim range to {selected.aimRange}!");
            fc.SetCurDist(selected.aimRange);
        }
    }


    private void UpdateUI()
    {
        ammoTextUI.text = selected.ammoLoaded + "/" + selected.ammoCarried;
        selectedWeaponUI.text = selected.weaponName;
        playerHPUI.text = playerManager.hp.ToString();
    }

    private void CallAllies()
    {
        callTimeTrack = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, callDist);

        if (callSFX != null)
        {
            AudioSource.PlayClipAtPoint(callSFX.GetRandomClip(), transform.position);
        }
        foreach (var collider in colliders)
        {
            //Debug.Log($"{myName}: testing {collider}");
            if (!collider.gameObject.Equals(gameObject) && collider.gameObject.TryGetComponent<DumbGunman>(out var dm) && dm.allyTags.Contains(gameObject.tag))
            {

                dm.SetFollowAlly(gameObject);
            }
        }
    }

}
