using UnityEngine;
using System.Collections;

public class ArenaFinalInfo
{
    public const int STATE_REPLAY = 1; //回看
    public const int STATE_GUESS = 2; //猜
    public const int STATE_WAIT = 3; //等待时间

    public string uid;
    public int team;

    public string userName;
    public int userIcon;
    public string userId;
    public bool lose;

    public int startTime;
    public int index; //场次
    public int state; //状态,replay,guess,wait
    public int finalState;
    
    public int guessStartTime;
    public int guessEndTime;
    public bool guessed; //

    public int getType()
    {
        return 0;
    }

    public bool hasUser()
    {
		return !string.IsNullOrEmpty(uid)||!string.IsNullOrEmpty(userId);
    }
	//是否是胜者，区分失败者和胜利者，失败者只有id没有其他信息
	public bool isWinner(){
		return !string.IsNullOrEmpty (userName);
	}
}
