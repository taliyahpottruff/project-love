using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Date : MonoBehaviour {
    public TurnPhase turnPhase = TurnPhase.PlayerChoice;
    public GameObject choiceButtons;

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

    private void PlayerChoicePhase() {
        choiceButtons.SetActive(true);
    }

    private void PlayerAttackPhase() {

    }

    private void EnemyDialogPhase() {

    }

    private void EnemyAttackPhase() {
        
    }

    public void ChoiceFlirt() {

    }

    public void ChoiceJoke() {

    }

    public void ChoiceStory() {

    }
}

public enum TurnPhase {
    PlayerChoice, PlayerAttack, EnemyDialog, EnemyAttack
}