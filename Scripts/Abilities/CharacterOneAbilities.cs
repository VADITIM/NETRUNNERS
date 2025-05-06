using UnityEngine;

public class CharacterOneAbilities : Abilities
{
    // [SerializeField] private GameObject abilityHitBox;

    private void Start()
    {
        // abilityHitBox.SetActive(false);
    }

    public override void AbilityOne()
    {
        Debug.Log("CharacterOne used Ability 1!");
        // abilityHitBox.SetActive(true);
        
    }

    public override void AbilityTwo()
    {
        // Debug.Log("CharacterOne used Ability 2!");
    }
}
