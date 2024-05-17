using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour, IInteractable
{
    public PathCreator pathCreator;
    public float forwardSpeed = 2.0f;
    public float backwardSpeed = 2.0f;
    private float distanceTravelled;
    private bool isReturning = false;
    [SerializeField] private bool isLighting;
    private bool hasLooped = false;
    private float pathLength;

    public void NotInteract()
    {
        isLighting = false;
    }

    public void OnInteract(Vector3 movePosition, bool isFocused)
    {
        isLighting = true;
    }

    void Start()
    {
        distanceTravelled = 0; // Ba�lang��ta mesafe s�f�r
        pathLength = pathCreator.path.length; // Path uzunlu�unu al
    }

    void Update()
    {
        if (isLighting)
        {
            isReturning = false;
            distanceTravelled += forwardSpeed * Time.deltaTime;

            if (distanceTravelled > pathLength)
            {
                hasLooped = true;
                distanceTravelled %= pathLength; // E�er path'i ge�tiysek, distanceTravelled'i pathLength ile mod al
            }
        }
        else
        {
            isReturning = true;
            distanceTravelled -= backwardSpeed * Time.deltaTime;

            if (distanceTravelled < 0)
            {
                distanceTravelled = 0;
                isReturning = false;
            }

            if (hasLooped && distanceTravelled <= 0)
            {
                hasLooped = false; // Ba�lang�� noktas�na geri d�nd�k, durma zaman�
                distanceTravelled = 0;
                isReturning = false;
            }
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
    }
}
