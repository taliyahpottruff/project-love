using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Date : MonoBehaviour {
    public string[] flirtSuccessResponses;
    public string[] flirtFailureResponses;
    public string[] jokeSuccessResponses;
    public string[] jokeFailureResponses;
    public string[] storySuccessResponses;
    public string[] storyFailureResponses;

    public TurnPhase turnPhase = TurnPhase.PlayerChoice;
    public float dateProgress = 10;
    public float dateSuccess = 100;
    public GameObject choiceButtons;
    public Player player;
    public GameObject battleSquare;
    public PlayerAttackBar playerAttackBar;
    public Text mainDialog;
    public GameObject dateKillerPrefab;
    public RectTransform dateMeter;
    public RectTransform dateBar;

    private PlayerAction playerAction = PlayerAction.None;
    private PlayerActionStatus lastActionStatus = PlayerActionStatus.None;
    private string currentDialog = "";
    private bool dialogMoveOn = false;
    private SpriteRenderer sr;
    private DateObject date;

    private void Start() {
        battleSquare.SetActive(false);

        sr = GetComponent<SpriteRenderer>();

        date = Game.currentDate;

        if (date != null) {
            sr.sprite = date.sprite;
            dateSuccess = 100 * date.difficulty;
            playerAttackBar.goal = Mathf.Clamp(50 + (25 * (date.difficulty / Game.gameLength)), 0, 100);
        }
        else {
            SceneManager.LoadScene("Menu");
        }

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

        dateBar.sizeDelta = new Vector2(Mathf.Clamp01(dateProgress / dateSuccess) * dateMeter.rect.width, 1);

        if (dateProgress <= 0) {
            //Player loses
            Debug.Log("Player lost!");
            SceneManager.LoadScene("Menu");
        }

        if (dateProgress >= dateSuccess) {
            //Player succeeds
            if (Game.dateSequence.Count > 0) { //More dates
                Game.LoadNextDate();
                SceneManager.LoadScene("Battle");
            } else { //No more dates
                //Player wins
                Debug.Log("Player wins");
                SceneManager.LoadScene("Menu");
            }
        }
    }

    public void NextPhase() {
        switch (turnPhase) {
            case TurnPhase.PlayerChoice:
                playerAttackBar.gameObject.SetActive(true);
                playerAttackBar.secondsLeft = 10;
                playerAttackBar.current = 0;
                playerAttackBar.StartAttack();
                mainDialog.gameObject.SetActive(true);
                turnPhase = TurnPhase.PlayerAttack;
                break;
            case TurnPhase.PlayerAttack:
                playerAttackBar.gameObject.SetActive(false);
                mainDialog.gameObject.SetActive(true);
                dialogMoveOn = false;
                StartCoroutine(DelayedSpacePress());
                turnPhase = TurnPhase.EnemyDialog;
                break;
            case TurnPhase.EnemyDialog:
                battleSquare.SetActive(true);
                player.ResetPosition();
                mainDialog.gameObject.SetActive(false);
                turnPhase = TurnPhase.EnemyAttack;
                StartCoroutine(EnemyFire());
                break;
            case TurnPhase.EnemyAttack:
                battleSquare.SetActive(false);
                turnPhase = TurnPhase.PlayerChoice;
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
        mainDialog.text = playerAttackBar.secondsLeft + "";

        if (playerAttackBar.secondsLeft <= 0) {
            bool success = (playerAttackBar.current - playerAttackBar.goal) >= 0;

            if (success) {
                Debug.Log("Action success!");
                AttackSuccess(playerAction);
            } else {
                Debug.Log("Action failed!");
                AttackFailed(playerAction, playerAttackBar.current, playerAttackBar.goal);
            }
        }
    }

    private void EnemyDialogPhase() {
        choiceButtons.SetActive(false);
        player.canMove = false;
        mainDialog.text = currentDialog;

        if (Input.GetKeyDown(KeyCode.Space) && dialogMoveOn) NextPhase();
    }

    private void EnemyAttackPhase() {
        choiceButtons.SetActive(false);
        player.canMove = true;
    }

    public void ChoiceFlirt() {
        if (turnPhase == TurnPhase.PlayerChoice) {
            playerAction = PlayerAction.Flirt;
            NextPhase();
        }
    }

    public void ChoiceJoke() {
        if (turnPhase == TurnPhase.PlayerChoice) {
            playerAction = PlayerAction.Joke;
            NextPhase();
        }
    }

    public void ChoiceStory() {
        if (turnPhase == TurnPhase.PlayerChoice) {
            playerAction = PlayerAction.Story;
            NextPhase();
        }
    }

    public void AttackSuccess(PlayerAction action) {
        if (turnPhase == TurnPhase.PlayerAttack) {
            switch (action) {
                case PlayerAction.Flirt:
                    lastActionStatus = PlayerActionStatus.FlirtSuccess;
                    currentDialog = flirtSuccessResponses[Random.Range(0, flirtSuccessResponses.Length)];
                    break;
                case PlayerAction.Joke:
                    lastActionStatus = PlayerActionStatus.JokeSuccess;
                    currentDialog = jokeSuccessResponses[Random.Range(0, jokeSuccessResponses.Length)];
                    break;
                case PlayerAction.Story:
                    lastActionStatus = PlayerActionStatus.StorySuccess;
                    currentDialog = storySuccessResponses[Random.Range(0, storySuccessResponses.Length)];
                    break;
            }

            float progress = 25;
            dateProgress += progress;
            NextPhase();
        }
    }

    public void AttackFailed(PlayerAction action, float achieved, float goal) {
        if (turnPhase == TurnPhase.PlayerAttack) {
            switch (action) {
                case PlayerAction.Flirt:
                    lastActionStatus = PlayerActionStatus.FlirtFailure;
                    currentDialog = flirtFailureResponses[Random.Range(0, flirtFailureResponses.Length)];
                    break;
                case PlayerAction.Joke:
                    lastActionStatus = PlayerActionStatus.JokeFailure;
                    currentDialog = jokeFailureResponses[Random.Range(0, jokeFailureResponses.Length)];
                    break;
                case PlayerAction.Story:
                    lastActionStatus = PlayerActionStatus.StoryFailure;
                    currentDialog = storyFailureResponses[Random.Range(0, storyFailureResponses.Length)];
                    break;
            }

            float penaltyPercent = (goal - achieved) / goal;
            float penalty = (dateSuccess * 0.05f) * penaltyPercent;
            dateProgress -= penalty;
            NextPhase();
        };
    }

    private IEnumerator DelayedSpacePress() {
        yield return new WaitForSeconds(1f);
        dialogMoveOn = true;
    }

    private IEnumerator EnemyFire() {
        float cycles = 0;
        while (true) {
            GameObject go = Instantiate<GameObject>(dateKillerPrefab, this.transform.position, Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.InitProjectile(player.transform.position);
            yield return new WaitForSeconds(0.5f);
            cycles++;

            float secondPassed = (float)cycles * 0.5f;
            Debug.Log(secondPassed);
            if (secondPassed >= 10) break;
        }
        NextPhase();
    }
}

public enum TurnPhase {
    PlayerChoice, PlayerAttack, EnemyDialog, EnemyAttack
}

public enum PlayerAction {
    None, Flirt, Joke, Story
}

public enum PlayerActionStatus {
    None, FlirtSuccess, FlirtFailure, JokeSuccess, JokeFailure, StorySuccess, StoryFailure
}
