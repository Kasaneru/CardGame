using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit : MonoBehaviour {
    // [SerializeField] public GameObject model;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Animator animator;
    public Vector2 coords;

    public Card card;

    public bool isTower = false;
    public int owner;
    public int attack;
    public int health;
    public int maxHealth;
    public int moves;
    public int maxMovement;
    public TMP_Text attackText;
    public TMP_Text healthText;

    public bool isMoving;

    void Start() {
        setUnit(card);
    }

    public void setUnit(Card _card) {
        card = _card;
        attack = _card.attack;
        health = _card.maxHealth;
        maxHealth = _card.maxHealth;
        moves = _card.movement;
        maxMovement = _card.movement;
        attackText.text = _card.attack.ToString();
        healthText.text = _card.maxHealth.ToString();
    } 

    public void UpdateCanvas() {
        attackText.text = attack.ToString();
        healthText.text = health.ToString();
    } 

    public void SetHighlight(bool x) {
        _highlight.SetActive(x);
    }

    // Animation part
    public IEnumerator MoveToPosition(Vector3 target, float duration) {
        isMoving = true;
        animator.SetBool("IsMoving", true);
        Vector3 startPos = transform.position;

        Quaternion originalModelRotation = animator.transform.localRotation;
        Vector3 direction = (target - startPos).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        float elapsed = 0f;
        while (elapsed < duration) {
            animator.transform.localRotation = Quaternion.Slerp(
                    animator.transform.localRotation,
                    targetRotation, 
                    Time.deltaTime * 10f);
            transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
        animator.SetBool("IsMoving", false);
        animator.transform.localRotation = originalModelRotation;
    }

    public IEnumerator PlayAttackAnimation(Vector3 target, float duration) {
        Vector3 startPos = transform.position;
        Quaternion originalModelRotation = animator.transform.localRotation;
        Vector3 direction = (target - startPos).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        animator.SetTrigger("Attack");
        float elapsed = 0f;
        while (elapsed < duration) {
            animator.transform.localRotation = Quaternion.Slerp(
                    animator.transform.localRotation,
                    targetRotation, 
                    Time.deltaTime * 10f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        animator.transform.localRotation = originalModelRotation;
    }

    public void PlayDeathAnimation() {
        if (animator != null)
            animator.SetTrigger("Die");
    }
}
