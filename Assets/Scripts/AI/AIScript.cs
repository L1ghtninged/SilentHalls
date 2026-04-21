
using Assembly_CSharp;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AIScript : MonoBehaviour
{
    public EnemyAnimation animationScript;
    public EnemyLegacyAnimation legacyAnimation;
    public float moveDelay = 1f;
    public float attackCooldown = 1f;
    public int maxDistanceToPlayer = 10;
    public float attackDistanceToPlayer = 1;
    public float cellSize = 5f;
    public State state = State.Idle;
    public float rotationDuration = 1f;

    private float currentTimer = 0f;
    private float attackTimer = 0f;

    PositionScript position;
    GameObject playerObject;
    PositionScript playerPosition;
    AttackScript attackScript;
    StatScript playerStats;
    EnemyStatScript enemyStats;
    private bool isRotating = false;
    private float currentRotationY = 0f;
    
    private void Start()
    {
        position = GetComponent<PositionScript>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerPosition = playerObject.GetComponent<PositionScript>();
        attackScript = playerObject.GetComponent<AttackScript>();
        playerStats = playerObject.GetComponent<StatScript>();
        enemyStats = GetComponent<EnemyStatScript>();
    }
    private void Update()
    {
        attackTimer += Time.deltaTime;
        currentTimer += Time.deltaTime;

        // Pøed jakoukoliv akcí zkontrolujte platnost pozice
        if (position.x < 0 || position.y < 0 ||
            position.x >= EntityPositions.width ||
            position.y >= EntityPositions.height)
        {
            Debug.LogError("AI out of bounds! Resetting position.");
            position.x = Mathf.Clamp(position.x, 0, EntityPositions.width - 1);
            position.y = Mathf.Clamp(position.y, 0, EntityPositions.height - 1);
            return;
        }

        if (position.gridMovement.isMoving || isRotating)
        {
            if (position.gridMovement.isMoving)
                position.gridMovement.MovePlayer();
            return;
        }
        else
        {
            if(animationScript != null)animationScript.SetWalking(false);
            if(legacyAnimation != null)legacyAnimation.SetWalking(false);
        }

        int currentDistance = CalculateDistance();
        bool shouldAttack = currentDistance <= attackDistanceToPlayer;

        // Prùbìžná kontrola vzdálenosti pro všechny stavy
        if (shouldAttack && state != State.Attack)
        {
            state = State.Attack;
            attackTimer = 0f;
            if (animationScript != null) animationScript.SetWalking(false);
            if (legacyAnimation != null) legacyAnimation.SetWalking(false);
        }
        else if (!shouldAttack)
        {
            state = currentDistance <= maxDistanceToPlayer ? State.Chase : State.Idle;
        }

        if (state == State.Attack)
        {
            HandleAttackState();
        }
        else if (currentTimer >= moveDelay)
        {
            currentTimer = 0f;
            HandleMovement();
            if (animationScript != null)animationScript.SetWalking(true);
            if (legacyAnimation != null)legacyAnimation.SetWalking(true);
        }
    }

    private int CalculateDistance()
    {
        return Math.Abs(position.x - playerPosition.x) + Math.Abs(position.y - playerPosition.y);
    }

    private void HandleAttackState()
    {
        if (state != State.Attack)
        {
            attackTimer = 0f;
            state = State.Attack;
            return;
        }

        if (isRotating) return;

        if (attackTimer < attackCooldown)
            return;

        int correctDirection = GetDirectionTowardsPlayer();

        if (correctDirection != position.orientationIndex)
        {
            attackTimer = 0f;
            RotateProperly(correctDirection, position.orientationIndex);
        }
        else
        {
            if (animationScript != null) animationScript.Attack();
            if (legacyAnimation != null) legacyAnimation.Attack();
            attackScript.EnemyAttack(enemyStats, playerStats);
            attackTimer = 0f;
        }
    }

    private int GetDirectionTowardsPlayer()
    {
        int dx = playerPosition.x - position.x;
        int dy = playerPosition.y - position.y;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            return dx > 0 ? 1 : 3;
        }
        else
        {
            return dy > 0 ? 0 : 2;
        }
    }

    // Opravená rotace
    private void RotateProperly(int correctDirection, int currentOrientation)
    {
        int diff = (correctDirection - currentOrientation + 4) % 4;

        switch (diff)
        {
            case 1:
                position.TurnRight();
                RotateCamera(90f);
                break;
            case 3:
                position.TurnLeft();
                RotateCamera(-90f);
                break;
            case 2:
                position.TurnRight();
                RotateCamera(90f);
                break;
        }
    }
    private int GetCorrectDirection()
    {
        int bestDirection = -1;
        float bestDistance = float.MaxValue;

        for (int direction = 0; direction < 4; direction++)
        {
            if (IsDirectionValid(direction))
            {
                Vector2Int newPos = GetNewPosition(direction);
                float distance = Vector2Int.Distance(newPos, playerPosition.GetGridPosition());

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = direction;
                }
            }
        }
        return bestDirection;
    }

    private void HandleMovement()
    {
        switch (state)
        {
            case State.Chase:
                int bestDir = GetCorrectDirection();

                if (bestDir == -1) // Žádný platný smìr
                {
                    state = State.Idle;
                    break;
                }

                if (bestDir == position.orientationIndex)
                {
                    position.Move(0);
                }
                else
                {
                    RotateProperly(bestDir, position.orientationIndex);
                }
                break;
            case State.Idle:
                HandleIdleState();
                break;
        }
    }

    private void HandleIdleState()
    {
        if (UnityEngine.Random.value < 0.5f && IsDirectionValid(position.orientationIndex))
        {
            position.Move(0);
        }
        else
        {
            int randomDir = UnityEngine.Random.Range(0, 4);
            RotateProperly(randomDir, position.orientationIndex);
        }
    }

    private bool IsDirectionValid(int direction)
    {
        Vector2Int newPos = GetNewPosition(direction);

        return EntityPositions.IsWalkable(newPos.x, newPos.y)
       && !EntityPositions.IsOccupied(newPos.x, newPos.y);

    }
    private Vector2Int GetNewPosition(int direction)
    {
        return position.GetNewPosition(direction);
    }

    private IEnumerator SmoothRotate(Transform target, Quaternion targetRotation, float duration)
    {
        isRotating = true;
        Quaternion startRotation = target.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.rotation = targetRotation;
        isRotating = false;
    }
    void RotateCamera(float angle)
    {
        currentRotationY += angle;
        StartCoroutine(SmoothRotate(transform, Quaternion.Euler(0, currentRotationY, 0), rotationDuration));
    }

}
public enum State
{
    Idle,
    Chase,
    Attack,
}