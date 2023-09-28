using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    //konstante
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float DESTROY_PIPE_X_POS = -165f;
    private const float SPAWN_PIPE_X_POS = 165f;
    private const float BIRD_X_POSITION = 0f;

    private static Level instance;

    public static Level GetInstance() {
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipesSpawned;
    private int pipesPassedCount;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;
    
    //enumeratori
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
    }

    private enum State
    {
        Waiting, 
        Playing, 
        Dead,
    }

    //klasa za nivo
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform) {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }
        //metoda za pomeranje nivoa ka igracu
        public void Move() {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }
        //metoda za vracanje x pozicije 
        public float GetXPosition() {
            return pipeHeadTransform.position.x;
        }
        //metoda za ciscenje
        public void DestroySelf() {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }


    //awake metoda za pocetak
    private void Awake() {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.Waiting;
    }

    private void Start() {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStarted += Bird_OnStarted;
    }

    private void Bird_OnDied(object sender, System.EventArgs e) { //event za stavljanje dead state
        Debug.Log("Dead");
        state = State.Dead;
    }

    private void Bird_OnStarted(object sender, System.EventArgs e) {//za pokretanje
        state = State.Playing;
    }

    //update metoda sa pozivom za pomeranje i stvaranje nivoa
    private void Update() {
        if (state == State.Playing) {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    //poziv klasne metode i provera da li je za ciscenje
    private void HandlePipeMovement() {
        for (int i = 0; i < pipeList.Count; i++) {
            Pipe pipe = pipeList[i];

            bool isToTheRight = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRight && pipe.GetXPosition() <= BIRD_X_POSITION) {
                pipesPassedCount++;
                SoundManager.PlaySound("score");
            }

            if (pipe.GetXPosition() < DESTROY_PIPE_X_POS) {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    //objedinjena metoda za kreiranje cevi
    private void CreateGapPipes(float gapY, float gapSize, float xPosition) {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    //metoda za kreiranje cevi
    private void CreatePipe(float height, float xPosition, bool createBottom) {
        //instanciranje vrha cevi
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom) {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        } else {
            pipeHeadYPosition = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        //instanciranje tela cevi
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom) {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        } else {
            pipeBodyYPosition = CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        //renderer za prikaz
        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        //collider za cev
        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        //samo pravljenje i dodavanje u listu
        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }
    //poziv metoda za kreiranje cevi
    private void HandlePipeSpawning() {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0) {
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeigt = gapSize * .5f + heightEdgeLimit;
            float maxHeight = CAMERA_ORTHO_SIZE * 2f - gapSize * .5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeigt, maxHeight);
            CreateGapPipes(height, gapSize, SPAWN_PIPE_X_POS);
        }
    }
    //podesavanje nivoa tezine
    private Difficulty GetDifficulty() {
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }
    //biranje nivoa tezine
    private void SetDifficulty(Difficulty difficulty) {
        switch (difficulty) {
            case Difficulty.Easy:
                gapSize = 40f;
                break;
            case Difficulty.Medium:
                gapSize = 30f;
                break;
            case Difficulty.Hard:
                gapSize = 20f;
                break;
        }
    }
    
    public int GetPipesSpawned() {
        return pipesSpawned;
    }

    public int GetPipesPassedCount() {
        return pipesPassedCount;
    }

}
