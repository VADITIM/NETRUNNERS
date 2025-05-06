using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerKilledUI : MonoBehaviour
{
    [SerializeField] private CameraLogic cameraLogic;
    [SerializeField] private TextMeshProUGUI playerKilledText;
    [SerializeField] private GameObject panel;
    
    private float moveDuration = 0.1f;
    private float shootInterval = 0.05f;
    private float verticalOffset = 300f;
    private float pauseBeforeReset = 2f;
    private float resetDuration = 0.4f;
    
    private bool isAnimating = false;
    private bool hasShownAnimation = false;
    private bool bothPlayersWereAssigned = false;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        HandlePlayerKillUI();
    }

    private void HandlePlayerKillUI()
    {
        if (cameraLogic == null) return;
        if (isAnimating) return;

        bool isPlayer1Assigned = cameraLogic.player1 != null;
        bool isPlayer2Assigned = cameraLogic.player2 != null;
        
        if (isPlayer1Assigned && isPlayer2Assigned)
        {
            bothPlayersWereAssigned = true;
            hasShownAnimation = false;
            panel.SetActive(false);
        }

        if (bothPlayersWereAssigned && (!isPlayer1Assigned || !isPlayer2Assigned) && !hasShownAnimation)
        {
            panel.SetActive(true);
            
            if (!isPlayer1Assigned)
            {
                SetPlayerKilledText("PLAYER LEFT ELIMINATED");
            }
            else if (!isPlayer2Assigned)
            {
                SetPlayerKilledText("PLAYER RIGHT ELIMINATED");
            }
            
            hasShownAnimation = true;
        }
        else if (!isPlayer1Assigned && !isPlayer2Assigned)
        {
            panel.SetActive(false);
            bothPlayersWereAssigned = false;
            hasShownAnimation = false;
        }
    }

    private void SetPlayerKilledText(string text)
    {
        playerKilledText.text = text;
        AnimateCharacters();
    }

    private void AnimateCharacters()
    {
        if (isAnimating) return;
        
        isAnimating = true;
        
        playerKilledText.DOKill();
        
        playerKilledText.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = playerKilledText.textInfo;
        
        Vector3[][] originalVertexPositions = new Vector3[textInfo.characterCount][];
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            
            if (!charInfo.isVisible) continue;
            
            int materialIndex = charInfo.materialReferenceIndex;
            
            int vertexIndex = charInfo.vertexIndex;
            
            Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
            
            originalVertexPositions[i] = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                originalVertexPositions[i][j] = sourceVertices[vertexIndex + j];
            }
            
            for (int j = 0; j < 4; j++)
            {
                sourceVertices[vertexIndex + j] += new Vector3(0, verticalOffset, 0);
            }
        }
        
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            playerKilledText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
        
        float totalAnimTime = (textInfo.characterCount - 1) * shootInterval + moveDuration;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            
            int charIndex = i;
            
            DOVirtual.DelayedCall(charIndex * shootInterval, () => {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                
                Sequence charSequence = DOTween.Sequence();
                
                for (int j = 0; j < 4; j++)
                {
                    int vertexID = vertexIndex + j;
                    Vector3 targetPosition = originalVertexPositions[charIndex][j];
                    
                    charSequence.Join(DOTween.To(() => textInfo.meshInfo[materialIndex].vertices[vertexID],
                        newPos => {
                            textInfo.meshInfo[materialIndex].vertices[vertexID] = newPos;
                            playerKilledText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                        },
                        targetPosition, moveDuration).SetEase(Ease.OutBounce));
                }
            });
        }
        
        DOVirtual.DelayedCall(totalAnimTime + pauseBeforeReset, () => {
            Sequence resetSequence = DOTween.Sequence();
            
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                
                if (!charInfo.isVisible) continue;
                
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                
                for (int j = 0; j < 4; j++)
                {
                    int vertexID = vertexIndex + j;
                    Vector3 currentPos = textInfo.meshInfo[materialIndex].vertices[vertexID];
                    Vector3 targetPosition = currentPos + new Vector3(0, verticalOffset, 0);
                    
                    resetSequence.Join(DOTween.To(() => textInfo.meshInfo[materialIndex].vertices[vertexID],
                        newPos => {
                            textInfo.meshInfo[materialIndex].vertices[vertexID] = newPos;
                            playerKilledText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                        },
                        targetPosition, resetDuration).SetEase(Ease.InBack));
                }
            }
            
            resetSequence.OnComplete(() => {
                isAnimating = false;
            });
        });
    }
    
    public void ResetState()
    {
        playerKilledText.DOKill();
        isAnimating = false;
        hasShownAnimation = false;
        bothPlayersWereAssigned = false;
    }
    
    private void OnDisable()
    {
        playerKilledText.DOKill();
        isAnimating = false;
    }
}