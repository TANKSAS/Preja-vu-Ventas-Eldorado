using Convai.Scripts.Runtime.Core;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobertaController : Singleton<RobertaController>
{
    public RobertaBodyController bodyController;
    public RobertaAIController robertaAI; 
    public RobertaAnimationController animationController;
    public RobertaLocomotionController moveLocomotionController;
    public RobertaProjectingController projectingController;
    public ParticlesController particlesController;
    public RobertaSoundController sfxController;
    public RotateFan[] rotateFans;
    public GameObject player;
    public GameObject playerPointOfView;
    public GameObject[] lookPoints;
    public GameObject[] movePoints;
    public float minProjectingHigh = 0.55f;
    
    public int newMoveBodyPosition;
    public int newLookProjectionPositionIndex;
    public int newProjectingObjectPositionIndex;
    public int newProjectionObjectIndex;

    public LineRenderer lineRenderer; // Referencia al LineRenderer
    public Transform firePoint; // El punto desde donde se disparará el láser
    public float laserRange = 50f; // Máxima distancia del láser
    public float laserExtendSpeed = 20f; // Velocidad a la que el láser se extiende
    public LayerMask laserLayerMask; // Definir qué capas puede colisionar el láser

    private Coroutine laserCoroutine;
    
    public bool isInteracting;
    public bool isPowerOn;
    public bool isTurningOff;
    public bool isGrounded;
    public bool isWaiting;
    public bool isRecharging;

    private bool isLaserActive;
    private bool buttonPressed;

    #region Monobeheviur

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerPointOfView = CharacterManager.Instance?.playerPointOfView.gameObject;
            moveLocomotionController.movementRoberta.Follow = player.transform;
            moveLocomotionController.movementRoberta.LookAt = playerPointOfView.transform;
            RobertaStart();
        }
    }

    public IEnumerator RobertaGoToCinematic(int cinematicIndex)
    {
        UIManager.Instance.ChanceMusicBackGround(1); 
        UIManager.Instance.ChanceMusicBackGround(4);

        SlowVelocity();
        yield return StartCoroutine(GoToCentralPoint());
        GameManager.Instance.timeLineController.SetPlayableDirector(cinematicIndex);
        GameManager.Instance.timeLineController.Play();


        Debug.Log($"Reproduciendo cinemática en el índice: {cinematicIndex}");
        yield return new WaitUntil(() => !GameManager.Instance.timeLineController.StatePlayable());
        yield return StartCoroutine(EndCinematicPosition());

        UIManager.Instance.ChanceMusicBackGround(5);
        UIManager.Instance.ChanceMusicBackGround(0);
    }

    public IEnumerator RobertaGoToFeedBack(AssessmentModule assessment ,int cinematicIndex)
    {
        UIManager.Instance.ChanceMusicBackGround(4);

        SlowVelocity();
        yield return StartCoroutine(GoToCentralPoint());

        // esto funcionara cuando implementemos una cinematica final de feedback 
        //GameManager.Instance.timeLineController.Play(cinematicIndex);
        //Debug.Log($"Reproduciendo cinemática en el índice: {cinematicIndex}");
        //yield return new WaitUntil(() => !GameManager.Instance.timeLineController.StatePlayable(cinematicIndex));

        yield return StartCoroutine(robertaAI.StartFinalTestFeedBack(assessment));

        yield return StartCoroutine(EndCinematicPosition());
        UIManager.Instance.ChanceMusicBackGround(5);
        UIManager.Instance.ChanceMusicBackGround(0);
    }

    public IEnumerator RobertaGoToExercise(int cinematicIndex, int caseMethodologyIndex)
    {
        UIManager.Instance.ChanceMusicBackGround(1);
        UIManager.Instance.ChanceMusicBackGround(4);

        SlowVelocity();
        yield return StartCoroutine(GoToCentralPoint());

        //GameManager.Instance.timeLineController.Play(cinematicIndex);
        //Debug.Log($"Reproduciendo cinemática en el índice: {cinematicIndex}");
        //yield return new WaitUntil(() => !GameManager.Instance.timeLineController.StatePlayable(cinematicIndex));

        yield return StartCoroutine(robertaAI.StartFinalTestFeedBack(GameManager.Instance.finalTestController));
       // yield return StartCoroutine(robertaAI.StartQuestion(caseMethodologyIndex));

        yield return StartCoroutine(EndCinematicPosition());
        UIManager.Instance.ChanceMusicBackGround(5);
        UIManager.Instance.ChanceMusicBackGround(0);
    }

    void Update()
    {
        if (isLaserActive)
        {

            RaycastHit hit;
            Vector3 laserEndPoint;

            // Lanza un rayo desde el punto de disparo hacia adelante (en la dirección que está mirando el jugador)
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, laserRange, laserLayerMask))
            {
                // Si el rayo colisiona con un objeto, el láser termina en el punto de colisión
                laserEndPoint = hit.point;
            }
            else
            {
                // Si no colisiona con ningún objeto, el láser termina en su máximo alcance
                laserEndPoint = firePoint.position + firePoint.forward * laserRange;
            }

            // Definir los puntos inicial y final del láser en el LineRenderer
            lineRenderer.SetPosition(0, firePoint.position); // Posición inicial del láser (en el firePoint)
            lineRenderer.SetPosition(1, laserEndPoint); // Posición final del láser (colisión o distancia máxima)

        }
        Test();
        //animationController.SetAnimation(bodyController.horizon, bodyController.rotation);

        //if (isInteracting)
        //{
        //    bodyController.StopValue(bodyController.horizon);
        //    bodyController.StopValue(bodyController.rotation);
        //    return;
        //}

        bodyController.RobertaBodyMovement();
        //bodyController.OrientationFly(moveLocomotionController.movementRoberta.m_XAxis.Value, moveLocomotionController.stopAngles[moveLocomotionController.destPoint],
        //moveLocomotionController.movementRoberta.m_YAxis.Value, moveLocomotionController.stopHeights[moveLocomotionController.heightDestPoint]);
        moveLocomotionController.Movement();
    }

    //RayoLaser
    //void OnDrawGizmos()
    //{
    //    // Definir color del Gizmo
    //    Gizmos.color = Color.red;

    //    // Calcular el punto final del láser (máximo alcance)
    //    Vector3 laserEndPoint = firePoint.position + firePoint.forward * laserRange;

    //    // Dibujar línea desde el punto de disparo hasta el alcance máximo del láser
    //    Gizmos.DrawLine(firePoint.position, laserEndPoint);
    //}
    #endregion

    #region INPUTS
    void Test()
    {
        bodyController.Testing();

        Keyboard keyboard = Keyboard.current;

        if (keyboard.dKey.wasPressedThisFrame)
        {
            SetLookAtPosition(0);
            LookAtObject(false);
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            LookAtPlayer();
        }

        if (keyboard.numpad1Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(0);
        }

        if (keyboard.numpad2Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(2);
        }

        if (keyboard.numpad3Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(4);
        }

        if (keyboard.numpad4Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(6);
        }

        if (keyboard.numpad5Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(8);
        }

        if (keyboard.numpad6Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(10);
        }

        if (keyboard.numpad7Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(12);
        }

        if (keyboard.numpad8Key.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHorizontalPosition(14);
        }

        if (keyboard.zKey.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHeightPosition(0);
        }

        if (keyboard.xKey.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHeightPosition(4);
        }

        if (keyboard.cKey.wasPressedThisFrame)
        {
            moveLocomotionController.SetNewHeightPosition(10);
        }

        if (keyboard.pKey.wasPressedThisFrame)
        {
            CallStartProjection();
        }

        if (keyboard.oKey.wasPressedThisFrame)
        {
            GoToCentralPos();
        }

        if (keyboard.iKey.wasPressedThisFrame)
        {
            StartScan();
        }

        if (keyboard.aKey.wasPressedThisFrame)
        {
            RobertaStart();
        }

        if (keyboard.sKey.wasPressedThisFrame)
        {
            RobertaEnd();
        }
    }

    //Variable en roberta que llamada a la cinematica terminada
    public void EndDo(int index)
    {
        UIManager.Instance.CallQuestPresentation(index);
    }
    #endregion

    #region POWER ON/OFF

    //Esta funcion se ejecuta al salir de la animacion
    public void RobertaStart()
    {
        isTurningOff = true;
        animationController.collider.enabled = true;

        particlesController.SetParticlesIndex(0);
        particlesController.StartParticle();
        particlesController.SetParticlesIndex(1);
        particlesController.StartParticle();
        particlesController.SetParticlesIndex(2);
        particlesController.StartParticle();

        SlowVelocity();
        
        bodyController.SetMoveVelocity(0);
        bodyController.SetLookTransitionVelocity(0);
        bodyController.SetSpeakingVelocity(0);
        bodyController.SetOrbitVelocity(0);
        
        for (int i = 0; i < rotateFans.Length; i++)
        {
            rotateFans[i].power = true;
        }
    }

    //Esta funcion ejecuta la salida de un estado
    public void RobertaEnd()
    {
        LookAtObject(true);
        moveLocomotionController.SetNewHeightPosition(0);

        particlesController.SetParticlesIndex(0);
        particlesController.StopParticle();
        particlesController.SetParticlesIndex(1);
        particlesController.StopParticle();
        particlesController.SetParticlesIndex(2);
        particlesController.StopParticle();
        
        SlowVelocity();

        bodyController.SetSpeakingVelocity(0);
        bodyController.SetMoveVelocity(0);
        bodyController.SetLookTransitionVelocity(1);
        

        for (int i = 0; i < rotateFans.Length; i++)
        {
            rotateFans[i].power = false;
        }

        isTurningOff = true;
        animationController.collider.enabled = true;
    }

    #endregion

    #region SPAWN
    
    public void OnButtonPressed()
    {
        buttonPressed = true;
    }

    public IEnumerator Spawning()
    {
        yield return new WaitUntil(()=> !moveLocomotionController.isMovingToNextHeightPoint && !moveLocomotionController.isMovingToNextPoint);

        NormalizedVelocity();
        bodyController.SetLookTransitionVelocity(1);
        SetLookAtPosition(9);
        moveLocomotionController.SetNewHeightPosition(10); // Punto mas alto

        yield return new WaitUntil(() => moveLocomotionController.movementRoberta.m_YAxis.Value > minProjectingHigh);
        Debug.Log("Go Center Air");
        SetMoveBodyPosition(3);
        bodyController.MoveBodyTowardsNextPoint(movePoints[3]); //Center Air

 
        bodyController.MoveBodyTowardsNextPoint(movePoints[newMoveBodyPosition]);
        yield return new WaitUntil(() => !bodyController.isMoving);

        //Al llegar a una altura en Y cambia a el objeto que debe ver.
        LookAtObject(false);
        yield return new WaitUntil(() => !bodyController.isChangingLook);

        //Al llegar a una altura en Y cambia a el objeto que debe ver
        Debug.Log("SpawnTemporal");
    }
    
    #endregion

    #region WAITING
    //Funcion para iniciar la espera
    public void CallStartWaiting()
    {
        Debug.Log("Start Waiting");
        StartCoroutine(Waiting());
    }
    //Funcionalidad de esperar
    IEnumerator Waiting()
    {

        bodyController.SetMoveVelocity(0);
        bodyController.SetLookTransitionVelocity(1);
        SetLookAtPosition(9);
        moveLocomotionController.SetNewHeightPosition(10); // Punto mas alto

        yield return new WaitUntil(() => moveLocomotionController.movementRoberta.m_YAxis.Value > minProjectingHigh);
        Debug.Log("Go Center Air");
        bodyController.MoveBodyTowardsNextPoint(movePoints[3]); //Center Air
        
        yield return new WaitUntil(() => !bodyController.isMoving);

        LookAtObject(false);

        yield return new WaitUntil(() => !bodyController.isChangingLook);
        Debug.Log("Esperando quiz");
        //Visor en tiempo.
    }

    public void EndWait() //Confirmacion
    {
        //Desactivar efectos especiales
        LookAtPlayer();
        GoToCentralPos();
    }
    #endregion

    #region RECHARGING
    public void StartCharging()
    {
        isInteracting = true;
        isRecharging = true;

        SlowVelocity();
        
        bodyController.SetSpeakingVelocity(0);
        bodyController.SetMoveVelocity(0);
        bodyController.SetLookTransitionVelocity(1);
        
        LookAtObject(true);
        
        moveLocomotionController.SetNewHeightPosition(0);
        
        animationController.collider.enabled = true;

        for (int i = 0; i < rotateFans.Length; i++)
        {
            rotateFans[i].power = false;
        }
    }

    public void EndCharging()
    {
        isInteracting = false;
        isRecharging = false;
       
        SlowVelocity();
        
        bodyController.SetMoveVelocity(0);
        bodyController.SetLookTransitionVelocity(0);
        bodyController.SetSpeakingVelocity(0);
        bodyController.SetOrbitVelocity(0);
        
        animationController.collider.enabled = true;

        for (int i = 0; i < rotateFans.Length; i++)
        {
            rotateFans[i].power = true;
        }
    }

    #endregion

    #region OBSERVER

    //Funcion encargada para mirar al player
    public void LookAtPlayer()
    {
        bodyController.SetNewLookCentralObjet(playerPointOfView);
    }

    //funcion encargada para mirar a un nuevo punto apartir de una lista 
    public void LookAtObject(bool isNullView)
    {
        if (isNullView)
        {
            bodyController.SetNewLookCentralObjet(null);
        }
        else
        {
            GameObject newCentralObject = lookPoints[newLookProjectionPositionIndex];
            bodyController.SetNewLookCentralObjet(newCentralObject);
        }
    }
    #endregion

    #region SCANNING 
    public void StartScan()
    {
        //Esta corrutina hace que Roberta se acerque y escanee a el jugador.
        StartCoroutine(Scanning());
    }
    //Funcion Para escanear al jugador
    IEnumerator Scanning()
    {
        NormalizedVelocity();
        Debug.Log("EscanenadoPlayer");
        moveLocomotionController.SetNewHeightPosition(5); // //Altura de la cadera (4)
        //moveLocomotionController.SetNewHorizontalPosition(4); //Altura de la cadera (4)
        //LookAtPlayer(); //Escaneando al jugador

        yield return new WaitUntil(() => !moveLocomotionController.isMovingToNextHeightPoint);

        bodyController.MoveBodyTowardsNextPoint(movePoints[9]); //Aproach
        Debug.Log("StartToMove");

        // Activar animación y partículas de escaneo
        // bodyController.ScanningPlayer();
        // Activar particula de escaneando al player
        yield return new WaitUntil(() => !bodyController.isMoving);

        SlowVelocity();

        // Esperar 3 segundos mientras escanea
        yield return new WaitForSeconds(6); // Despues de 3 segundos, regresa a su posicion.

        LookAtPlayer();
        yield return new WaitUntil(() => !bodyController.isChangingLook);


        GoToCentralPos();
    }

    #endregion|

    #region PROJECTION 
    public void CallStartProjection()
    {
        Debug.Log("Start Projecting");
        StartCoroutine(Projecting()); //Iniciar la corrutina proyecion
    }

    // Funcion encargada de proyectar 
    IEnumerator Projecting()
    {
        moveLocomotionController.SetVerticalSpeed(0);
        moveLocomotionController.SetNewHeightPosition(7);
        //yield return new WaitUntil(() => !moveLocomotionController.isMovingToNextHeightPoint);
        bodyController.MoveBodyTowardsNextPoint(movePoints[newMoveBodyPosition]); //Acercarce al nuevo punto a proyectar
        yield return new WaitUntil(() => !bodyController.isMoving);

        //Al llegar a una altura en Y cambia a el objeto que debe ver.
        LookAtObject(false);
        yield return new WaitUntil(() => !bodyController.isChangingLook);

        projectingController.SelectNewProjectingPosition(newProjectingObjectPositionIndex);
        projectingController.SelectNewProjectingObject(newProjectionObjectIndex);

        particlesController.SetParticlesIndex(3);
        particlesController.StartParticle();
        particlesController.SetParticlesIndex(4);
        particlesController.StartParticle();

        projectingController.ActivateProjection();//??

        //Activar particula de proyeccion.
        //Terminar con los sprites y la particula.
        Debug.Log("End Projecting");
    }

    public void EndProjection()
    {
        particlesController.SetParticlesIndex(3);
        particlesController.StopParticle();
        particlesController.SetParticlesIndex(4);
        particlesController.StopParticle();
        projectingController.DeactivateProjection();
        LookAtPlayer();
        Debug.Log("End Projecting");
    }

    public void ChangePosition()
    {

    }

    #endregion

    #region Rasho Laser
    public void CallStartPointing()
    {
        Debug.Log("Start Pointing");
        StartCoroutine(ExtendLaser()); //Iniciar la corrutina proyecion
    }

    // Funcion encargada de proyectar 
    IEnumerator ExtendLaser()
    {
        float currentDistance = 0f;
        RaycastHit hit;

        // Inicializar el LineRenderer con dos puntos
        lineRenderer.positionCount = 2;

        while (currentDistance < laserRange)
        {
            // Aumentar progresivamente la distancia del láser
            currentDistance += Time.deltaTime * laserExtendSpeed;

            // Lanza el rayo y verifica si colisiona con algún objeto en la distancia actual
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, currentDistance, laserLayerMask))
            {
                // Si colisiona, el láser se detiene en el punto de colisión
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hit.point);
                yield break; // Salimos de la coroutine cuando el láser choca con algo
            }
            else
            {
                // Si no colisiona, el láser sigue extendiéndose
                lineRenderer.SetPosition(0, firePoint.position);
                Vector3 laserEndPoint = firePoint.position + firePoint.forward * currentDistance;
                lineRenderer.SetPosition(1, laserEndPoint);
            }

            // Esperamos al siguiente frame antes de seguir extendiendo
            yield return null;
        }

        // Si no colisiona con nada y se alcanza el rango máximo
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, firePoint.position + firePoint.forward * laserRange);
    }
    public void ActivateLaser()
    {
        if (!isLaserActive)
        {
            isLaserActive = true;
            lineRenderer.enabled = true;
            laserCoroutine = StartCoroutine(ExtendLaser()); // Iniciar la coroutine que extiende el láser
        }
    }

    public void DeactivateLaser()
    {
        if (isLaserActive)
        {
            isLaserActive = false;
            if (laserCoroutine != null)
            {
                StopCoroutine(laserCoroutine);
            }
            lineRenderer.enabled = false; // Deshabilitar el LineRenderer
        }
    }


    #endregion

    #region SPEAKING PROJECTION Adicional
    public void StartSpeakingProjection(int indexNextLookPoint) //Ready
    {
        Debug.Log("Start SpeakingProjecting");
        StartCoroutine(SpeakingProjecting(indexNextLookPoint)); //Iniciar la corrutina proyecion
    }
    //Funcion creada para proyectar objetos mientras roberta habla.
    IEnumerator SpeakingProjecting(int indexNewProjectionObject) // Se pide el objeto a proyectar
    {

        //Se debe Mira un momento a el objeto.
        LookAtObject(false);
        yield return new WaitUntil(() => !bodyController.isChangingLook);
        //Activar algun SPRITE.
        projectingController.ActivateProjection();
        //Activar particula de señalar aqui 
        //Terminar con los sprites y la particula.
        Debug.Log("End Speaking Projecting");
    }
    #endregion

    #region SPEAKING
    public void StartSpeaking(){

        bodyController.StartTalking();
        StartCoroutine(Speaking()); 
    }

    public void EndSpeaking()
    {
        StopCoroutine(Speaking());
        bodyController.EndTalking();
    }

    IEnumerator Speaking() 
    {
        yield return null;

        //esta funcion debe de tener que cambiar las animaciones de talking , tal vez variar la velocidad o no se xd 
    }

    #endregion

    #region HAPPY
    public void RobertaHappy() //Posicion (X ?, Y 1)
    {
        animationController.SetHappy();
    }
    public void HappyEffect(int indexHappyEffect)
    {
        //ActivarEstrellas
        //ActivarParticulas(indexHappyEffect)
    }

    #endregion

    #region SAD 
    public void RobertaSad() //Posicion (X ?, Y 1)
    {
        //animationController.SetSad();
    }
    public void SadEffect(int indexSadEffect)
    {
        //ActivarLluvia
        //ActivarParticulas(indexSadEffect)
    }

    #endregion

    #region GENERIC FUNTION
    //Finaliza Cualquier estado
    public void GoToCentralPos()
    {
        StartCoroutine(GoToCentralPoint());
    }

    public IEnumerator EndCinematicPosition()
    {
        yield return new WaitUntil(() => !moveLocomotionController.isMovingToNextHeightPoint && !moveLocomotionController.isMovingToNextPoint);

        NormalizedVelocity();

        bodyController.SetLookTransitionVelocity(1);
        
        SetLookAtPosition(9);
        LookAtObject(false);
        
        SetMoveBodyPosition(3);
        bodyController.MoveBodyTowardsNextPoint(movePoints[newMoveBodyPosition]);
        //yield return new WaitUntil(() => !bodyController.isMoving);
        
        moveLocomotionController.SetNewHeightPosition(10); // Punto mas alto
        yield return new WaitUntil(() => !moveLocomotionController.isMovingToNextHeightPoint);
        
        yield return new WaitUntil(() => !bodyController.isChangingLook);
    }

    public void ApproachSpeaking()
    {
        //Acercarse a algun punto lentamente
    }

    //Esta funcion va a el punto central
    public IEnumerator GoToCentralPoint()
    {
        bodyController.StartResetBodyHolderPosition();//devolver el contenedor del cuerpo al origen
        moveLocomotionController.SetNewHeightPosition(4); // cambiara a nueva posiciones
        moveLocomotionController.SetNewHorizontalPosition(0);
        yield return new WaitUntil(()=> moveLocomotionController.movementRoberta.m_YAxis.Value < 0.58f);
        //cambiar velocidad del giro para mirar al player
        LookAtPlayer();
        yield return new WaitUntil(() => !moveLocomotionController.isMovingToNextHeightPoint);
    }

    #endregion

    #region VELOCITIES
    //Velocidad reducida
    public void SlowVelocity()
    {
        Debug.Log("Velocidad Reducida");
        moveLocomotionController.SetVerticalSpeed(0);
        moveLocomotionController.SetHorizontalSpeed(0);
        bodyController.SetMoveVelocity(0);
    }
    
    //Normaliza las velocidades 
    public void NormalizedVelocity()
    {
        Debug.Log("Velocidad Normalizada");
        moveLocomotionController.SetVerticalSpeed(1);
        moveLocomotionController.SetHorizontalSpeed(1);
        bodyController.SetMoveVelocity(1);
    }

    //Velocidad Aumentada
    public void RunVelocity()
    {
        Debug.Log("Velocidad Aumentada");
        moveLocomotionController.SetVerticalSpeed(2);
        moveLocomotionController.SetHorizontalSpeed(2);
        bodyController.SetMoveVelocity(2);
    }
    #endregion

    #region SET's.
    public void SetObjectToProject(int indexNewProjectionObject)
    {
        newProjectionObjectIndex = indexNewProjectionObject;
    }

    public void SetLookAtPosition(int index)
    {
        newLookProjectionPositionIndex = index;
    }

    public void SetPositionToProjectObject(int position)
    {
        newProjectingObjectPositionIndex = position;
    }

    public void SetMoveBodyPosition(int positionIndex)
    {
        newMoveBodyPosition = positionIndex;
    }

    #endregion
}
