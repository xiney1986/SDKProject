using System;
/// <summary>
/// 天梯——玩家数存储
/// </summary>
public class LaddersPlayers
{
		 
	private LaddersPlayerInfo[] players;

	public LaddersPlayers (int _count)
	{
		players=new LaddersPlayerInfo[_count];
	}
	public void M_updatePlayer(LaddersPlayerInfo[] _players)
	{
		for(int i=0,length=_players.Length;i<length;i++)
		{
			players[i]=_players[i];
		}
	}	
	public void M_updatePlayer(int _index,LaddersPlayerInfo _player)
	{
		players[_index]=_player;
	}

	public LaddersPlayerInfo[] M_getPlayers()
	{
		return players;
	}
	public LaddersPlayerInfo M_getPlayer(int _index)
	{
		return players[_index];
	}
	public void M_clear()
	{
		for(int i=0,length=players.Length;i<length;i++)
		{
			players[i]=null;
		}
	}
}


