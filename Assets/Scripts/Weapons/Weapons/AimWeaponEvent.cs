using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    /// <summary>
    /// Event that signals if the player or enemy is aiming
    /// </summary>
    /// <param name="aimDirection"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, 
            weaponAimDirectionVector = weaponAimDirectionVector });
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
