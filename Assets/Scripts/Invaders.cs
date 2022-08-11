﻿using UnityEngine;
using UnityEngine.SceneManagement;
public class Invaders : MonoBehaviour
{

    public Invader[] prefabs;
    public int rows = 5;
    public int columns = 11;
    public AnimationCurve speed = new AnimationCurve();
    public System.Action<Invader> killed;
    public int amountKilled { get; private set; }
    public int totalInvaders => this.rows * this.columns;
    public int TotalAmount => rows * columns;
    public float percentKilled => (float)AmountKilled / (float)TotalAmount;
    public Vector3 _direction = Vector2.right;
    public Vector3 initialPosition { get; private set; }
    public int AmountKilled { get; private set; }
    public int AmountAlive => TotalAmount - AmountKilled;
    public float missileAttackRate = 1.0f;
    public Projectile missilePrefab;

    private void Awake()
    {
        initialPosition = transform.position;

        for (int i = 0; i < rows; i++)
        {
            float width = 2f * (columns - 1);
            float height = 2f * (rows - 1);

            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                Invader invader = Instantiate(prefabs[i], transform);
                //invader.killed += OnInvaderKilled;
                invader.killed += InvaderKilled;

                Vector3 position = rowPosition;
                position.x += 2f * j;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        //InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }

    private void Update()
    {
        this.transform.position += _direction * this.speed.Evaluate(this.percentKilled) * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (_direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
                break;
            }
            else if (_direction == Vector3.left && invader.position.x <= (leftEdge.x - 1.0f))
            {
                AdvanceRow();
                break;
            }
        }
    }
    private void MissileAttack()
    {
        int amountAlive = AmountAlive;

        if (amountAlive == 0)
        {
            return;
        }

        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1f / (float)amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }

    }

    private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;
    }

    //private void OnInvaderKilled(Invader invader)
    //{
    //    invader.gameObject.SetActive(false);
    //    AmountKilled++;
    //    killed(invader);
    //}
    private void InvaderKilled()
    {
        //invader.gameObject.SetActive(false);
        AmountKilled++;
        if (this.AmountKilled >= this.totalInvaders)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        //killed(invader);
    }
}
