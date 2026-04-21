using UnityEngine;
using System;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveDelay = 0f;
    public float gridSize = 1f;
    public bool isMoving = false;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float moveTime;
    public GameObject playerObject;
    public Action<Vector3> UpdatePos;
    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void StartMovement(Vector3 direction)
    {
        float maxSoundDistance = 5f * gridSize;

        if (playerObject == gameObject)
        {
            // pøehrát krok hráèe vždy
            SoundManager.Instance.PlayStep();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
            if (distanceToPlayer <= maxSoundDistance)
            {
                // pøehrát krok NPC jen pokud je blízko hráèe
                SoundManager.Instance.PlayStep();
            }
        }

        isMoving = true;
        startPosition = transform.position;
        endPosition = startPosition + direction * gridSize;
        moveTime = 0;
        UpdatePos?.Invoke(endPosition);
    }


    public void MovePlayer()
    {
        moveTime += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
        if (moveTime >= 1f + moveDelay)
        {
            transform.position = endPosition;
            isMoving = false;

        }
    }
    public void ForceStopAt(Vector3 position)
    {
        isMoving = false;
        transform.position = position;
    }
}
