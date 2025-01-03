using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("Populate with image component on the child WeaponImage gameObject")]
    #endregion Tooltip
    [SerializeField] private Image weaponImage;

    #region Tooltip
    [Tooltip("Populate with the Transform from the child AmmoHolder gameObject")]
    #endregion Tooltip
    [SerializeField] private Transform ammoHolderTransform;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameObject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI reloadText;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameObject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI ammoRemainingText;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameObject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI weaponNameText;

    #region Tooltip
    [Tooltip("Populate with the RectTransform of the child gameObject ReloadBar")]
    #endregion Tooltip
    [SerializeField] private Transform reloadBar;

    #region Tooltip
    [Tooltip("Populate with the Image component of the child gameObject BarImage")]
    #endregion Tooltip
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        // Get player
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        // Subscribe to set active weapon event 
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        // Subscribe to set weapon fired event 
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        // Subscribe to reload weapon event 
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        // Subscribe to set active weapon event 
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from set active weapon event 
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        // Unsubscribe from set weapon fired event 
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        // Unsubscribe from reload weapon event 
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        // Unsubscribe from set active weapon event 
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        // Update active weapon status on the UI
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    /// <summary>
    /// Handle set active weapon event on the UI
    /// </summary>
    /// <param name="setActiveWeaponEvent"></param>
    /// <param name="setActiveWeaponEventArgs"></param>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Handle Weapon fired event on the UI
    /// </summary>
    /// <param name="weaponFiredEvent"></param>
    /// <param name="weaponFiredEventArgs"></param>
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    /// <summary>
    /// Weapon fired update UI
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Handle Weapon reload event on the UI
    /// </summary>
    /// <param name="reloadWeaponEvent"></param>
    /// <param name="reloadWeaponEventArgs"></param>
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Handle Weapon reloaded event on the UI
    /// </summary>
    /// <param name="weaponReloadedEvent"></param>
    /// <param name="weaponReloadedEventArgs"></param>
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    /// <summary>
    /// Weapon has been reloaded - update UI if current weapon
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponReloaded(Weapon weapon)
    {
        // If weapon reloaded is the current weapon
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    /// <summary>
    /// Set the active weapon on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeapon(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // If set weapon is still reloading then update reload bar
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Populate active weapon image
    /// </summary>
    /// <param name="weaponDetails"></param>
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    /// <summary>
    /// Populate active weapon name
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateActiveWeapon(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    /// <summary>
    /// Update the ammo remaining text on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    /// <summary>
    /// Update ammo clip icons on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++){
            // Instantiate ammo icon prefab
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// Clear ammo icons
    /// </summary>
    private void ClearAmmoLoadedIcons()
    {
        // Loop through icon gameObjects and destroy
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// <summary>
    /// Reload weapon - update the reload bar on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// <summary>
    /// Animate reload weapon bar coroutine
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        // Set the reload bat to red
        barImage.color = Color.red;

        // Animate the weapon reload bar
        while (currentWeapon.isWeaponReloading)
        {
            // update reload bar
            float barfill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            // update bar fill
            reloadBar.transform.localScale = new Vector3(barfill, 1f, 1f);

            yield return null;
        }
    }

    /// <summary>
    /// Initialize the weapon reload bar on the UI
    /// </summary>
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        // Set bar color as green
        barImage.color = Color.green;

        // Set bar scale to 1
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// Stop coroutine updating weapon reload progress bar
    /// </summary>
    private void StopReloadWeaponCoroutine()
    {
        // Stop any active weapon reload bar on the UI
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            // set the reload bar to red
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextCoroutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// <summary>
    /// Start the coroutine to blink the reload weapon text
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBlinkingReloadTextCoroutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// Stop the blinking reload text
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    /// <summary>
    /// Stop the blinking reload text coroutine
    /// </summary>
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
    #endregion Validation
}
