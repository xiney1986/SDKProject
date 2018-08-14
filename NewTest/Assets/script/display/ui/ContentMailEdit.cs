using UnityEngine;
using System.Collections;

/**
 * 邮件附件容器内容编辑
 * @author 汤琦
 * */
public class ContentMailEdit : ContentBase 
{
	private MailInfoWindow win;
	private Mail mail;
	private int mailTime = 0;
	
	public void initMail(Mail _mail,WindowBase win,int mailTime)
	{
		this.win = win as MailInfoWindow; 
		mail = _mail;
		this.mailTime = mailTime;
	}
	 
	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		base.CreateButton (index, page, buttonIndex);
		if (index == -1)
			return;
		PrizesModule button = page.transform.GetChild(buttonIndex).GetComponent<PrizesModule>();
		button.initPrize(getPrize(mail.annex[index]),windowBack,win);
		if(index == mail.annex.Length - 1)
		{
			if(index % 4 != 0||index == 0)
			{
				setHide(page,index%4+1);
			}
		}
	}
	
	private PrizeSample getPrize(Annex annex)
	{
		if(annex.exp != null)
			return annex.exp;
		else if(annex.money != null)
			return annex.money;
		else if(annex.prop != null)
			return annex.prop;
		else if(annex.pve != null)
			return annex.pve;
		else if(annex.pvp != null)
			return annex.pvp;
		else if(annex.rmb != null)
			return annex.rmb;
		else
			return null;
	}
	
	private void windowBack()
	{
		UiManager.Instance.openWindow<MailInfoWindow>((win)=>{
			win.init(mail,mailTime);
		});
	}
	
	private void setHide(GameObject page , int index)
	{
		for (int i = 0; i < page.transform.childCount - index; i++) {
			page.transform.GetChild(index + i).gameObject.SetActive(false);
		}
		
	}
	
	public override void updateActive (GameObject obj, int pageNUm)
	{
		updateContent (activeGameObj, pageNUm);
	}
}
