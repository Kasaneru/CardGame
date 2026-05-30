using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGamePanel : MonoBehaviour {
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color loseColor = Color.red;

    private void Awake() {
        if (panelRoot == null)
            panelRoot = gameObject;
        panelRoot.SetActive(false);
    }

    public void Show(int winner) {
        panelRoot.SetActive(true);

        if (backgroundImage != null) {
            backgroundImage.color = (winner == 1) ? winColor : loseColor;
        }

        if (resultText != null) {
            resultText.text = (winner == 1) ? "Вы победили!" : "Вы проиграли!";
        }
    }
}
