using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    [Header("Target Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    
    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomBuffer = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);

    [Header("Split Screen Settings")]
    [SerializeField] private float swapDelay = 1.25f;
    [SerializeField] private float swapOffset = 5f;
    [SerializeField, Range(0, 1)] private float leftSplitPoint = 0.35f;
    [SerializeField, Range(0, 1)] private float rightSplitPoint = 0.65f;
    [SerializeField] private float minDistanceToTrigger = 0f;
    [SerializeField] private float maxAllowedDistance = 11f;
    
    private Camera cam;
    private GameObject player1GameObject;
    private GameObject player2GameObject;
    private float zoneTimer = 0f;
    private bool isTimerActive = false;
    private bool isPlayer1CrossingRight = false;
    private bool isPlayer2CrossingLeft = false;
    private Vector3 centerPosition;

    void Start()
    {
        cam = GetComponent<Camera>();

        player1GameObject = player1.gameObject;
        player2GameObject = player2.gameObject;
        centerPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        if (CheckMaxDistance())
        {
            ResetPlayersToCenter();
            return;
        }

        CheckZoneCrossing();
        HandleZoneSwap();

        Vector3 centerPoint = GetCenterPoint();
        float newZoom = CalculateZoom();
        Vector3 targetPosition = centerPoint + offset;
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        centerPosition = transform.position; 
        
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, smoothSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 newOffset = offset;
            newOffset.z = -newZoom;
            targetPosition = centerPoint + newOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }

    private bool CheckMaxDistance()
    {
        float distance = Mathf.Abs(player1.position.x - player2.position.x);
        if (distance > maxAllowedDistance)
        {
            return true;
        }
        return false;
    }

    private void ResetPlayersToCenter()
    {
        // Get the center point in world space
        Vector3 center = new Vector3(centerPosition.x, player1.position.y, player1.position.z);
        
        // Reset Player 1 to left of center
        Vector3 player1Position = center;
        player1Position.x -= swapOffset;
        player1.position = player1Position;

        // Reset Player 2 to right of center
        Vector3 player2Position = center;
        player2Position.x += swapOffset;
        player2.position = player2Position;

        // Reset all timing and state variables
        isTimerActive = false;
        isPlayer1CrossingRight = false;
        isPlayer2CrossingLeft = false;
        zoneTimer = 0f;
    }

    private bool ArePlayersDistantEnough()
    {
        float distance = Mathf.Abs(player1.position.x - player2.position.x);
        return distance >= minDistanceToTrigger && distance <= maxAllowedDistance;
    }

    private void CheckZoneCrossing()
    {
        Vector3 player1ViewportPos = cam.WorldToViewportPoint(player1.position);
        Vector3 player2ViewportPos = cam.WorldToViewportPoint(player2.position);

        bool player1InRightZone = player1ViewportPos.x > rightSplitPoint;
        bool player2InLeftZone = player2ViewportPos.x < leftSplitPoint;
        bool distanceConditionMet = ArePlayersDistantEnough();

        if (player1InRightZone && !isTimerActive && distanceConditionMet)
        {
            isTimerActive = true;
            isPlayer1CrossingRight = true;
            isPlayer2CrossingLeft = false;
            zoneTimer = 0f;
            Debug.Log($"Player 1 crossed to right zone with valid distance ({Mathf.Abs(player1.position.x - player2.position.x)}), starting timer");
        }
        else if (player2InLeftZone && !isTimerActive && distanceConditionMet)
        {
            isTimerActive = true;
            isPlayer1CrossingRight = false;
            isPlayer2CrossingLeft = true;
            zoneTimer = 0f;
            Debug.Log($"Player 2 crossed to left zone with valid distance ({Mathf.Abs(player1.position.x - player2.position.x)}), starting timer");
        }
        else if ((!player1InRightZone && isPlayer1CrossingRight) || 
                (!player2InLeftZone && isPlayer2CrossingLeft) ||
                (!distanceConditionMet && isTimerActive))
        {
            isTimerActive = false;
            isPlayer1CrossingRight = false;
            isPlayer2CrossingLeft = false;
            zoneTimer = 0f;
        }
    }

    private void HandleZoneSwap()
    {
        if (!isTimerActive) return;

        if (!ArePlayersDistantEnough())
        {
            isTimerActive = false;
            zoneTimer = 0f;
            return;
        }

        zoneTimer += Time.deltaTime;

        if (zoneTimer >= swapDelay)
        {
            if (isPlayer1CrossingRight)
            {
                Vector3 newPosition = player1.position;
                newPosition.x += swapOffset;
                player2.position = newPosition;
                Debug.Log("Swapped Player 2 to the right of Player 1");
            }
            else if (isPlayer2CrossingLeft)
            {
                Vector3 newPosition = player2.position;
                newPosition.x -= swapOffset;
                player1.position = newPosition;
                Debug.Log("Swapped Player 1 to the left of Player 2");
            }

            isTimerActive = false;
            isPlayer1CrossingRight = false;
            isPlayer2CrossingLeft = false;
            zoneTimer = 0f;
        }
    }

    private Vector3 GetCenterPoint()
    {
        Bounds bounds = new Bounds(player1.position, Vector3.zero);
        bounds.Encapsulate(player2.position);
        return bounds.center;
    }

    private float CalculateZoom()
    {
        float distance = Vector3.Distance(player1.position, player2.position);
        float zoom = distance / 2f + zoomBuffer;
        return Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    public void SetPlayers(Transform newPlayer1, Transform newPlayer2)
    {
        player1 = newPlayer1;
        player2 = newPlayer2;
        player1GameObject = newPlayer1.gameObject;
        player2GameObject = newPlayer2.gameObject;
        isTimerActive = false;
        isPlayer1CrossingRight = false;
        isPlayer2CrossingLeft = false;
        zoneTimer = 0f;
    }

    private void OnDrawGizmos()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (!cam) return;

        // Draw left split line
        Gizmos.color = Color.yellow;
        Vector3 leftLine = cam.ViewportToWorldPoint(new Vector3(leftSplitPoint, 0.5f, 10));
        Gizmos.DrawLine(leftLine + Vector3.up * 5, leftLine + Vector3.down * 5);

        // Draw right split line
        Vector3 rightLine = cam.ViewportToWorldPoint(new Vector3(rightSplitPoint, 0.5f, 10));
        Gizmos.DrawLine(rightLine + Vector3.up * 5, rightLine + Vector3.down * 5);

        // Draw distance visualization if both players exist
        if (player1 && player2)
        {
            float currentDistance = Mathf.Abs(player1.position.x - player2.position.x);
            
            // Green: Distance is in valid range
            // Red: Too close or too far
            Color distanceColor = (currentDistance >= minDistanceToTrigger && currentDistance <= maxAllowedDistance) ? Color.green : Color.red;
            
            Gizmos.color = distanceColor;
            Gizmos.DrawLine(player1.position, player2.position);

            // Draw max distance boundaries from center (if camera exists)
            Gizmos.color = Color.red;
            Vector3 centerPos = transform.position;
            Vector3 leftBoundary = centerPos;
            Vector3 rightBoundary = centerPos;
            leftBoundary.x -= maxAllowedDistance / 2;
            rightBoundary.x += maxAllowedDistance / 2;
            
            Gizmos.DrawLine(leftBoundary + Vector3.up * 5, leftBoundary + Vector3.down * 5);
            Gizmos.DrawLine(rightBoundary + Vector3.up * 5, rightBoundary + Vector3.down * 5);
        }
    }
}