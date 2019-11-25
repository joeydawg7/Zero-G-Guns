using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunBullet : Bullet
{

    public override void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color)
    {
        base.Construct(damage, player, dir, color);

        SetPFXTrail("MinigunTrail", true);
    }
}
