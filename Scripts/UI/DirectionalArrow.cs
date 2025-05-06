using UnityEngine;
using DG.Tweening;

public class DirectionalArrow : MonoBehaviour
{
    [SerializeField] private CameraLogic cameraLogic;
    [SerializeField] private GameObject leftPlayerArrow;
    [SerializeField] private GameObject rightPlayerArrow;
    
    private float moveDuration = 1f;
    private float stayDuration = 1.7f;
    private float exitDuration = 0.6f;

    private Vector3 leftArrowOriginalPosition;
    private Vector3 rightArrowOriginalPosition;
    
    private bool isAnimating = false;
    private bool hasAnimatedLeft = false;
    private bool hasAnimatedRight = false;
    
    private bool wasPlayer1EverAssigned = false;
    private bool wasPlayer2EverAssigned = false;
    private bool bothPlayersEverAssigned = false;

    private void Start()
    {
        leftPlayerArrow.SetActive(false);
        rightPlayerArrow.SetActive(false);

        leftArrowOriginalPosition = leftPlayerArrow.transform.localPosition;
        rightArrowOriginalPosition = rightPlayerArrow.transform.localPosition;
    }

    private void Update()
    {
        if (cameraLogic != null)
        {
            if (cameraLogic.player1 != null)
            {
                wasPlayer1EverAssigned = true;
            }
            
            if (cameraLogic.player2 != null)
            {
                wasPlayer2EverAssigned = true;
            }
            
            if (wasPlayer1EverAssigned && wasPlayer2EverAssigned)
            {
                bothPlayersEverAssigned = true;
            }
        }
        
        HandleArrowActivation();
    }

    private void HandleArrowActivation()
    {
        if (cameraLogic == null || isAnimating) return;
        
        if (!bothPlayersEverAssigned) return;

        bool isPlayer1Assigned = cameraLogic.player1 != null;
        bool isPlayer2Assigned = cameraLogic.player2 != null;

        if (isPlayer1Assigned && !isPlayer2Assigned && !hasAnimatedLeft)
        {
            leftPlayerArrow.SetActive(true);
            AnimateArrow(leftPlayerArrow, leftArrowOriginalPosition, -1500f, 1300f);
            hasAnimatedLeft = true;
            hasAnimatedRight = false;
        }
        else if (isPlayer2Assigned && !isPlayer1Assigned && !hasAnimatedRight)
        {
            rightPlayerArrow.SetActive(true);
            AnimateArrow(rightPlayerArrow, rightArrowOriginalPosition, 1500f, -1300f);
            hasAnimatedRight = true;
            hasAnimatedLeft = false;
        }
        else if (!isPlayer1Assigned && !isPlayer2Assigned)
        {
            leftPlayerArrow.SetActive(false);
            rightPlayerArrow.SetActive(false);
            hasAnimatedLeft = false;
            hasAnimatedRight = false;
        }
    }

    private void AnimateArrow(GameObject arrow, Vector3 originalPosition, float startOffset, float endOffset)
    {
        isAnimating = true;
        
        arrow.transform.DOKill();
        
        Vector3 startPosition = originalPosition + new Vector3(startOffset, 0, 0);
        Vector3 endPosition = originalPosition + new Vector3(endOffset, 0, 0);
        
        arrow.transform.localPosition = startPosition;
        
        Sequence arrowSequence = DOTween.Sequence();
        
        arrowSequence.Append(arrow.transform.DOLocalMove(originalPosition, moveDuration).SetEase(Ease.OutBack));
        arrowSequence.AppendInterval(stayDuration);
        arrowSequence.Append(arrow.transform.DOLocalMove(endPosition, exitDuration).SetEase(Ease.InBack));
        
        arrowSequence.OnComplete(() => {
            arrow.transform.localPosition = startPosition;
            isAnimating = false;
            arrow.SetActive(false);
            
            if (arrow == leftPlayerArrow) {
                hasAnimatedLeft = false;
            } else if (arrow == rightPlayerArrow) {
                hasAnimatedRight = false;
            }
        });
    }
    
    public void ResetState()
    {
        leftPlayerArrow.transform.DOKill();
        rightPlayerArrow.transform.DOKill();
        
        leftPlayerArrow.SetActive(false);
        rightPlayerArrow.SetActive(false);
        
        leftPlayerArrow.transform.localPosition = leftArrowOriginalPosition;
        rightPlayerArrow.transform.localPosition = rightArrowOriginalPosition;
        
        isAnimating = false;
        hasAnimatedLeft = false;
        hasAnimatedRight = false;
    }
    
    private void OnDisable()
    {
        leftPlayerArrow.transform.DOKill();
        rightPlayerArrow.transform.DOKill();
    }
}