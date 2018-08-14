using UnityEngine;
using System.Collections;

/**
 * 角色选择内容编辑
 * @author 汤琦
 * */
public class ContentRoleEdit : ContentBase
{
	public Card[] cards;
	public UISprite leftShow;
	public UISprite rightShow;
	
	public void initContent (Card[] cards)
	{
		this.cards = cards;
	} 
	
	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		base.CreateButton (index, page, buttonIndex);
		if (index == -1)
			return;
		ButtonRoleSelect button = page.GetComponent<ButtonRoleSelect> ();
		setCreatButton (button, cards[index]);
	}
	
	public override void updateActive (GameObject obj, int pageNUm)
	{
		updateContent (activeGameObj, pageNUm);
		(fatherWindow as RoleNameWindow).updateInfo(cards[pageNUm - 1]);
		if(pageNUm == 1)
		{
			leftShow.gameObject.SetActive(false);
			rightShow.gameObject.SetActive(true);
		}
		else if(pageNUm == cards.Length)
		{
			leftShow.gameObject.SetActive(true);
			rightShow.gameObject.SetActive(false);
		}
		else 
		{
			leftShow.gameObject.SetActive(true);
			rightShow.gameObject.SetActive(true);
		}
	}
	
	//设置创建按钮信息
	private void setCreatButton (ButtonRoleSelect button, Card card)
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), button.roleImage);
		button.roleImage.gameObject.SetActive(true);
	}
}
