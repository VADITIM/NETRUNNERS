using UnityEngine;

public class AttackAnimationHandler : MonoBehaviour
{
    private CharacterBase characterBase;

    private void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
    }

    public void StartAttack()
    {
        if (characterBase != null)
        {
            characterBase.StartNormalAttack();
            characterBase.StartSpecialAttack();
            characterBase.StartAirNormalAttack();
            characterBase.StartAirSpecialAttack();
        }
    }

    public void EndAttack()
    {
        if (characterBase != null)
        {
            characterBase.EndNormalAttack();
            characterBase.EndSpecialAttack();
            characterBase.EndAirNormalAttack();
            characterBase.EndAirSpecialAttack();
        }
    }
}