using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, 
        Vector3 weaponAimDirectVector, bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}