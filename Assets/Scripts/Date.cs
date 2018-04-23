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
    public GameObject cupidsArrowPrefab;
    public RectTransform dateMeter;
    public RectTransform dateBar;
    public Transform firingPointsParent;
    public AudioClip[] audioClips;
    public Animator feedbackTextAnim;
    public GameObject bossButtons;
    public Transform[] bossTargets;
    public string[] bossLines;

    private Transform[] firingPoints;
    private PlayerAction playerAction = PlayerAction.None;
    private PlayerActionStatus lastActionStatus = PlayerActionStatus.None;
    private string currentDialog = "";
    private bool dialogMoveOn = false;
    private SpriteRenderer sr;
    private DateObject date;
    private AttackType attackType = AttackType.FireAtPlayer;
    private AudioSource src;
    private bool boss = false;
    private int currentLine = 0;

    private void Start() {
        battleSquare.SetActive(false);
        mainDialog.gameObject.SetActive(true);

        sr = GetComponent<SpriteRenderer>();
        src = GetComponent<AudioSource>();
        
        firingPoints = new Transform[firingPointsParent.childCount];
        for (int i = 0; i < firingPointsParent.childCount; i++) {
            firingPoints[i] = firingPointsParent.GetChild(i);
        }

        date = Game.currentDate;

        if (date != null) { //We have a date to do
            if (Game.IsLastScene("Battle")) {
                feedbackTextAnim.Play("FeedbackText-Action");
                PlaySound(2);
            }

            if (date.difficulty >= 10) {
                GameObject.FindGameObjectWithTag("Music Player").GetComponent<MusicPlayer>().PlayBossIntro();
                boss = true;
                Debug.LogWarning("YOU CAN'T WIN");
            }

            sr.sprite = date.sprite;
            dateSuccess = 100 * date.difficulty;
            playerAttackBar.goal = Mathf.Clamp(50 + (25 * (date.difficulty / Game.gameLength)), 0, 75);
            playerAttackBar.goal_flirt = 10 * date.hatesFlirting;
            playerAttackBar.goal_joke = 10 * date.hatesJokes;
            playerAttackBar.goal_story = 10 * date.hatesStory;
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
            case TurnPhase.BossBattle:
                bossButtons.gameObject.SetActive(false);
                break;
            case TurnPhase.BossBattlePre:
                bossButtons.gameObject.SetActive(false);
                sr.enabled = true;
                
                mainDialog.text = "You really thought you could win that easily?";
                if (Input.GetKeyDown(KeyCode.Space) && dialogMoveOn) NextPhase();
                break;
            case TurnPhase.BossBattlePost:
                
                mainDialog.text = bossLines[currentLine];
                if (Input.GetKeyDown(KeyCode.Space)) {
                    currentLine++;
                    if (currentLine == 2) {
                        Destroy(GameObject.FindGameObjectWithTag("Music Player"));
                    }
                    if (currentLine >= bossLines.Length) {
                        
                        SceneManager.LoadScene("Menu");
                    }
                }
                break;
        }

        dateBar.sizeDelta = new Vector2(Mathf.Clamp01(dateProgress / dateSuccess) * dateMeter.rect.width, 1);

        if (dateProgress <= 0) {
            //Player loses
            Debug.Log("Player lost!");
            SceneManager.LoadScene("Menu");
        }

        if (dateProgress >= dateSuccess && !boss) {
            //Player succeeds
            if (Game.dateSequence.Count > 0) { //More dates
                Game.LoadNextDate();
                Game.SetLastScene("Battle");
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
                if (boss) {
                    mainDialog.gameObject.SetActive(true);
                    dialogMoveOn = false;
                    StartCoroutine(DelayedSpacePress());
                    GameObject.FindGameObjectWithTag("Music Player").GetComponent<MusicPlayer>().PlayBossSong();
                    turnPhase = TurnPhase.BossBattlePre;
                    break;
                }

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
                attackType = Utils.GetRandomAttackType();
                turnPhase = TurnPhase.EnemyAttack;
                StartCoroutine(EnemyFire());
                break;
            case TurnPhase.EnemyAttack:
                battleSquare.SetActive(false);
                mainDialog.gameObject.SetActive(true);
                turnPhase = TurnPhase.PlayerChoice;
                break;
            case TurnPhase.BossBattle:
                battleSquare.SetActive(false);
                mainDialog.gameObject.SetActive(true);
                turnPhase = TurnPhase.BossBattlePost;
                break;
            case TurnPhase.BossBattlePre:
                battleSquare.SetActive(true);
                player.ResetPosition();
                mainDialog.gameObject.SetActive(false);
                turnPhase = TurnPhase.BossBattle;
                StartCoroutine(BossBattle());
                break;
        }
    }

    private void PlayerChoicePhase() {
        if (!boss)
            choiceButtons.SetActive(true);
        else {
            choiceButtons.SetActive(false);
            bossButtons.SetActive(true);
            sr.enabled = false;
        }
        player.canMove = false;

        string prompt = "Choose an action!";
        if (boss) prompt = Utils.Scramble(prompt);

        mainDialog.text = prompt;
    }

    private void PlayerAttackPhase() {
        choiceButtons.SetActive(false);
        player.canMove = false;
        mainDialog.text = playerAttackBar.secondsLeft + "\nTap space repeatedly!";

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

            float progress = 50;
            dateProgress += progress;
            PlaySound(3);
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
            PlaySound(4);
            NextPhase();
        };
    }

    public void PlaySound(int id) {
        src.PlayOneShot(audioClips[id]);
    }

    private IEnumerator DelayedSpacePress() {
        yield return new WaitForSeconds(1f);
        dialogMoveOn = true;
    }

    private IEnumerator EnemyFire() {
        float cycles = 0;
        int stage = 0;
        while (true) {
            float cycleSpeed = 0.5f;
            GameObject chosenPrefab = dateKillerPrefab;
            if (Random.Range(0, 10) == 0) chosenPrefab = cupidsArrowPrefab;

            switch(attackType) {
                case AttackType.FireAtPlayer:
                    cycleSpeed = 0.5f;
                    FireAtPlayer(chosenPrefab);
                    break;
                case AttackType.FireAtPlayerRandom:
                    cycleSpeed = 0.5f;
                    FireAtPlayerRandom(chosenPrefab);
                    break;
                case AttackType.FireHorizontally:
                    cycleSpeed = 0.1f;
                    FireAtPlayerHorizontally(chosenPrefab);
                    break;
                case AttackType.AlternatingCone:
                    cycleSpeed = 0.5f;
                    if (stage == 0) stage = 1;
                    else stage = 0;
                    AlternatingCone(chosenPrefab, stage);
                    break;
            }
            yield return new WaitForSeconds(cycleSpeed);
            cycles++;

            float secondPassed = (float)cycles * cycleSpeed;
            Debug.Log(secondPassed);
            if (secondPassed >= 10) break;
        }
        NextPhase();
    }

    private IEnumerator BossBattle() {
        float cycles = 0;
        int stage = 0;
        float cycleSpeed = 0.01f;
        int firingPoint = 0;

        while (true) {

            GameObject chosenPrefab = dateKillerPrefab;

            //Battle logic
            GameObject go = Instantiate<GameObject>(chosenPrefab, firingPoints[firingPoint].position, Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();

            Vector2 target = GetBossTarget(firingPoint).position;

            if (stage == 1) {
                target = new Vector2(0.01f, firingPoints[firingPoint].position.y);

                if (firingPoint == 7 || firingPoint == 12) {
                    target = new Vector2(firingPoints[firingPoint].position.x * 2, firingPoints[firingPoint].position.y);
                }
            } else if (stage == 2) {
                target = new Vector2(firingPoints[firingPoint].position.x, 0.01f);

                if (firingPoint == 0 || firingPoint == 15) {
                    target = new Vector2(firingPoints[firingPoint].position.x, firingPoints[firingPoint].position.y * 2);
                }
            }

            projectile.InitProjectile(target);
            firingPoint++;
            if (firingPoint >= firingPoints.Length) firingPoint = 0;

            yield return new WaitForSeconds(cycleSpeed);
            cycles++;

            float secondPassed = (float)cycles * cycleSpeed;
            float dateScore = secondPassed * (1000/20);
            
            dateProgress = dateScore;
            float datePercent = dateProgress / dateSuccess;
            Debug.Log(datePercent + ", " + stage);

            if (datePercent >= 0.95f) stage = 0;
            else if (datePercent >= 0.90f) stage = 2;
            else if (datePercent >= 0.85f) stage = 1;
            else if (datePercent >= 0.80f) stage = 2;
            else if (datePercent >= 0.75f) stage = 1;
            else if (datePercent >= 0.50f) stage = 2;
            else if (datePercent >= 0.25f) stage = 1;
            else stage = 0;

            if (dateProgress >= 1000) {
                NextPhase();
                break;
            }
        }
        NextPhase();
    }

    private Transform GetBossTarget(int firingPoint) {
        switch (firingPoint) {
            case 0:
                return bossTargets[1];
            case 1:
                return bossTargets[1];
            case 2:
                return bossTargets[0];
            case 3:
                return bossTargets[1];
            case 4:
                return bossTargets[2];
            case 5:
                return bossTargets[1];
            case 6:
                return bossTargets[1];
            case 7:
                return bossTargets[0];
            case 8:
                return bossTargets[2];
            case 9:
                return bossTargets[0];
            case 10:
                return bossTargets[3];
            case 11:
                return bossTargets[3];
            case 12:
                return bossTargets[3];
            case 13:
                return bossTargets[0];
            case 14:
                return bossTargets[0];
            case 15:
                return bossTargets[3];
            case 16:
                return bossTargets[1];
            case 17:
                return bossTargets[3];
            case 18:
                return bossTargets[0];
            case 19:
                return bossTargets[0];
            case 20:
                return bossTargets[1];
            case 21:
                return bossTargets[3];
            case 22:
                return bossTargets[2];
            case 23:
                return bossTargets[2];
            default:
                return bossTargets[0];
        }
    }

    #region Attacks
    private void FireAtPlayer(GameObject prefab) {
        GameObject go = Instantiate<GameObject>(prefab, firingPoints[0].position, Quaternion.identity);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.InitProjectile(player.transform.position);
    }

    private void FireAtPlayerRandom(GameObject prefab) {
        int point = Random.Range(1, 5);
        GameObject go = Instantiate<GameObject>(prefab, firingPoints[point].position, Quaternion.identity);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.InitProjectile(player.transform.position);
    }

    private void FireAtPlayerHorizontally(GameObject prefab) {
        int point = Random.Range(5, 15);
        GameObject go = Instantiate<GameObject>(prefab, firingPoints[point].position, Quaternion.identity);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.InitProjectile(new Vector2(0, firingPoints[point].position.y));
    }

    private void AlternatingCone(GameObject prefab, int stage) {
        for (int i = 0; i < 6 + stage; i++) {
            GameObject go = Instantiate<GameObject>(prefab, firingPoints[0].position, Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.InitProjectile(Utils.Rotate(Vector2.down, (36 * i) - 90 - (18 * stage)));
        }
    }
    #endregion
}

public enum TurnPhase {
    PlayerChoice, PlayerAttack, EnemyDialog, EnemyAttack, BossBattle, BossBattlePre, BossBattlePost
}

public enum PlayerAction {
    None, Flirt, Joke, Story
}

public enum PlayerActionStatus {
    None, FlirtSuccess, FlirtFailure, JokeSuccess, JokeFailure, StorySuccess, StoryFailure
}

public enum AttackType {
    FireAtPlayer, FireAtPlayerRandom, FireHorizontally, AlternatingCone
}
