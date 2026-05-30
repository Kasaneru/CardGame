using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour {
    public Card card;

    public TMP_Text nameText;
    public TMP_Text descText;

    public Image artImage;

    public TMP_Text costText;
    public TMP_Text attackText;
    public TMP_Text maxHealthText;
    public TMP_Text movementText;

    public void SetCard(Card _card) {
        card = _card;
        nameText.text = _card.cardName;
        descText.text = _card.description;

        artImage.sprite = _card.artwork;

        costText.text = _card.manaCost.ToString();
        attackText.text = _card.attack.ToString();
        maxHealthText.text = _card.maxHealth.ToString();
        movementText.text = _card.movement.ToString();
    }
}
