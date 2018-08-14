using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaReplayInfo
{
    public ArenaReplayInfoUser user1;
    public ArenaReplayInfoUser user2;
    public List<string> winUids;
}

public class ArenaReplayInfoUser
{
    public int score;
    public string uid;
    public string name;
    public int style;
    public bool win;
}