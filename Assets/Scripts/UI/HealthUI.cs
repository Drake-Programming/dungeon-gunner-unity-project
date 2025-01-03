using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHeartsList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    /// <summary>
    /// Handles health changed event
    /// </summary>
    /// <param name="healthEvent"></param>
    /// <param name="healthEventArgs"></param>
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);

    }

    /// <summary>
    /// Clears health bar
    /// </summary>
    private void ClearHealthBar()
    {
        foreach (GameObject heartIcon in healthHeartsList)
        {
            Destroy(heartIcon);
        }

        healthHeartsList.Clear();
    }

    /// <summary>
    /// Sets hearts to display health amount
    /// </summary>
    /// <param name="healthEventArgs"></param>
    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        // Instantiate heart image prefabs
        int healthHearts = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f / 20f);

        for (int i = 0; i < healthHearts; i++)
        {
            // Instantiate heart prefabs
            GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);

            // Position
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);

            healthHeartsList.Add(heart);
        }

    }
}