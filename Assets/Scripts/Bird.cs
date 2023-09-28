using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMOUNT = 80f;

    private static Bird instance;

    public event EventHandler OnDied;
    public event EventHandler OnStarted;

    private Rigidbody2D birdRigidbody2D;

    private State state;

    private enum State {
        Waiting,
        Playing,
        Dead
    }

    private void Awake() {
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.Waiting;
    }

    private void Update() {  //metoda sa poziv kretanja
        switch (state) {
            default:
            case State.Waiting:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStarted != null) OnStarted(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Jump();
                }
                transform.Rotate(0, 0, birdRigidbody2D.velocity.y * .5f);
                break;
            case State.Dead:
                break;
        }
        transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y * .3f);
    }

    private void Jump() { //metoda za skok
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound("jump");
    }

    private void OnTriggerEnter2D(Collider2D collider) { //event za kontakt sa preprekom
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound("lose");
        if (OnDied != null) OnDied(this, EventArgs.Empty);
        state = State.Dead;

    }

    public static Bird GetInstance() {
        return instance;
    }
}
