using UnityEngine;

public class DirectionalArrow : MonoBehaviour
{
    [SerializeField] private CameraLogic cameraLogic;
    [SerializeField] private GameObject leftPlayerArrow;
    [SerializeField] private GameObject rightPlayerArrow;

    private float wiggleSpeed = 5f; 
    private float wiggleAmount = 50.5f; 

    private Vector3 leftArrowOriginalPosition;
    private Vector3 rightArrowOriginalPosition;

    private void Start()
    {
        leftPlayerArrow.SetActive(false);
        rightPlayerArrow.SetActive(false);

        leftArrowOriginalPosition = leftPlayerArrow.transform.localPosition;
        rightArrowOriginalPosition = rightPlayerArrow.transform.localPosition;
    }

    private void Update()
    {
        HandleArrowActivation();
        ApplyWiggleEffect();
    }

    private void HandleArrowActivation()
    {
        if (cameraLogic == null) return;

        bool isPlayer1Assigned = cameraLogic.player1 != null;
        bool isPlayer2Assigned = cameraLogic.player2 != null;

        leftPlayerArrow.SetActive(isPlayer1Assigned && !isPlayer2Assigned);
        rightPlayerArrow.SetActive(isPlayer2Assigned && !isPlayer1Assigned);

        if (!isPlayer1Assigned && !isPlayer2Assigned)
        {
            leftPlayerArrow.SetActive(false);
            rightPlayerArrow.SetActive(false);
        }
    }

    private void ApplyWiggleEffect()
    {
        if (leftPlayerArrow.activeSelf)
        {
            leftPlayerArrow.transform.localPosition = leftArrowOriginalPosition + new Vector3(Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount, 0, 0);
        }

        if (rightPlayerArrow.activeSelf)
        {
            rightPlayerArrow.transform.localPosition = rightArrowOriginalPosition + new Vector3(Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount, 0, 0);
        }
    }
}