using UnityEngine;
using FishNet.Object;

public class CameraLogic : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomBuffer = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);

    [Header("Split Screen Settings")]
    [SerializeField] private float swapDelay = 1.25f;
    [SerializeField] private float swapOffset = 5f;
    [SerializeField, Range(0, 1)] private float leftSplitPoint = 0.25f;
    [SerializeField, Range(0, 1)] private float rightSplitPoint = 0.75f;
    [SerializeField] private float minDistanceToTrigger = 0f;
    [SerializeField] private float maxAllowedDistance = 10f;

    private Camera cam;
    public Transform player1;
    public Transform player2;

    private float zoneTimer = 0f;
    private bool isTimerActive = false;
    private bool isPlayer1CrossingRight = false;
    private bool isPlayer2CrossingLeft = false;
    private Vector3 centerPosition;

    private void Start()
    {
        cam = GetComponent<Camera>();
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
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, smoothSpeed * Time.deltaTime);
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
        return distance > maxAllowedDistance;
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
        }
        else if (player2InLeftZone && !isTimerActive && distanceConditionMet)
        {
            isTimerActive = true;
            isPlayer1CrossingRight = false;
            isPlayer2CrossingLeft = true;
            zoneTimer = 0f;
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

    private void ResetPlayersToCenter()
    {
        Vector3 center = new Vector3(centerPosition.x, player1.position.y, player1.position.z);

        Vector3 player1Position = center;
        player1Position.x -= swapOffset;
        player1.position = player1Position;

        Vector3 player2Position = center;
        player2Position.x += swapOffset;
        player2.position = player2Position;

        isTimerActive = false;
        isPlayer1CrossingRight = false;
        isPlayer2CrossingLeft = false;
        zoneTimer = 0f;
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
            }
            else if (isPlayer2CrossingLeft)
            {
                Vector3 newPosition = player2.position;
                newPosition.x -= swapOffset;
                player1.position = newPosition;
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

    public void AssignPlayerDynamically(NetworkObject playerNetworkObject)
    {
        int ownerID = playerNetworkObject.OwnerId;
        if (ownerID == 0)
            player1 = playerNetworkObject.transform; 
        else if (ownerID == 1)
            player2 = playerNetworkObject.transform; 
    }

    public void AssignPlayers(NetworkObject p1, NetworkObject p2)
    {
        player1 = p1.transform;
        player2 = p2.transform;
    }
}
