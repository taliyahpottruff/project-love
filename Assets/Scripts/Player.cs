using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
    public bool canMove = true;
    public float speed = 5f;

    private Vector2 inputVector;
    private Vector2 directionVector;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        directionVector = inputVector.normalized;

        if (canMove) {
            rb.velocity = directionVector * speed;
        }
    }

    public void ResetPosition() {
        this.transform.localPosition = Vector2.zero;
    }
}