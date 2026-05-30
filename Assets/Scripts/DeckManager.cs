using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckManager : MonoBehaviour {
    public static DeckManager Instance;
    public TMP_Text deck1Count, deck2Count;

    [Header("Колоды игроков")]
    [SerializeField] private List<Card> player1Deck;
    [SerializeField] private List<Card> player2Deck;

    [Header("UI руки")]
    [SerializeField] private Transform player1HandArea;
    // [SerializeField] private Transform player2HandArea;

    [SerializeField] private GameObject cardPrefab; // префаб карты (с CardDisplay и Draggable)

    private const int MAX_HAND_SIZE = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Shuffle(player1Deck);
        Shuffle(player2Deck);
    }

    public Card DrawCard(int playerIndex)
    {
        List<Card> deck = (playerIndex == 1) ? player1Deck : player2Deck;
        // Transform handArea = (playerIndex == 1) ? player1HandArea : player2HandArea;
        Transform handArea = player1HandArea;

        BotPlayer bp = BotPlayer.Instance;

        if (handArea.childCount >= MAX_HAND_SIZE) {
            Debug.Log($"Рука игрока {playerIndex} переполнена! Карта сгорает.");
            return null;
        }

        if (deck.Count == 0) {
            Debug.Log($"Колода игрока {playerIndex} пуста! Карта не взята.");
            return null;
        }

        Card drawnCard = deck[0];
        Debug.Log($"Взята карта {drawnCard.cardName} игроком {playerIndex}.");

        deck.RemoveAt(0);

        // UI for player 1
        if (playerIndex == 1) {
            GameObject cardObj = Instantiate(cardPrefab, handArea);
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
                display.SetCard(drawnCard);
        } else if (playerIndex == 2) {
            bp.AddCardToHand(drawnCard);
        }

        return drawnCard;
    }

    // Bubble sort :)
    private void Shuffle(List<Card> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    private void Update() {
        deck1Count.text = player1Deck.Count.ToString();
        deck2Count.text = player2Deck.Count.ToString();
    }
}
