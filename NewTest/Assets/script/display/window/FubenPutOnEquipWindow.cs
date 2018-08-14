using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FubenPutOnEquipWindow : WindowBase {

	private List<Equip> equipList;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void initWin(List<Equip> _equipList)
	{
		equipList = _equipList;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "buttonOk")
		{
			UiManager.Instance.applyMask();
			toPutOnEquip();
		}
	}

	public void toPutOnEquip()
	{
		if(IncreaseManagerment.Instance.isCanBePutOnEquip(equipList))
        {
			for (int i = 0; i < equipList.Count; i++) {
				Card chooseCard = IncreaseManagerment.Instance.getCanPutOnEquipByTeamCards(equipList[i]);
				if(chooseCard != null) {
					EquipOperateFPort eof = FPortManager.Instance.getFPort ("EquipOperateFPort") as EquipOperateFPort;
					eof.access (chooseCard.uid, chooseCard.sid, equipList[i].uid, equipList[i].getPartId (), equipResult);
					equipList.Remove(equipList[i]);
				}
			}
		}
		else
        {
			//UiManager.Instance.cancelMask();
		//	(fatherWindow as FubenAwardWindow).destoryWindow();

			if(MissionManager.instance != null)
				MissionManager.instance.missionEnd ();
			else
				ScreenManager.Instance.loadScreen (1, null, GameManager.Instance.outMission);	
		}
	}

	//穿装备和脱装备成功后的回调
	private void equipResult (List<AttrChange> attrs, int types)
	{ 
		toPutOnEquip();
	}
}
