using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatTipsWindow : WindowBase
{
    public CombatTipsItem UI_ItemTemplate;
    public UIGrid UI_ItemContainer;
	


    protected override void begin()
    {
        base.begin();
		if (!isAwakeformHide) {
			updateData();
		}
        MaskWindow.UnlockUI();
    }

	protected override void DoEnable () {
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}

    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        switch (gameObj.name)
        {
            case "close":
                finishWindow();
                break;
        }
    }

    private void updateData()
    {
        int lv = UserManager.Instance.self.getUserLevel();
        ArrayList list = CombatTipsSampleManager.Instance.GetAllSample();
        int offset = 0;
        for (int i = 0; i < list.Count; i++)
        {
            CombatTipsSample sample = CombatTipsSampleManager.Instance.getDataBySid((int)list[i]);
            //if ( lv > sample.maxLv || ((sample.funshow < 100 && lv < sample.funshow) || (sample.funshow > 100 && !GuideManager.Instance.isMoreThanStep(sample.funshow))) )
            if (lv > sample.maxLv || lv < sample.funshow)
            {
                offset++;
                continue;
            }
            int index = i - offset;
            CombatTipsItem combatTipsItem = GameObject.Instantiate(UI_ItemTemplate) as CombatTipsItem;
            combatTipsItem.setData(sample, index < 3);
			combatTipsItem.setFawin (this);
            Transform t = combatTipsItem.transform;
            t.parent = UI_ItemContainer.transform;
            t.localPosition = new Vector3(-UI_ItemContainer.cellWidth * 2, -index * UI_ItemContainer.cellHeight, 0);
			t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
			UI_ItemContainer.Reposition();
//            iTween.MoveTo(t.gameObject, iTween.Hash("isLocal", true, "position", new Vector3(0, -index * UI_ItemContainer.cellHeight, 0), "time", 1f));
//            yield return new WaitForSeconds(0.1f);
        }
    }
}

