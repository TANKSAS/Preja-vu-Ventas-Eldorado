using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RobertaLocomotionController : MonoBehaviour
{
    public CinemachineFreeLook movementRoberta; //Camara Free look encargada de el posicionamiento del cuerpo de roberta
    public int destPoint;
    public int heightDestPoint;
    public bool isMovingToNextPoint; // Flag para moverse a puntos específicos
    public bool isSlowingDown;
    public bool moveConstantly; // Flag para moverse constantemente
    public bool isMovingToNextHeightPoint;
    public bool isProjecting; //Estoy proyectando
    public List<float> stopAngles; // Ángulos donde se debe detener
    public List<float> stopHeights; //Angulos eje Y 

    [SerializeField] private float horizontalSlowSpeed;
    [SerializeField] private float verticalSlowSpeed;
    [SerializeField] private float horizontalNormalSpeed;
    [SerializeField] private float verticalNormalSpeed;
    [SerializeField] private float horizontalRunSpeed;
    [SerializeField] private float verticalRunSpeed;
    [SerializeField] private float verticalSpeed;// Velocidad de cambio en el eje Y
    [SerializeField] private float horizontalSpeed; // Velocidad de cambio en el eje X
    [SerializeField] private int currentStopIndex;// Índice actual de la lista de ángulos
    [SerializeField] private int currentHeightIndex; //Indice Y
    
    public void Movement()
    {
        if (moveConstantly)
        {
            OrbitConstantly();
        }
        else
        {
            GotoNextPointFly();
        }

        if (isMovingToNextHeightPoint)
        {
            GotoNextPointHeigh();
        }
    }

    public void SetNewHorizontalPosition(int newPosition)
    {
        if (destPoint == newPosition)
            return;
        Debug.Log("Start Move Horizontal");

        isMovingToNextPoint = true;
        float currentAngle = stopAngles[currentStopIndex];
        float targetAngle = stopAngles[newPosition];
        float directDistance = Mathf.Abs(targetAngle - currentAngle);
        float complementaryDistance = 360f - directDistance;

        // Encuentra la ruta más corta
        if (directDistance <= complementaryDistance)
        {
            // Movimiento directo
            if (targetAngle > currentAngle)
            {
                horizontalSpeed = Mathf.Abs(horizontalSpeed);
            }
            else
            {
                horizontalSpeed = -Mathf.Abs(horizontalSpeed);
            }
        }
        else
        {
            // Movimiento complementario (dar la vuelta)
            if (targetAngle < currentAngle)
            {
                horizontalSpeed = Mathf.Abs(horizontalSpeed);
            }
            else
            {
                horizontalSpeed = -Mathf.Abs(horizontalSpeed);
            }
        }
        
        destPoint = newPosition;
    }

    public void SetNewHeightPosition(int newHeight)
    {
        if (heightDestPoint == newHeight)
            return;

        Debug.Log("Start Move Vertical");

        float currentAngle = stopHeights[currentHeightIndex];
        float targetAngle = stopHeights[newHeight];

        // Movimiento directo
        verticalSpeed = targetAngle > currentAngle ? Mathf.Abs(verticalSpeed) : -Mathf.Abs(verticalSpeed);
        heightDestPoint = newHeight;
        isMovingToNextHeightPoint = true;
    }

    void GotoNextPointFly()
    {
        // Returns if no points have been set up
        if (stopAngles.Count == 0 || !isMovingToNextPoint  || isSlowingDown)
            return;

        // Get current and destination angles
        float currentAngle = movementRoberta.m_XAxis.Value;
        float targetAngle = stopAngles[destPoint];

        // Calculate the shortest distance between the angles
        float distance = Mathf.DeltaAngle(currentAngle, targetAngle);

        // If the distance is very small, stop moving and set the current stop index
        if (Mathf.Abs(distance) <= 2f)
        {
            StartCoroutine(SlowingDownX(targetAngle));
            isSlowingDown = true;
        }
        else
        {
            OrbitConstantly();// Continue orbiting
        }
    }

    IEnumerator SlowingDownX(float value)
    {
        float valueNewPosition = value;
        float transitionDuration = Mathf.Abs(horizontalSpeed);
        
        while (Mathf.Abs(Mathf.DeltaAngle(movementRoberta.m_XAxis.Value, valueNewPosition)) > 0.01f)
        {
           // Debug.Log(Mathf.Abs(Mathf.DeltaAngle(movementRoberta.m_XAxis.Value, valueNewPosition)));
            movementRoberta.m_XAxis.Value = Mathf.MoveTowardsAngle(movementRoberta.m_XAxis.Value, valueNewPosition, transitionDuration * Time.deltaTime);
            yield return null;
        }

        RobertaController.Instance.bodyController.SetCurrentPosition();
        isSlowingDown = false;
        isMovingToNextPoint = false;
        currentStopIndex = destPoint;
        Debug.Log("End Move Horizontal");
    }

    void GotoNextPointHeigh()
    {
        // Returns if no points have been set up
        if (stopHeights.Count == 0 && !isMovingToNextHeightPoint || isSlowingDown)
            return;
        
        //Debug.Log(movementRoberta.m_YAxis.Value);

        if (DistanceAngleNextPoint(movementRoberta.m_YAxis.Value, stopHeights[heightDestPoint]) < 0.01f)
        {
            StartCoroutine(SmoothTruncateYValues(stopHeights[heightDestPoint]));
            isSlowingDown = true;   
        }
        else
        {
            MoveHeight();
        }
    }

    IEnumerator SmoothTruncateYValues(float value)
    {
        float valueNewPosition = value;
        float transitionDuration = Mathf.Abs(verticalSpeed);

        while (movementRoberta.m_YAxis.Value != value)
        {
            movementRoberta.m_YAxis.Value = Mathf.MoveTowards(movementRoberta.m_YAxis.Value, valueNewPosition, transitionDuration * Time.deltaTime);
            yield return null;
        }
        
        RobertaController.Instance.bodyController.SetCurrentPosition();
        isMovingToNextHeightPoint = false;
        isSlowingDown = false;
        currentHeightIndex = heightDestPoint;

        Debug.Log("End Move Vertical");
    }

    public void MoveHeight()
    {
        if (isSlowingDown)
            return; 
        
        if (movementRoberta.m_YAxis.Value < 0)
         {
            movementRoberta.m_YAxis.Value = 0;
         }
        
        movementRoberta.m_YAxis.Value = Mathf.Clamp(movementRoberta.m_YAxis.Value + verticalSpeed * Time.deltaTime, 0, 1);
      //  (movementRoberta.m_YAxis.Value +  verticalSpeed * Time.deltaTime);
    }
    
    void OrbitConstantly()
    {
        if (isSlowingDown)
            return;

        if (movementRoberta.m_XAxis.Value < 0)
        {
            movementRoberta.m_XAxis.Value += 360;
        }

        movementRoberta.m_XAxis.Value = (movementRoberta.m_XAxis.Value + horizontalSpeed * Time.deltaTime) % 360;
    }

    // agregar a esta funcion, que pueda identificar en que punto de la lista va segun su posicion actual y remplazar el current position 
    void EndOrbitConstantly()
    {
        moveConstantly = false;
    }

    float DistanceAngleNextPoint(float value1, float value2)
    {
        return Mathf.Abs(value1 - value2);
    }

    #region Sets
    // crear nuevo set de velocidades en X y aparte para Y 

    public void SetMoveConstantly(bool move)
    {
        if (move)
        {
            moveConstantly = true;
        }
        else
        {
            EndOrbitConstantly();
        }
    }

    public void SetStopAngles(List<float> newStopAngles)
    {
        stopAngles = newStopAngles;
        currentStopIndex = 0;
    }

    public void SetStopHeights(List<float> newStopHeights)
    {
        stopHeights = newStopHeights;
        currentHeightIndex = 0;
    }

    public void SetVerticalSpeed(int speedY)
    {
        switch (speedY)
        {
            case 0:
                verticalSpeed = verticalSlowSpeed;
                break;

            case 1:
                verticalSpeed = verticalNormalSpeed;
                break;

            case 2:
                verticalSpeed = verticalRunSpeed;
                break;
        }
    }

    public void SetHorizontalSpeed(int speedX)
    {
        switch (speedX)
        {
            case 0:
                horizontalSpeed = horizontalSlowSpeed;
                break;

            case 1:
                horizontalSpeed = horizontalNormalSpeed;
                break;

            case 2:
                horizontalSpeed = horizontalRunSpeed;
                break;
        }
    }

    #endregion
}
