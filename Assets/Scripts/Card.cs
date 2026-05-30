using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {
    public GameObject unitPrefab;

    public string cardName;
    public string description;

    public Sprite artwork;
    public GameObject unit;

    public int manaCost;
    public int attack;
    public int maxHealth;
    public int movement;
}
