using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button), typeof(Image))]
public class TurnButton : MonoBehaviour {
    private Button button;
    private Image buttonImage;

    // Цвета для разных состояний
    [Header("Цвета кнопки")]
    [SerializeField] private Color player1Color = Color.green;
    [SerializeField] private Color player2Color = Color.red;
    [SerializeField] private Color inactiveColor = Color.gray;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        switch (GameManager.Instance.GameState)
        {
            case GameState.GenerateGrid:
                button.interactable = false;
                buttonImage.color = inactiveColor;
                break;
            case GameState.Player1Turn:
                button.interactable = true;
                buttonImage.color = player1Color;
                break;
            case GameState.Player2Turn:
                button.interactable = false;
                buttonImage.color = player2Color;
                break;
            case GameState.EndGame:
                button.interactable = false;
                buttonImage.color = inactiveColor;
                break;
        }
    }

    private void OnButtonClicked()
    {
        GameManager.Instance.EndTurn();
    }
}
