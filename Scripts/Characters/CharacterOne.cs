using UnityEngine;

public class CharacterOne : CharacterBase
{
    protected override void Update()
    {
        base.Update();
        if (IsOwner && Input.GetKeyDown(KeyCode.Q))
        {
            if (abilities == null) return;
                abilities.AbilityOne();
        }
        
        if (IsOwner && Input.GetKeyDown(KeyCode.E))
        {
            if (abilities == null) return;
                abilities.AbilityTwo();
        }
    }
}