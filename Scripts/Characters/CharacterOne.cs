using UnityEngine;

public class CharacterOne : CharacterBase
{
    protected override void Update()
    {
        if (!HasAuthority) return;
        base.Update();
    }
}
