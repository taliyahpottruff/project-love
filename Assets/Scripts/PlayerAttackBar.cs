using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBar : MonoBehaviour {
    public float current = 0f;
    public float goal = 50f;
    public float goal_flirt = 50f;
    public float goal_joke = 50f;
    public float goal_story = 50f;
    public float max = 100f;
    public float decreaseSpeed = 0.01f;
    public float increaseSpeed = 2f;
    public RectTransform bar;
    public RectTransform goalObject;

    public int secondsLeft = 10;

    private float width = 0f;

    private void Start() {
        width = this.GetComponent<RectTransform>().rect.width;
    }

    private void Update() {
        goalObject.anchoredPosition = new Vector2(Mathf.Clamp01(goal/max) * width, 0);

        if (secondsLeft > 0) {
            current -= decreaseSpeed;

            if (Input.GetKeyDown(KeyCode.Space)) {
                current += increaseSpeed;
            }
        }

        current = Mathf.Clamp(current, 0, max);
        float percent = Mathf.Clamp01(current / max);

        bar.sizeDelta = new Vector2((width*percent)-width, 1f);
    }

    public void StartAttack() {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        while (true) {
            yield return new WaitForSeconds(1);
            secondsLeft--;
            Debug.Log(secondsLeft);

            if (secondsLeft <= 0) break;
        }
    }
}