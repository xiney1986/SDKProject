using UnityEngine;
using System.Collections;

public class ButtonSpriteAdd : ButtonBase
{
	public UISprite  spriteBack;
	public UITexture spriteType;
	public UILabel   countLabel;
	public UILabel   spriteName;
	private int count;
	public ArrayList spriteList;
	public int spriteSid;
	public bool isInit = false;

	public void initUI (string iconId, int spriteSid)
	{
		isInit = true;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + iconId, spriteType);
		spriteList = StorageManagerment.Instance.getRoleBySid (spriteSid);
		this.count = spriteList.Count;
		this.spriteSid = spriteSid;
		countLabel.text = "x" + count;
	}

	public void updateCount ()
	{
		countLabel.text = "x" + count;
	}

	public int getCount ()
	{
		return count;
	}

	public void setCount(int count)
	{
		this.count = count;
	}
}
