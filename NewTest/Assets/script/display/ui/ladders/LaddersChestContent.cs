using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯中 宝箱容器
/// </summary>
public class LaddersChestContent : MonoBase
{
	private UIGrid grid_content;
	public GameObject box_Prefab;
	public LaddersWindow parentWindows;

	public int count=3;
	private Ladders_BoxItem[] boxComponents;

	void Start()
	{
		box_Prefab.SetActive(false);
	}
	/// <summary>
	/// 更新所有的宝箱
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_updateAll(LaddersChestInfo[] _data)
	{
		if(_data==null||_data.Length!=count)
			return;
		if(boxComponents==null||boxComponents.Length<1)
		{
			M_creat();
		}
		for(int i=0;i<count;i++)
		{
			boxComponents[i].M_update(_data[i]);
		}
	}
	/// <summary>
	/// 根据索引 更新对应的宝箱数据
	/// </summary>
	/// <param name="_index">_index.</param>
	/// <param name="_itemData">_item data.</param>
	public void M_udpateByIndex(int _index,LaddersChestInfo _itemData)
	{
		if(boxComponents==null||boxComponents.Length<1)
		{
			M_creat();
		}
		boxComponents[_index].M_update(_itemData);
	}
	/// <summary>
	/// 创建宝箱原型
	/// </summary>
	public void M_creat()
	{
		grid_content=GetComponent<UIGrid>();
		boxComponents=new Ladders_BoxItem[count];
		GameObject newBox;
		Ladders_BoxItem boxC;
		for(int i=0;i<count;i++)
		{
			newBox=MonoBehaviour.Instantiate(box_Prefab) as GameObject;
			newBox.SetActive(true);
			newBox.name="chest_"+i;
			UIUtils.M_addChild(transform,newBox);
			boxC=newBox.GetComponent<Ladders_BoxItem>();
			boxC.index=i;
			boxC.fatherWindow=parentWindows;
			boxComponents[i]=boxC;
		}
		grid_content.Reposition();
	}
	/// <summary>
	/// 清除宝箱
	/// </summary>
	public void M_clear()
	{
		if(boxComponents==null)
			return;
		for(int i=0,length=boxComponents.Length;i<length;i++)
		{
			MonoBehaviour.Destroy(boxComponents[i].gameObject);
		}
		boxComponents=null;
	}
	/// <summary>
	/// 根据索引 返回对应的宝箱
	/// </summary>
	/// <returns>The box component.</returns>
	/// <param name="_index">_index.</param>
	public Ladders_BoxItem M_getBoxComponent(int _index)
	{
		return boxComponents[_index];
	}
}

