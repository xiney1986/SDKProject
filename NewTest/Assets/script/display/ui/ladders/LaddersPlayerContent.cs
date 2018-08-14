using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯玩家容器
/// </summary>
public class LaddersPlayerContent : MonoBehaviour
{
	private UIGrid grid_content;
	public GameObject player_Prefab;
	public LaddersWindow parentWindows;

	public const int count=9;
	public const int rowNum=3;

	private Ladders_PlayerItem[] playerComponents;

	void Start()
	{
		player_Prefab.SetActive(false);
	}
	/// <summary>
	/// 更新所有的——对手信息
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_updateAll(LaddersPlayerInfo[] _data)
	{
		if(_data==null||_data.Length!=count)
			return;
		if(playerComponents==null||playerComponents.Length<1)
		{
			M_creat();
		}
		for(int i=0;i<count;i++)
		{
			M_updatePlayer(_data[i]);
		}
	}
	/// <summary>
	/// 更新单个玩家信息
	/// </summary>
	/// <param name="_itemData">_item data.</param>
	public void M_updatePlayer(LaddersPlayerInfo _itemData)
	{
		int index=M_getPlayerIndex(_itemData.belongChestIndex,_itemData.index);
		playerComponents[index].M_update(_itemData);
	}
	/// <summary>
	/// 根据索引 返回单个玩家
	/// </summary>
	/// <returns>The player component.</returns>
	/// <param name="_index">_index.</param>
	public Ladders_PlayerItem M_getPlayerComponent(int _index)
	{
		return playerComponents[_index];
	}
	/// <summary>
	/// 清除
	/// </summary>
	public void M_clear()
	{
		if(playerComponents==null)
			return;
		for(int i=0,length=playerComponents.Length;i<length;i++)
		{
			MonoBehaviour.Destroy(playerComponents[i].gameObject);
		}
		playerComponents=null;
	}
	/// <summary>
	/// 根据 行数和列数 返回玩家所在的索引
	/// </summary>
	/// <returns>The player index.</returns>
	/// <param name="_row">_row.</param>
	/// <param name="_col">_col.</param>
	private int M_getPlayerIndex(int _row,int _col)
	{
		return (_row-1)*rowNum+_col-1;
	}
	/// <summary>
	/// 构建玩家 原型 
	/// </summary>
	private void M_creat()
	{
		grid_content=GetComponent<UIGrid>();
		playerComponents=new Ladders_PlayerItem[count];
		GameObject newPlayer;
		Ladders_PlayerItem playerC;
		for(int i=0;i<count;i++)
		{
			newPlayer=MonoBehaviour.Instantiate(player_Prefab) as GameObject;
			newPlayer.SetActive(true);
			newPlayer.name="player_"+i;
			UIUtils.M_addChild(transform,newPlayer);
			playerC=newPlayer.GetComponent<Ladders_PlayerItem>();
			playerC.index=i;
			playerC.fatherWindow=parentWindows;
			playerComponents[i]=playerC;
		}
		grid_content.Reposition();
	}
}

