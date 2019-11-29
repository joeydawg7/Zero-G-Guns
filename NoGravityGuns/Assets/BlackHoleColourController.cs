using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleColourController : MonoBehaviour
{
    public SpriteRenderer blackHoleSpriteOne, blackHoleSpriteTwo, blackHoleSpriteThree;
    
    public void SetBlackHoleColour(PlayerScript player)
    {
        blackHoleSpriteThree.color = player.playerColor;
        blackHoleSpriteTwo.color = player.playerColor + new Color(0.5f, 0.5f, 0.5f);
        blackHoleSpriteOne.color = player.playerColor - new Color(0.5f, 0.5f, 0.5f);
    }
    
}
