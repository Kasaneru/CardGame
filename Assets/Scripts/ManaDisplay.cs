using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaDisplay : MonoBehaviour {
    private Image manaImage;
    public TMP_Text manaText;

    public int player;

    private void Update()
    {
        if (player == 1) {
            manaText.text = GameManager.Instance.p1Mana.ToString();
        } else if (player == 2) {
            manaText.text = GameManager.Instance.p2Mana.ToString();
        }
    }
}
