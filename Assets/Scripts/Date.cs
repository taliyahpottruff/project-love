using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Date : MonoBehaviour {
    public TurnPhase turnPhase = TurnPhase.PlayerChoice;
    public GameObject choiceButtons;
    public Player player;
    public GameObject battleSquare;
    public PlayerAttackBar playerAttackBar;

    private void Start() {
        battleSquare.SetActive(false);
        playerAttackBar.gameObject.SetActive(false);
    }

    private void Update() {
        switch(turnPhase) {
            case TurnPhase.PlayerChoice:
                PlayerChoicePhase();
                break;
            case TurnPhase.PlayerAttack:
                PlayerAttackPhase();
                break;
            case TurnPhase.EnemyDialog:
                EnemyDialogPhase();
                break;
            case TurnPhase.EnemyAttack:
                EnemyAttackPhase();
                break;
        }
    }

    public void NextPhase() {
        switch (turnPhase) {
            case TurnPhase.PlayerChoice:
                playerAttackBar.gameObject.SetActive(true);
                playerAttackBar.secondsLeft = 10;
                playerAttackBar.StartAttack();
                turnPhase = TurnPhase.PlayerAttack;
                break;
            case TurnPhase.PlayerAttack:
                playerAttackBar.gameObject.SetActive(false);
                turnPhase = TurnPhase.EnemyDialog;
                break;
            case TurnPhase.EnemyDialog:
                battleSquare.SetActive(true);
                player.ResetPosition();
                turnPhase = TurnPhase.EnemyAttack;
                break;
            case TurnPhase.EnemyAttack:
                
                break;
        }
    }

    private void PlayerChoicePhase() {
        choiceButtons.SetActive(true);
        player.canMove = false;
    }

    private void PlayerAttackPhase() {
        choiceButtons.SetActive(false);
        player.canMove = false;

        if (playerAttackBar.secondsLeft <= 0) {
            bool success = (playerAttackBar.current - playerAttackBar.goal) >= 0;

            if (success) {
                Debug.Log("Action success!");
                AttackSuccess();
            } else {
                Debug.Log("Action failed!");
                AttackFailed();
            }
        }
    }

    private void EnemyDialogPhase() {
        choiceButtons.SetActive(false);
        player.canMove = false;
    }

    private void EnemyAttackPhase() {
        choiceButtons.SetActive(false);
        player.canMove = true;
    }

    public void ChoiceFlirt() {
        if (turnPhase == TurnPhase.PlayerChoice)
            NextPhase();
    }

    public void ChoiceJoke() {
        if (turnPhase == TurnPhase.PlayerChoice)
            NextPhase();
    }

    public void ChoiceStory() {
        if (turnPhase == TurnPhase.PlayerChoice)
            NextPhase();
    }

    public void AttackSuccess() {
        NextPhase();
    }

    public void AttackFailed() {
        NextPhase();
    }
}

public enum TurnPhase {
    PlayerChoice, PlayerAttack, EnemyDialog, EnemyAttack
}