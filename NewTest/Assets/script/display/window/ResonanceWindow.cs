using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResonanceWindow : WindowBase
{
	public TapContentBase tapBase;//顶级按钮
    public UILabel titleInfoLabel;//点击可以直接强化等等说明
    public Equiqianghuacontent equiqianghuacontent;//装备强化容器
    public GameObject zhuangbenqianghGameObject;
    public Equipjingliancontent equipjingliancontent;//装备精炼容器
    public GameObject zhuangbeijinglianGameObject;
    public StarSoulGMContent cardstarsoulcontent;//星魂容器
    public GameObject cardstarsoulGameObject;
    private int startIndex;//默认进入窗口选择的标签
    private string cardUID;//传入的卡片uid
    private int showTypeNum;//你从哪里来
    private ServerCardMsg showCard;//别人的卡
    private string chatPlayerUid;//聊天界面角色UID

	public override void OnAwake ()
	{
	}

    public void init(string  cardUid,int showType)
    {
        cardUID = cardUid;
        showTypeNum = showType;
    }
    public void initServer(ServerCardMsg card, int showType,string _uid)
    {
        chatPlayerUid = _uid;
        showCard = card;
        showTypeNum = showType;
    }
	protected override void begin ()
	{
		base.begin ();
		tapBase.changeTapPage(tapBase.tapButtonList[startIndex]);
        updateTapPage(startIndex);
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
	    if (gameObj.name == "close")
	    {
			ResonanceSampleManager.Instance.showNum(false);
	        finishWindow();
	    }
		
		 
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
       // updateEquipIntensify();
	}
    /// <summary>
    /// 回来的时候更新面板
    /// </summary>
    /// <param name="index"></param>
    private void updateTapPage(int index)
    {
        if (index==0)
        {
            updateEquipIntensify();
        }
        else if(index==1)
        {
            updateEquipRefineIntensify();
        }
        else
        {
            updateCardStarSoulIntensify();
        }
    }

    /// <summary>
    /// 更新装备强化面板
    /// </summary>
    public void updateEquipIntensify()
    {

        if (showCard != null)
        {
            equiqianghuacontent.initServer(showCard, this, showTypeNum);
        }
        else
        {
            equiqianghuacontent.init(StorageManagerment.Instance.getRole(cardUID), this, showTypeNum);
        }
        
    }
    /// <summary>
    /// 更新装备精炼面板
    /// </summary>
    public void updateEquipRefineIntensify()
    {
        if (showCard != null)
        {
            equipjingliancontent.initServer(showCard, this, showTypeNum);
        }
        else
        {
            equipjingliancontent.init(StorageManagerment.Instance.getRole(cardUID), this, showTypeNum);
        }
    }
    /// <summary>
    /// 更新星魂面板
    /// </summary>
    public void updateCardStarSoulIntensify()
    {
        if (showCard != null)
        {
            cardstarsoulcontent.initServer(showCard, this, showTypeNum, chatPlayerUid);
        }
        else
        {
            cardstarsoulcontent.init(StorageManagerment.Instance.getRole(cardUID), this, showTypeNum);
        }
    }
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase(gameObj, enable);
        if (gameObj.name == "buttonEquipjinglian" && enable) {//装备精练
            zhuangbeijinglianGameObject.gameObject.SetActive(true);
            zhuangbenqianghGameObject.SetActive(false);
            cardstarsoulGameObject.SetActive(false);
            startIndex = 1;
            updateEquipRefineIntensify();
        } else if (gameObj.name == "buttonEquipQianghua" && enable) {//装备强化
            zhuangbeijinglianGameObject.gameObject.SetActive(false);
            zhuangbenqianghGameObject.SetActive(true);
            cardstarsoulGameObject.SetActive(false);
            startIndex = 0;
            updateEquipIntensify();
        } else if (gameObj.name == "buttonStarQianghua" && enable) {//星魂强化
            zhuangbeijinglianGameObject.gameObject.SetActive(false);
            zhuangbenqianghGameObject.SetActive(false);
            cardstarsoulGameObject.SetActive(true);
            startIndex = 2;
            updateCardStarSoulIntensify();
        }
        //下3个备用
        else if (gameObj.name == "buttonEquipjinglian" && !enable) {//装备精练
        } else if (gameObj.name == "buttonEquipQianghua" && !enable) {//装备强化
        } else if (gameObj.name == "buttonStarQianghua" && !enable) {//星魂强化
        }
	}
}
