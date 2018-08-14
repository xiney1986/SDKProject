using UnityEngine;
using System.Collections;

public class ButtonTempProp : ButtonBase
{
	public GameObject goodsPrefab;//奖励预制体
	public GameObject goodsView;//对象
	public TempProp temp;//关联的tempProp对象
	public UILabel propName;//道具名字
	public UILabel propNums;//道具数量
	public UILabel surplusTime;//剩余时间
	public MailWindow win;
	public ButtonTempResult buttonExtract;//提取按钮
	public ButtonTempResult buttonDelete;//删除按钮
	private Timer timer;//计时器
	
	public override void begin ()
	{
		base.begin (); 
	}

	public override void OnDrop (GameObject drag)
	{
		base.OnDrop (drag);
	}

	public  void initialize (TempProp _prop)
	{
		win = fatherWindow as MailWindow;
		updateButton (_prop);
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (surplusTimeShow);
		timer.start ();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		switch (temp.type) {
		case TempPropType.BEAST:
			UiManager.Instance.openWindow <BeastAttrWindow>((win1)=>{
				win1.Initialize (CardManagerment.Instance.createCard (temp.sid), BeastAttrWindow.TEMPSTORE);
			});
			break;
		case TempPropType.CARD:
			CardBookWindow.Show(CardManagerment.Instance.createCard (temp.sid),CardBookWindow.OTHER,null);
			break;
		case TempPropType.EQUIPMENT:
			UiManager.Instance.openWindow <EquipAttrWindow>((win1)=>{
				win1.Initialize (EquipManagerment.Instance.createEquip (temp.sid), EquipAttrWindow.OTHER, null);
			});
			break;
		case TempPropType.GOODS:
			UiManager.Instance.openDialogWindow<PropAttrWindow>((winc)=>{
				winc.Initialize (PropManagerment.Instance.createProp (temp.sid));
			});
			break;
		}
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null)
			timer.stop ();
		timer = null;
	}

	public void updateButton (TempProp newTemp)
	{
		if (newTemp == null) {
			return;
		} else {
			temp = newTemp;
			buttonExtract.fatherWindow = fatherWindow;
			buttonExtract.UpdateTemp (temp);
			buttonDelete.fatherWindow = fatherWindow;
			buttonDelete.UpdateTemp (temp);
			createGoodsView (temp.type,temp.getNum());
			propName.text = temp.getName ();
			surplusTimeShow ();
		}
	}


	public void surplusTimeShow ()
	{
		surplusTime.text = timeTransform (temp.time);
	}
	
	//转换时间格式
	private string timeTransform (double time)
	{
		time = time - ServerTimeKit.getSecondTime();
		string str = string.Empty;
		if (time > 0) {
			int hours = (int)(time / 3600);
			int minutes = (int)(time % 3600 / 60);
			int seconds = (int)(time % 3600 % 60);
			if (hours >= 24) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", (hours / 24).ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0018");
			} else if (hours < 24 && hours >= 1) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", hours.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0019");
			} else if (hours < 1 && minutes >= 1) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", minutes.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0020");
			} else {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", seconds.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0021");
			}
		} else {
			if(timer !=null){
				timer.stop();
				timer = null;
			}
			str = LanguageConfigManager.Instance.getLanguage ("s0138");
			buttonExtract.disableButton(true);
			buttonDelete.disableButton(true);
		}
		return  str;
	}
	private void createGoodsView(string type,int num) {
		if (goodsView.transform.childCount>0)
			Utils.RemoveAllChild (goodsView.transform);
		GameObject goodsObj= NGUITools.AddChild (goodsView.gameObject,goodsPrefab);
		GoodsView button = goodsObj.GetComponent<GoodsView> ();
		button.fatherWindow = win;
		if (type == TempPropType.BEAST || type == TempPropType.CARD) {
			button.init(CardManagerment.Instance.createCard (temp.sid),num);
		} else if(type == TempPropType.STARSOUL) {
			button.init(StarSoulManager.Instance.createStarSoul (temp.sid),num);
        } else if (type == TempPropType.MAGICWEAPON)
        {
            button.init(new MagicWeapon(temp.uid,temp.sid,0,0,0),num);
        }
        else {
			button.init(new Equip(temp.uid,temp.sid,0,0,0,0),num);
		}
	}
}
