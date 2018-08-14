using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 卡片星魂穿戴条目
/// </summary>
public class ContentShowItem : SampleDynamicContent {


	public GameObject starSoulinfoLabel;
	private List<Card> data;
	private int teamNum;
	private int index;
	public SoulEquiShow activeShowItem;
	public SoulEquiInfoShow soulEquiInfo;
	private Card activeCard;
	private  int changedFlag=0;//是否可以点按钮
	/** 父容器*/
	StarSoulEquipContent fatherContent;

	public void init(StarSoulEquipContent fatherContent,List<Card> cardList,int index,int changedFlag) {
		this.fatherContent=fatherContent;
		teamNum=index;
		data=cardList;
		maxCount=data.Count;
		this.changedFlag=changedFlag;
	}
	public void updateButton (GameObject page) {
		int index=StringKit.toInt(page.name)-1;
		this.index=index;
		SoulEquiShow button = page.transform.GetComponent<SoulEquiShow>();
		button.initInfo(data[index],fatherWindow);
	}
	/// <summary>
	/// 得到队伍中实际的卡片
	/// </summary>
	private List<Card> updataCard(Army ar) {
		string[] players=ar.players;
		List<Card> list =new List<Card>();
		for(int i=0;i<players.Length;i++) {
			Card c=StorageManagerment.Instance.getRole(players[i]);
			if(c!=null)list.Add(c);
		}
		return list;
	}
	/// <summary>
	/// 节点滑动到中间以后触发的事件.
	/// </summary>
	public  void updateActive (GameObject obj) {
		activeShowItem=obj.GetComponent<SoulEquiShow>();
		activeCard=activeShowItem.currectCard;
        StarSoulManager.Instance.setActiveInfo(activeCard,-1);
		updateStarSoulInfo(activeCard);
		soulEquiInfo.initInfo(data[StringKit.toInt(obj.name)-1],fatherWindow,teamNum,changedFlag);
        if (changedFlag == CardBookWindow.VIEW || changedFlag == 0) fatherContent.contentTop.initTopButton(StringKit.toInt(obj.name) - 1);

    }
	/// <summary>
	/// 更新卡片上星魂的属性条目
	/// </summary>
	public void updateStarSoulInfo(Card card) {
		UILabel[] labe=starSoulinfoLabel.GetComponentsInChildren<UILabel>();
		StarSoul[] starSouls=activeCard.getStarSoulByAll();
		for(int i=0;i<labe.Length;i++) {
			labe[i].text="";
		}
		if(starSouls!=null) 
        {
            List<AttrChangeSample> attribute = new List<AttrChangeSample>();
            List<string> attributeDesc = new List<string>();
            for (int j = 0; j < starSouls.Length; j++) {
                AttrChangeSample[] str3 = starSouls[j].getStarSoulSample().getAttrChangeSample();//获取星魂所有属性
                string[] str4 = StarSoulManager.Instance.getStarSoulDese(starSouls[j]).Split('#');//获取星魂所有属性描述
                for (int i = 0; i < str3.Length; i++) {
                    attribute.Add(str3[i]);
                    attributeDesc.Add(str4[i]);
                }
            }
            for (int i = 0; i < attribute.Count; i++) {//把所有相同的属性整合到一起
                for (int k = i + 1; k < attribute.Count; k++) {
                    if (attribute[i].getAttrType() == attribute[k].getAttrType()) {
                        attributeDesc[i] = attributeDesc[i].Split('+')[0]+"+"+(StringKit.toInt(attributeDesc[i].Split('+')[1]) + StringKit.toInt(attributeDesc[k].Split('+')[1])).ToString();
                        attributeDesc.RemoveAt(k);
                        attribute.RemoveAt(k);
                        k--;
                    }
                }
            }
            for (int i = 0; i < attribute.Count; i++) {//显示
                UILabel labeDesc = starSoulinfoLabel.transform.FindChild("num" + (i+1).ToString()).gameObject.GetComponent<UILabel>();
                labeDesc.text = "[A65644]" + attributeDesc[i].Split('+')[0] + "[3A9663]+" + attributeDesc[i].Split('+')[1];
            }
		}
	}
	/// <summary>
	/// 更新单个的卡片信息
	/// </summary>
	public void updateUI()
	{
		if(StarSoulManager.Instance.getActiveCard()!=null)
			activeCard  = StarSoulManager.Instance.getActiveCard();
		activeShowItem.initInfo(activeCard,fatherWindow);
		soulEquiInfo.initInfo(activeCard,fatherWindow,teamNum,changedFlag);
		updateStarSoulInfo(activeCard);
	}
}