using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {
    public float speed;
    public int life = 10;
    public float penalty = 5;

    private Rigidbody2D rb;
    private Vector2 pointing = Vector2.down;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        AdditionalStart();
        StartCoroutine(LifeTimer());
    }

    protected virtual void AdditionalStart() {

    }

    public void InitProjectile(Vector2 targetPosition) {
        Vector2 deltaPositions = targetPosition - (Vector2)this.transform.position;
        pointing = deltaPositions.normalized;
        float angle = Vector2.SignedAngle(Vector2.down.normalized, deltaPositions.normalized);
        this.transform.Rotate(Vector3.forward, angle);
    }

    private void Update() {
        rb.velocity = pointing * speed;
    }

    private IEnumerator LifeTimer() {
        int timeElapsed = 0;
        while (timeElapsed < life) {
            yield return new WaitForSeconds(1);
            timeElapsed++;
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {
            Hit();
        }
    }

    public abstract void Hit();
}