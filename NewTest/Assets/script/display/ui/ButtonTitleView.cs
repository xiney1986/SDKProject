using UnityEngine;
using System.Collections;

public class ButtonTitleView : ButtonBase
{
	public UILabel label_titleName;

	public GameObject prefab_des;
	public GameObject root_des;
	
	void Start()
	{
		prefab_des.SetActive(false);
	}
	
	
	public void updateButton (LaddersTitleSample _sample)
	{
		label_titleName.text=_sample.name;
		updateDes(root_des,_sample.addDescriptions);
	}
	
	private void updateDes(GameObject _parent,string[] _des)
	{
		UIUtils.M_removeAllChildren(_parent);
		if(_des==null)
		{
			return;
		}
		GameObject itemDes;
		for(int i=0,length=_des.Length;i<length;i++)
		{
			if(i>5)
			{
				break;
			}
			itemDes=NGUITools.AddChild (_parent, prefab_des);
			itemDes.SetActive(true);
			itemDes.GetComponent<UILabel>().text="[87373E]"+_des[i]+"[-]";
		}
		_parent.GetComponent<UIGrid>().Reposition();
	} 
 
}
