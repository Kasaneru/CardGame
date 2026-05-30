using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitTooltip : MonoBehaviour {
    public static UnitTooltip Instance;

    [SerializeField] private CardDisplay cardDisplay;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

   public void Show(Unit unit)
    {
        if (unit == null || unit.card == null) return;

        cardDisplay.SetCard(unit.card);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
