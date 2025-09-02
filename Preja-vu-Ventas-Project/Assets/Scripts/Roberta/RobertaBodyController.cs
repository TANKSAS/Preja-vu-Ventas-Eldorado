using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobertaBodyController : MonoBehaviour
{
    [SerializeField] private bool isRandomMove;
    [SerializeField] private bool isTalking;
    [SerializeField] private bool isResetingBodyHolderPosition;
    [SerializeField] private bool isResetingBodyPosition;

    [SerializeField] public bool isChangingLook;
    [SerializeField] public bool isMoving;

    [SerializeField] private float stopSpeed = 1f;
    [SerializeField] private float thresholdDistance = 0.5f;

    [SerializeField] private float oscillationNormalSpeed;
    [SerializeField] private float oscilationRunSpeed;

    [SerializeField] private float speakingSlowSpeed;
    [SerializeField] private float speakingRunSpeed;

    [SerializeField] private float transitionDurationSlowSpeed;
    [SerializeField] private float transitionDurationRunSpeed;

    [SerializeField] private float moveBodySlowSpeed;
    [SerializeField] private float moveBodyNormalSpeed;
    [SerializeField] private float moveBodyRunSpeed;


    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float offsetY = 10f;
    [SerializeField] private float offsetZ = 0.5f;

    [SerializeField] private float size = 5f;   // Tamaño de la figura de infinito
    [SerializeField] private float smoothTime = 0.1f;

    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject moveTarget; //Punto al cual acercarse 
    [SerializeField] private GameObject[] oscilationWaitPoints;

    public GameObject body; // modelado del cuerpo
    public GameObject bodyHolder; // Objeto que guarda el cuerpo
    public GameObject oscilationWaitPointsHolder; // Objeto que guarda el cuerpo
    public Direction moveDirection = Direction.Right;

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    
    public Vector3 initialBodyPosition;
    public Vector3 initialBodyHolderPosition;
    
    [SerializeField] private Vector3 targetNextPosition;

    public float rotation;
    public float horizon;
    public float oscillationSpeed;

    [SerializeField] float transitionDuration = 1f; // Duración de la transición en segundos
    [SerializeField] float moveSpeed;
    [SerializeField] float moveTalkingSpeed;

    public GameObject centralObject; // Objeto central alrededor del cual orbita


    Color32 color;
    float time;
    int currentPositionIndex;
    int destPoint;
    Coroutine transitionCoroutine;
    private Vector3 nextPosition;
    public  float distanceXYZ;

    public enum Direction
    {
        Left,
        Right
    }

    public void SetCurrentPosition()
    {
        currentPosition = body.transform.localPosition; // Guardar la posición inicial
    }

    public void Testing()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.upArrowKey.isPressed)
        {
            Vector3 test = new Vector3(0, 0, moveSpeed);
            bodyHolder.transform.position += test * Time.deltaTime;
        }

        if (keyboard.downArrowKey.isPressed)
        {
            Vector3 test = new Vector3(0, 0, -moveSpeed);
            bodyHolder.transform.position += test * Time.deltaTime;
        }

        if (keyboard.leftArrowKey.isPressed)
        {
            Vector3 test = new Vector3(-moveSpeed, 0, 0);
            bodyHolder.transform.position += test * Time.deltaTime;
        }

        if (keyboard.rightArrowKey.isPressed)
        {
            Vector3 test = new Vector3(moveSpeed, 0, 0);
            bodyHolder.transform.position += test * Time.deltaTime;
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            StartResetBodyPosition();
        }
    }

    public void Fly()
    {
        currentPosition = new Vector3(bodyHolder.transform.position.x, RobertaController.Instance.moveLocomotionController.movementRoberta.gameObject.transform.position.z, RobertaController.Instance.moveLocomotionController.movementRoberta.gameObject.transform.position.y);
        Vector3 moveDirection = currentPosition - previousPosition;

        //// Normalizar la dirección de movimiento
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }

        //RobertaController.Instance.animationController.SetAnimation(moveDirection.z, moveDirection.x);

        // Suavizar el valor del parámetro para la animación
        float targetHorizontal = moveDirection.x;
        float targetDepth = moveDirection.z;

        // Interpolación suave
        float smoothHorizontal = Mathf.Lerp(RobertaController.Instance.animationController.animator.GetFloat("Rotation"), targetHorizontal, smoothTime);
        float smoothDepth = Mathf.Lerp(RobertaController.Instance.animationController.animator.GetFloat("Velocity"), targetDepth, smoothTime);

        if (!isResetingBodyHolderPosition)
        {
            RobertaController.Instance.animationController.SetAnimation(smoothDepth, smoothHorizontal);
        }
        else
        {
            RobertaController.Instance.animationController.SetAnimation(0, 0);
        }

        previousPosition = currentPosition;
    }

    public void RobertaBodyMovement()
    {
        LookAtCentralObject();
        Fly();

        if (isResetingBodyHolderPosition)
        {
            RobertaController.Instance.sfxController.AdjustThrustersVolume(0.1f, 0.7f, new Vector3(bodyHolder.transform.position.x, RobertaController.Instance.transform.position.y, bodyHolder.transform.position.z));
            return;
        }
        else if (isMoving)
        {
            MoveTowardsTarget();
        }
        else if (isRandomMove)
        {
            RandomMovement();
        }
        else if (isTalking)
        {
            TalkingMovement();
        }

        RobertaController.Instance.sfxController.AdjustThrustersVolume(0.22f, 0.7f, new Vector3(bodyHolder.transform.position.x, RobertaController.Instance.transform.position.y, bodyHolder.transform.position.z));
    }

    public void StartTalking()
    {
        time = 0;
        Debug.Log("Start Move Talking");
        SetCurrentPosition();
        isTalking = true;
    }

    void TalkingMovement()
    {
        time += Time.deltaTime * moveTalkingSpeed;

        // Calcular las posiciones x e y usando la parametrización de la lemniscata
        float x = size * Mathf.Cos(time);
        float y = size * Mathf.Sin(time) * Mathf.Cos(time);

        // Ajustar la dirección del movimiento
        if (moveDirection == Direction.Left)
        {
            x = -x;
        }

        float z = body.transform.localPosition.z;  // Mantener la posición inicial en el eje Z

        // Interpolar suavemente desde la posición inicial hacia la posición de la lemniscata
        Vector3 targetPosition = initialBodyHolderPosition + new Vector3(x, y, z);

        body.transform.localPosition = Vector3.Lerp(body.transform.localPosition, targetPosition, Time.deltaTime * moveTalkingSpeed);
    }

    public void ChangeDirection()
    {
        if (moveDirection == Direction.Left)
        {
            moveDirection = Direction.Right;
        }
        else
        {
            moveDirection = Direction.Left;
        }
    }

    public void EndTalking()
    {
        isTalking = false;
        StartResetBodyPosition();
    }

    public void StartRandomPosition()
    {
        RandomPosition();
        isRandomMove = true;
    }

    void RandomPosition()
    {
        float randomX = UnityEngine.Random.Range(currentPosition.x - offsetX, currentPosition.x + offsetX);
        float randomY = UnityEngine.Random.Range(currentPosition.y - offsetY, currentPosition.y + offsetY);
        float randomZ = UnityEngine.Random.Range(currentPosition.z - offsetZ, currentPosition.z + offsetZ);
        targetNextPosition = new Vector3(randomX, randomY, randomZ);
        Debug.Log("NextRandomPoint" + targetNextPosition);
    }

    void RandomMovement()
    {
        bodyHolder.transform.localPosition = Vector3.MoveTowards(bodyHolder.transform.localPosition, targetNextPosition, moveSpeed * Time.deltaTime);
        // Check if the character has reached the target position
        if (Vector3.Distance(bodyHolder.transform.localPosition, targetNextPosition) <= 0.1f)
        {
            Debug.Log("LLegamos Random");
            // Set a new random target position
            RandomPosition();
        }
    }

    public void EndRandomMovement()
    {
        Debug.Log("EndRandomMovement");
        isRandomMove = false;
        StartResetBodyHolderPosition();
    }

    public void MoveBodyTowardsNextPoint(GameObject nextPoint)
    {
        moveTarget = nextPoint;
        Debug.Log("StartMoving");
        SetCurrentPosition();
        isMoving = true;
    }

    public float GetDistanceNextPoint()
    {
        Debug.Log(distanceXYZ);
        return distanceXYZ = Vector3.Distance(bodyHolder.transform.position, nextPosition);
    }

    void MoveTowardsTarget()
    {
        if (moveTarget == null)
            return;

        //// Mantener la posición Y actual del cuerpo y establecer la posición X, Y y Z del objetivo
        //nextPosition = moveTarget.transform.position/*new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y, moveTarget.transform.position.z)*/;

        //// Mover el objeto hacia la posición objetivo
        //bodyHolder.transform.position = Vector3.MoveTowards(bodyHolder.transform.position, nextPosition, moveSpeed * Time.deltaTime);


        //// Verificar si el objeto ha alcanzado la posición objetivo en X, Y y Z
        //if (GetDistanceNextPoint() < thresholdDistance)
        //{
        //    isMoving = false;
        //    Debug.Log("Object has reached the target point.");
        //}

        //// Mantener la posición Y actual del cuerpo y establecer la posición X y Z del objetivo
        Vector3 nextPosition = new Vector3(moveTarget.transform.position.x, bodyHolder.transform.position.y, moveTarget.transform.position.z);
        // Mover el objeto hacia la posición objetivo
        bodyHolder.transform.position = Vector3.MoveTowards(bodyHolder.transform.position, nextPosition, moveSpeed * Time.deltaTime);
        //oscilationWaitPointsHolder.transform.position = Vector3.MoveTowards(bodyHolder.transform.position, nextPosition, moveSpeed * Time.deltaTime);


        // Calcular la distancia en los ejes X y Z
        Vector2 currentPosXZ = new Vector3(bodyHolder.transform.position.x, bodyHolder.transform.position.y, bodyHolder.transform.localPosition.z);
        Vector2 targetPosXZ = new Vector3(nextPosition.x, nextPosition.y, nextPosition.z);
        float distanceXZ = Vector3.Distance(currentPosXZ, targetPosXZ);
        //Debug.Log(distanceXZ);
        // Verificar si el objeto ha alcanzado la posición objetivo en X y Z
        if (distanceXZ < thresholdDistance)
        {
            moveTarget = null;
            isMoving = false;
            Debug.Log("Object has reached the target point.");
        }
    }

    public void StartResetBodyHolderPosition()
    {
        Debug.Log("Start Reset BodyHolder Position");
        isResetingBodyHolderPosition = true;
        StartCoroutine(SmoothBodyHolderResetPosition());
    }

    public void StartResetBodyPosition()
    {
        Debug.Log("Start Reset Body Position");
        isResetingBodyPosition = true;
        StartCoroutine(SmoothBodyResetPosition());
    }

    IEnumerator SmoothBodyHolderResetPosition()
    {
        while (isResetingBodyHolderPosition)
        {
            // Calcula la distancia total entre la posición actual y la posición inicial
            float distance = Vector3.Distance(bodyHolder.transform.localPosition, initialBodyHolderPosition);

            // Si la distancia es menor o igual a 1f, entra en la verificación
            if (distance <= 0.9f)
            {
                //Debug.Log("Distance: " + distance);
                // Mueve el cuerpo hacia la posición inicial en los tres ejes a una velocidad constante
                bodyHolder.transform.localPosition = Vector3.MoveTowards(
                    bodyHolder.transform.localPosition,
                    initialBodyHolderPosition,
                    moveSpeed * Time.deltaTime
                );
                if (isMoving)
                {
                    isResetingBodyHolderPosition = false;
                    Debug.Log("Change Reset Body Position");
                    yield break;
                }

                // Si la distancia es muy pequeña, corrige la posición y termina
                if (distance <= 0.01f)
                {
                    Debug.Log("Distance: " + distance);
                    bodyHolder.transform.localPosition = initialBodyHolderPosition;
                    Debug.Log("End Reset Body Position");
                    isResetingBodyHolderPosition = false;
                }
            }
            else
            {
                bodyHolder.transform.localPosition = Vector3.Lerp(bodyHolder.transform.localPosition, initialBodyHolderPosition, moveSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    IEnumerator SmoothBodyResetPosition()
    {    
        while (isResetingBodyPosition)
        {
            // Calcula la distancia total entre la posición actual y la posición inicial
            float distance = Vector3.Distance(body.transform.localPosition, initialBodyPosition);

            // Mueve el cuerpo hacia la posición inicial en los tres ejes a una velocidad constante
            body.transform.localPosition = Vector3.MoveTowards(
                body.transform.localPosition,
                initialBodyPosition,
                moveSpeed * Time.deltaTime
            );

            // Si la distancia es menor o igual a 1f, entra en la verificación
            if (distance <= 1f)
            {
                if (isTalking)
                {
                    isResetingBodyPosition = false;
                    Debug.Log("Change Reset Body Position");
                    yield break;
                }

                // Si la distancia es muy pequeña, corrige la posición y termina
                if (distance <= 0.01f)
                {
                    body.transform.localPosition = initialBodyPosition;
                    Debug.Log("End Reset Body Position");
                    isResetingBodyPosition = false;
                }
            }

            yield return null;
        }
    }

    public void SetNewLookCentralObjet(GameObject newTarget)
    {
        if (centralObject != newTarget)
        {
            Debug.Log("Start Body New Look Object");
            isChangingLook = true;
            
            centralObject = newTarget;

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
                
            transitionCoroutine = StartCoroutine(SmoothLookAtNewTarget());
        }
    }

    IEnumerator SmoothLookAtNewTarget()
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = body.transform.rotation;
        Vector3 targetPosition;

        if (centralObject != null)
        {
            targetPosition = new Vector3(centralObject.transform.position.x, centralObject.transform.position.y, centralObject.transform.position.z);
        }
        else
        {
            targetPosition = new Vector3(initialBodyHolderPosition.x, body.transform.position.y, initialBodyHolderPosition.y);
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            Vector3 direction = (targetPosition - body.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            body.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // Ensure the final rotation is exactly towards the new target
        body.transform.LookAt(targetPosition);

        if (centralObject != null)
        {
            RobertaController.Instance.moveLocomotionController.movementRoberta.m_LookAt = centralObject.transform;
        }
        else
        {
            RobertaController.Instance.moveLocomotionController.movementRoberta.m_LookAt = RobertaController.Instance.playerPointOfView.transform;
        }
        
        Debug.Log("End Body New Look Object");
        isChangingLook = false;
    }

    public void LookAtCentralObject()
    {
        if (!isChangingLook)
        {
            if (centralObject != null)
            {
                body.transform.LookAt(new Vector3(centralObject.transform.position.x, centralObject.transform.position.y, centralObject.transform.position.z));
            }
        }

        Orbit();
    }

    void Orbit()
    {
        if (oscilationWaitPoints.Length == 0 || RobertaController.Instance.isInteracting)
        {
            return;
        }

        Vector3 adjusment = oscilationWaitPoints[destPoint].transform.position;
        adjusment.z = bodyHolder.transform.position.z;
        adjusment.x = bodyHolder.transform.position.x;

        bodyHolder.transform.position = (Vector3.MoveTowards(bodyHolder.transform.position, adjusment, oscillationSpeed * Time.deltaTime));

        if (Vector3.Distance(bodyHolder.transform.position, oscilationWaitPoints[destPoint].transform.position) < 0.1f)
        {
            NextPointFly();
        }
    }

    void NextPointFly()
    {
        currentPositionIndex = (currentPositionIndex + 1) % oscilationWaitPoints.Length;
        destPoint = currentPositionIndex;
    }

    public void OrientationFly(float currentPositionX, float nextPositionX, float currentPositionY, float nextPositionY)
    {
        if (RobertaController.Instance.moveLocomotionController.isMovingToNextHeightPoint || RobertaController.Instance.moveLocomotionController.isMovingToNextPoint)
        {
            if (currentPositionX > nextPositionX)
            {
                if (rotation > -.7f)
                {
                    rotation -= Time.fixedDeltaTime;
                }
            }
            else if (currentPositionX < nextPositionX)
            {
                if (rotation < .7f)
                {
                    rotation += Time.fixedDeltaTime;
                }
            }

            if (currentPositionY > nextPositionY)
            {
                if (horizon > -0.29f)
                {
                    horizon -= Time.fixedDeltaTime;
                }
            }
            else if (currentPositionY < nextPositionY)
            {
                if (horizon < 0.29f)
                {
                    horizon += Time.fixedDeltaTime;
                }
            }
        }
        else
        {
            StopValue(horizon);
            StopValue(rotation);
        }
    }

    public void SetMoveVelocity(int speed)
    {
        switch (speed)
        {
            case 0:
                moveSpeed = moveBodySlowSpeed;
                break;

            case 1:
                moveSpeed = moveBodyNormalSpeed;
                break;

            case 2:
                moveSpeed = moveBodyRunSpeed;
                break;
        }
    }

    public void SetSpeakingVelocity(int speed)
    {
        switch (speed)
        {
            case 0:
                moveTalkingSpeed = speakingSlowSpeed;
                break;

            case 1:
                moveTalkingSpeed = speakingRunSpeed;
                break;
        }
    }

    public void SetLookTransitionVelocity(int speed)
    {
        Debug.Log("Cambiar velocidad");
        switch (speed)
        {
            case 0:
                transitionDuration = transitionDurationSlowSpeed;
                break;

            case 1:
                transitionDuration = transitionDurationRunSpeed;
                break;
        }
    }

    public void SetOrbitVelocity(int velocity)
    {
        switch (velocity)
        {
            case 0:
                oscillationSpeed = oscillationNormalSpeed;
                break;

            case 1:
                oscillationSpeed = oscilationRunSpeed;
                break;
        }
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }
    
    public float StopValue(float value)
    {
        // Reducir gradualmente rotation y horizon a 0
        return Mathf.MoveTowards(value, 0, stopSpeed * Time.deltaTime);
    }
}
