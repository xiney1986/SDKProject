using UnityEngine;
using System.Collections;

public class BattleHpInfo
{
    public string uid;
    public int hp;
    public int maxhp;

    public float getHP()
    {
        if (maxhp == 0)
            return 0;
        return (float)hp / maxhp;
    }
}
