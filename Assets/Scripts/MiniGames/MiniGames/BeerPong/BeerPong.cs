using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static RuntimeSettings;

public class BeerPong : MiniGame
{
    [SerializeField] private QTE qte; // Reference to the QTE script
    [SerializeField] private GameObject qteGameObject;
    [SerializeField] private PhysicsMaterial bounceMaterial; // Bouncy material for the ball
    [SerializeField] private int chances;
    [SerializeField] private float baseStrenght;
    [SerializeField] private float strenghtModifier;
    [SerializeField] private DraggableBall dragBall;
    [SerializeField] private Transform shootingPosition;


    [Header("Cup Management")]
    [SerializeField] private BeerPongCup[] playerCups;
    private Dictionary<int,Transform> playerCupsDict = new Dictionary<int,Transform>();
    private Dictionary<int,Transform> playerWonCups = new Dictionary<int,Transform>();

    [SerializeField] private BeerPongCup[] aiCups;
    private Dictionary<int,Transform> aiCupsDict = new Dictionary<int,Transform>();
    private Dictionary<int,Transform> aiWonCups = new Dictionary<int,Transform>();

    [Header("Cup Collection Area Colliders")]
    [SerializeField] private BoxCollider playerWonCupsArea;
    [SerializeField] private BoxCollider aiWonCupsArea;
    [SerializeField] private Transform aiBallPosition;

    [Header("UI")]
    [SerializeField] private CanvasGroup shotsCanvasGroup;
    [SerializeField] private Image shotOne;
    private bool shotOneIsTaken;
    [SerializeField] private Image shotTwo;
    private bool shotTwoIsTaken;

    private BeerPongMinigameStates currentState;
    private BeerPongMinigameStates nextState;
    private Bounds winAreaBounds;

    [SerializeField]
    private float angle = 45f;
    private Coroutine activeMonitor;
    private bool isTransitioning = false;
    private Vector3[] futureBallPositions = new Vector3[5];
    [SerializeField] private bool allowErrorMargin = true;
    
    void OnEnable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange += StateChanged;
        GameEvents.OnRequestShowQTE += ShowQTE;
        GameEvents.OnRequestHideQTE += HideQTE;
        GameEvents.OnPingPongBallEnterCup += HandleCupWin;
        GameEvents.OnPingPongBallMissedCup += HandleMiss;
    }

    void OnDisable()
    {
        GameEvents.OnRequestBeerPongMinigameStateChange -= StateChanged;
        GameEvents.OnRequestShowQTE -= ShowQTE;
        GameEvents.OnRequestHideQTE -= HideQTE;
        GameEvents.OnPingPongBallEnterCup -= HandleCupWin;
        GameEvents.OnPingPongBallMissedCup -= HandleMiss;
    }

    private void Start()
    {
        dragBall.SetBeerPong(this);
        foreach (BeerPongCup cup in playerCups)
        {
            playerCupsDict.Add(cup.GetID(), cup.transform);
        }
        foreach (BeerPongCup cup in aiCups)
        {
            aiCupsDict.Add(cup.GetID(), cup.transform);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ResetMinigame(currentState));
        }
        if (IsPlayerTurn() && dragBall.Rb.isKinematic)
        {
            CalculateFutureBall();
            dragBall.lineRenderer.positionCount = futureBallPositions.Length;
            dragBall.lineRenderer.SetPositions(futureBallPositions);
        }
        else if (dragBall.lineRenderer != null)
        {
            dragBall.lineRenderer.positionCount = 0;
        }
    }

    public void CalculateFutureBall()
    {
        var startPosition = dragBall.transform.position;
        var timeInterval = 0.1f;
        Vector3 throwDirection = (dragBall.transform.forward).normalized;
        float throwForce = baseStrenght + strenghtModifier * qte.Strenght;
        
        for (int i = 0; i <= futureBallPositions.Length - 1; i++){
            
            var simulatedTime = i * timeInterval;

            var initialVelocity = (throwDirection * throwForce) * simulatedTime;

            var gravity = 0.5f * (Physics.gravity * Mathf.Pow(simulatedTime, 2));

            var calculatedFuturePosition = startPosition + initialVelocity + gravity;
            futureBallPositions[i] = calculatedFuturePosition;
        }
    }

    public void OnBallRelease()
    {
        // Calculate throw direction and force based on mouse movement
        Vector3 throwDirection = (dragBall.transform.forward).normalized;
        float throwForce = baseStrenght + strenghtModifier * qte.Strenght;
        dragBall.Rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        qte.Stop();
        Debug.Log("[BeerPong] Threw ball with force: " + throwForce);
        activeMonitor = StartCoroutine(MonitorBall());
    }

    private IEnumerator AIMoveTurn()
    {
        // okay for this im gonna have to emulate the player ball movement
        // given that the AI cant use a mouse
        float fixedY = 1.6f; // value taken from `DraggableBall`
        Sequence moveToAISideSequence = DOTween.Sequence();
        moveToAISideSequence.Append(dragBall.transform.DOMoveY(fixedY, 0.5f));
        moveToAISideSequence.Append(dragBall.transform.DOMove(aiBallPosition.position, 0.5f));
        
        yield return moveToAISideSequence.WaitForCompletion();
        isTransitioning = false;
        yield return new WaitForSeconds(1f);

        float minX = -0.05f;
        float maxX = 0.05f;
        float minZ = -0.05f;
        float maxZ = 0.05f;

        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomZ = UnityEngine.Random.Range(minZ, maxZ);

        Vector3 errorMargin = new Vector3(randomX, 0, randomZ);
        var targetCupID = CalculateNearestCupNeighbour();
        Vector3 targetCupTransform = playerCupsDict[targetCupID].position;
        if (!allowErrorMargin) errorMargin = Vector3.zero;
        Vector3 e_targetCupTransform = targetCupTransform + errorMargin;
        float cupHeight = 0.5f;
        e_targetCupTransform.y += cupHeight; // im adding the cup's height in order for the ball to hit the rim, rather than hit the bottom of the cup
        Vector3 requiredVel = CalculateDistanceToCup(dragBall.transform.position, e_targetCupTransform);
        Rigidbody ballRigidbody = dragBall.Rb;

        // firing sequence
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.isKinematic = false;
        ballRigidbody.useGravity = true;
        Debug.Log($"[BeerPong] AI is shooting at cup ID: {targetCupID}, with a vel of {requiredVel}, with error margin: {errorMargin}");
        ballRigidbody.AddForce(requiredVel, ForceMode.VelocityChange);
        activeMonitor = StartCoroutine(MonitorBall());
    }


    private void HandleCupWin(int ID, Transform cupTransform)
    {
        nextState = currentState;
        if (isTransitioning) return;
        isTransitioning = true;
        if(activeMonitor != null)
        {
            StopCoroutine(activeMonitor);
            activeMonitor = null;
        }
        
        if (currentState == BeerPongMinigameStates.PlayerTurn)
        {
            if (!playerCupsDict.ContainsKey(ID))
            {
                playerWonCups.Add(ID, cupTransform);
            }
            winAreaBounds = playerWonCupsArea.bounds;
            aiCupsDict.Remove(ID);
            nextState = BeerPongMinigameStates.AITurn;
        }
        if (currentState == BeerPongMinigameStates.AITurn)
        {
            if (!aiCupsDict.ContainsKey(ID))
            {
                aiWonCups.Add(ID, cupTransform);
            }
            winAreaBounds = aiWonCupsArea.bounds;
            playerCupsDict.Remove(ID);
            nextState = BeerPongMinigameStates.PlayerTurn;
        }
        
        int cupIndex = (currentState == BeerPongMinigameStates.PlayerTurn) ? playerWonCups.Count - 1 : aiWonCups.Count - 1;
        float cupSpacing = 0.3f;

        
        Vector3 newCupPosition = new Vector3(
            winAreaBounds.center.x, 
            winAreaBounds.max.y, 
            winAreaBounds.min.z + (cupIndex * cupSpacing) + cupSpacing
        );

        // hey. whoever is reading this, PLEASE.
        // please. stay safe tweening. i might be slow
        // i might be dumb, but i can NOT tween this.
        // oki thx byebye
        cupTransform.position = newCupPosition;

        if (cupTransform.TryGetComponent<Collider>(out Collider cupCollider))
        {
            cupCollider.enabled = false;
        }

        shotOne.color = new Color(255f, 255f, 255f, 1f);
        shotOneIsTaken = false;

        shotTwo.color = new Color(255f, 255f, 255f, 1f);
        shotTwoIsTaken = false;

        StartCoroutine(ResetMinigame(nextState));

    }

    private void HandleMiss()
    {
        nextState = currentState;
        if (isTransitioning) return;
        isTransitioning = true;
        if(activeMonitor != null)
        {
            StopCoroutine(activeMonitor);
            activeMonitor = null;
        }
        if (currentState == BeerPongMinigameStates.PlayerTurn)
        {
            if (!shotOneIsTaken)
            {
                shotOne.color = new Color(255f, 0f, 0f, 1f);
                shotOneIsTaken = true;
            }
            else if (!shotTwoIsTaken)
            {
                shotTwo.color = new Color(255f, 0f, 0f, 1f);
                shotTwoIsTaken = true;
            }
            if (shotOneIsTaken && shotTwoIsTaken)
            {
                nextState = BeerPongMinigameStates.AITurn;
            
                shotOne.color = new Color(255f, 255f, 255f, 1f);
                shotOneIsTaken = false;

                shotTwo.color = new Color(255f, 255f, 255f, 1f);
                shotTwoIsTaken = false;
            }
        }

        if (currentState == BeerPongMinigameStates.AITurn)
        {
            if (!shotOneIsTaken)
            {
                shotOne.color = new Color(255f, 0f, 0f, 1f);
                shotOneIsTaken = true;
            }
            else if (!shotTwoIsTaken)
            {
                shotTwo.color = new Color(255f, 0f, 0f, 1f);
                shotTwoIsTaken = true;
            }
            if (shotOneIsTaken && shotTwoIsTaken)
            {
                nextState = BeerPongMinigameStates.PlayerTurn;

                shotOne.color = new Color(255f, 255f, 255f, 1f);
                shotOneIsTaken = false;

                shotTwo.color = new Color(255f, 255f, 255f, 1f);
                shotTwoIsTaken = false;
            }
        }
        StartCoroutine(ResetMinigame(nextState));
    }

    private IEnumerator MonitorBall()
    {
        yield return new WaitForSeconds(1f); // give leway for flight time
        float totalTime = 0f;
        while (totalTime < 3f){
            if (dragBall.Rb.linearVelocity.sqrMagnitude < 0.05f && dragBall.Rb.angularVelocity.sqrMagnitude < 0.05f)
            {
                HandleMiss();
                yield break;
            }
            totalTime += Time.deltaTime;
            yield return null;
        }
        HandleMiss();
    }

    #region AI Logic
    // i'd like to point out that the functions are placed in their execution order
    private int CalculateNearestCupNeighbour()
    {
        // key value pair is ID, score
        Dictionary<int, int> cupScoreMap = new Dictionary<int, int>();
        var radius = 0.15f;

        foreach (KeyValuePair<int, Transform> cupOne in playerCupsDict)
        {
            cupScoreMap.Add(cupOne.Key, 0);
            foreach (KeyValuePair<int, Transform> cupTwo in playerCupsDict)
            {
                Debug.Log($"Comparing cup with ID: {cupOne.Key} againt: {cupTwo.Key}");
                if (cupOne.Key == cupTwo.Key)
                {
                    continue;
                }
                else
                {
                    var distance = cupOne.Value.position - cupTwo.Value.position;
                    var magnitueOfDistance = distance.magnitude;

                    if (magnitueOfDistance <= radius)
                    {
                        cupScoreMap[cupOne.Key]++;
                    }
                }
            }
        }

        var highestScore = -1;
        int targetID = 0;
        foreach (KeyValuePair<int, int> cup in cupScoreMap)
        {
            if (cup.Value > highestScore)
            {
                targetID = cup.Key;
                highestScore = cup.Value;
            } 
            // for ties, im doing what i would do in beer pong if i ever played and going for the easiest shot
            // which is what cup is closest
            else if (cup.Value == highestScore)
            {
                var targetIDPosition_BallPosition = playerCupsDict[targetID].position - aiBallPosition.position;
                var currentCupIDPosition_BallPosition = playerCupsDict[cup.Key].position - aiBallPosition.position;

                if (currentCupIDPosition_BallPosition.magnitude < targetIDPosition_BallPosition.magnitude)
                {
                    targetID = cup.Key;
                }
            }
        }
        return targetID;
    }

    private Vector3 CalculateDistanceToCup(Vector3 startPosition, Vector3 targetPosition)
    {
        float verticalDistance = targetPosition.y - startPosition.y;

        Vector3 distance = targetPosition - startPosition;
        distance.y = 0f;

        var horizontalDistance = distance.magnitude;

        var flatDirection = distance.normalized;
        var forwardVector = flatDirection * Mathf.Cos(Mathf.Deg2Rad * angle);
        var upwardsVector = Vector3.up * Mathf.Sin(Mathf.Deg2Rad * angle);
        var finalDirectionVector = forwardVector + upwardsVector;

        float requiredSpeed = CalculateVelocity(horizontalDistance, verticalDistance);
        var finalVelocity = requiredSpeed * finalDirectionVector;
        return finalVelocity;
    }

    // this is just the kinematic equaiton :D
    private float CalculateVelocity(float x, float y)
    {
        // when im in an anti-magic number competition and my opponent is myself
        var tan = Mathf.Tan(Mathf.Deg2Rad * angle);
        var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        var gravity = Mathf.Abs(Physics.gravity.y);

        var velocity = Mathf.Sqrt(
            gravity * Mathf.Pow(x, 2) 
            / 
            (2 * Mathf.Pow(cos, 2) * (x * tan - y) )
        );

        return velocity;
    }
    #endregion
    public bool IsPlayerTurn()
    {
        return currentState == BeerPongMinigameStates.PlayerTurn && !isTransitioning;
    }
    public IEnumerator ResetMinigame(BeerPongMinigameStates beerState)
    {
        dragBall.transform.rotation = Quaternion.Euler(-30, 0, 0);
        dragBall.ResetMiniGame();

        Sequence resetSequence = DOTween.Sequence();
        if (beerState == BeerPongMinigameStates.PlayerTurn)
        {
            resetSequence.Append(dragBall.transform.DOMove(dragBall.ballStartPosition, 0.75f));
            yield return resetSequence.WaitForCompletion();
            isTransitioning = false;
        }
        qte.Start();

        GameEvents.ChangeBeerPongState(beerState);
        
        if (beerState == BeerPongMinigameStates.AITurn)
        {
            StartCoroutine(AIMoveTurn());
        }
        
    }

    #region UI & State
    private void ShowQTE()
    {
        qteGameObject.SetActive(true);
    }

    private void HideQTE()
    {
        qteGameObject.SetActive(false);
    }

    private void ShowShotCount()
    {
        GameEvents.RequestFadeInUIElement(defaultTweenDuration, shotsCanvasGroup);
    }
    private void HideShotCount()
    {
        GameEvents.RequestFadeOutUIElement(defaultTweenDuration, shotsCanvasGroup);
    }

    private void StateChanged(BeerPongMinigameStates beerPongMinigameStates)
    {
        currentState = beerPongMinigameStates;
    }
    #endregion
}
