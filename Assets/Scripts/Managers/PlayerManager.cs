using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : DamageableObject
{

    private const int DEBUG_DMG = 10;
    
    protected override void LethalFX()
    {

    }

    private new void Update()
    {
        
        base.Update();
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeHP(-DEBUG_DMG);
        }
    }
}
