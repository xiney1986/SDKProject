using UnityEngine;
using System.Collections;

public class ButtonMedalView : ButtonBase
{
	public UILabel label_medalName;
	public UISprite sprite_medalBg;


	public GameObject prefab_des;
	public GameObject root_des;

	void Start()
	{
		prefab_des.SetActive(false);
	}


	public void updateButton (LaddersMedalSample _sample)
	{
		label_medalName.text=_sample.name;
		if(_sample==null)
		{
			sprite_medalBg.spriteName="medal_0";
		}else
		{
			sprite_medalBg.spriteName="medal_"+Mathf.Min(_sample.index+1,5);
		}

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
			itemDes=NGUITools.AddChild (_parent, prefab_des);
			itemDes.SetActive(true);
			itemDes.GetComponent<UILabel>().text=_des[i];
		}
		_parent.GetComponent<UIGrid>().Reposition();
	} 
}
