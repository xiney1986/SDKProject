using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentCardChoose : dynamicContent
{
	public GameObject roleViewPrefab;

	ArrayList RoleList;
	int showType = CardBookWindow.VIEW;
	bool isRoleChoose;
	List<Card> list;

	public void reLoad (ArrayList Roles)
	{
		list = Utils.ArrayList2List<Card> (Roles);
		showType = getShowType ();
		RoleList = Roles;
		isRoleChoose = fatherWindow is CardChooseWindow && (fatherWindow as CardChooseWindow).getShowType () == CardChooseWindow.ROLECHOOSE;
		base.reLoad (Roles.Count);
	}
		
	public void Initialize (ArrayList Roles)
	{

		showType = getShowType ();
		RoleList = Roles;
		isRoleChoose = fatherWindow is CardChooseWindow && (fatherWindow as CardChooseWindow).getShowType () == CardChooseWindow.ROLECHOOSE;
		if (Roles != null)
			base.reLoad (Roles.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//	base.updateItem (item, index);
		RoleView view = item.GetComponent<RoleView> ();
        Card c = RoleList[index] as Card;
        if (fatherWindow is CardSelectWindow) {
            view.init(c, fatherWindow, (rv) => {
                OnButtonClick(rv,index);
            });
            bool isShow = AttackBossOneOnOneManager.Instance.selectedCard.uid == c.uid ? true : false;
            view.tempGameObj.SetActive(isShow);
        } else {
            view.init(c, fatherWindow, (rv) => {
                OnItemClickEvent(index);
            });
        }
	}
    /// <summary>
    /// 更新箭头（只用于恶魔挑战卡片选择界面）
    /// </summary>
    public void updateArrow() {
        GameObject obj = (fatherWindow as CardSelectWindow).content.gameObject;
        (fatherWindow as CardSelectWindow).upArrow.gameObject.SetActive(false);
        (fatherWindow as CardSelectWindow).downArrow.gameObject.SetActive(false);
        if (RoleList.Count <= 6)
            return;
        if (RoleList.Count > 6) {
            (fatherWindow as CardSelectWindow).downArrow.gameObject.SetActive(true);
        }
        if (obj.transform.GetChild(0).position.y > 0.6f || obj.transform.GetChild(0).GetComponent<RoleView>().card.uid != (RoleList[0] as Card).uid) {
            (fatherWindow as CardSelectWindow).upArrow.gameObject.SetActive(true);
        }
        if (obj.transform.GetChild(obj.transform.childCount - 1).position.y > 0.1f
              && obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<RoleView>().card.uid == (RoleList[RoleList.Count - 1] as Card).uid) {
            (fatherWindow as CardSelectWindow).downArrow.gameObject.SetActive(false);
        }
    }
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, roleViewPrefab);
		    Card c = RoleList [i] as Card;

            if (fatherWindow is CardSelectWindow) {
                RoleView view = nodeList[i].GetComponent<RoleView>();
                view.showType = showType;
                view.hideInBattle = isRoleChoose;
                view.init(c, fatherWindow, (rv) => {
                    OnButtonClick(rv,i);
                });
                if (view.tempGameObj == null) {
                    UISprite us = NGUITools.AddChild<UISprite>(view.gameObject);
                    us.depth = 500;
                    us.atlas = view.qualityBg.atlas;
                    us.spriteName = "gou_3";
                    us.MakePixelPerfect();
                    us.transform.localScale = new Vector3(2, 2, 1);
                    us.gameObject.SetActive(false);
                    view.tempGameObj = us.gameObject;
                    if (AttackBossOneOnOneManager.Instance.selectedCard != null && c.uid == AttackBossOneOnOneManager.Instance.selectedCard.uid) {
                        us.gameObject.SetActive(true);
                        (fatherWindow as CardSelectWindow).selectCard = c;
                        AttackBossOneOnOneManager.Instance.selectedCard = c;
                    } else if (AttackBossOneOnOneManager.Instance.selectedCard == null && i == 0) {
                        us.gameObject.SetActive(true);
                        (fatherWindow as CardSelectWindow).selectCard = c;
                        AttackBossOneOnOneManager.Instance.selectedCard = c;
                    }
                }
            } else {
                RoleView view = nodeList[i].GetComponent<RoleView>();
                view.showType = showType;
                view.hideInBattle = isRoleChoose;
                view.init(c, fatherWindow, (rv) => {
                    OnItemClickEvent(i);
                });
            }
		}
	}

	private int getShowType ()
	{
		if (fatherWindow.GetType () == typeof(CardStoreWindow)) {
			return CardBookWindow.VIEW;
        } else if (fatherWindow.GetType() == typeof(CardSelectWindow)) {
            return CardBookWindow.CHATSHOW;
        } else {
            int type = (fatherWindow as CardChooseWindow).getShowType();
            if (type == CardChooseWindow.ROLECHOOSE)
                return CardBookWindow.INTOTEAM;
            else if (type == CardChooseWindow.CHATSHOW)
                return CardBookWindow.CHATSHOW;
            else if (type == CardChooseWindow.CARDCHANGE)
                return CardBookWindow.CARDCHANGE;
            else if (type == CardChooseWindow.CARDTRAINING)
                return CardBookWindow.CARDTRAINING;
            else if (type == CardChooseWindow.MINING) {
                return CardBookWindow.INTOTEAM;
            }
            return CardBookWindow.VIEW;
        }
	}
    public bool isSelected() {
        if (AttackBossOneOnOneManager.Instance.selectedCard != null)
            return true;
        return false;
    }
    public void clearAllSelect() {
        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).GetComponent<RoleView>().tempGameObj.SetActive(false);
        }
        (fatherWindow as CardSelectWindow).selectCard = null;
        AttackBossOneOnOneManager.Instance.selectedCard = null;
    }
    void OnButtonClick(RoleView view,int index) {
        CardSelectWindow win = fatherWindow as CardSelectWindow;
        if (isSelected()) {
            if (!view.tempGameObj.activeSelf) {
                clearAllSelect();
                view.tempGameObj.SetActive(true);
                win.selectCard = view.card;
                AttackBossOneOnOneManager.Instance.selectedCard = view.card;
            }
        } else {
            clearAllSelect();
            view.tempGameObj.SetActive(true);
            win.selectCard = view.card;
            AttackBossOneOnOneManager.Instance.selectedCard = view.card;
        }
        MaskWindow.UnlockUI();
    }

	void OnItemClickEvent(int index)
	{
        if (showType == CardBookWindow.CARDTRAINING)
        {
            MaskWindow.LockUI();
            fatherWindow.finishWindow();
            UiManager.Instance.getWindow<CardTrainingWindow>().setCardData(list[index]);
        }
        else
        {
            if (GuideManager.Instance.isEqualStep(135009000))
            {
                GuideManager.Instance.doGuide();
                GuideManager.Instance.guideEvent();
            }
            MaskWindow.LockUI();
            ChatManagerment.Instance.chatCard = null;
            CardBookWindow.Show(list, index, showType, null);
        }

	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(8004000)||GuideManager.Instance.isEqualStep(8007000)||GuideManager.Instance.isEqualStep(107001000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
}

