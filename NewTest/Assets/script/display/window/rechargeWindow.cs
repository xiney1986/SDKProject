using UnityEngine;
using System.Collections.Generic;

public class rechargeWindow : WindowBase
{

	public ContentRecharge contentGoods;
	public GameObject rechargeButtonPrefab;
	public UITexture bannerBg;
	public Texture[] bannerTextures;
    public List<string> firstSid=null;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.rechargeWWindow = this;
        RmbFristFPort fport = FPortManager.Instance.getFPort("RmbFristFPort") as RmbFristFPort;
        fport.access(getInfo);

		//还没充值过
        
	}

    public void updateRMB()
    {
        RmbFristFPort fport = FPortManager.Instance.getFPort("RmbFristFPort") as RmbFristFPort;
        fport.access(getInfo);
    }
    void getInfo(List<string> ids) {
        firstSid = ids;
        updateUI();
    }
    void updateUI() {
        bool isFirstState = false;
        if (firstSid != null) {
            if (firstSid.Count < SdkManager.jsonGoodsList.Count - 1) {
                isFirstState = true;
            } else {
                isFirstState = false;
            }
        } else {
            isFirstState = true;
        }
        if (isFirstState) {
            bannerBg.mainTexture = bannerTextures[0];//双倍
        } else {
            bannerBg.mainTexture = bannerTextures[1];
        }

        if (SdkManager.jsonGoodsList != null) {
            contentGoods.sidList = firstSid;
            contentGoods.reLoad(SdkManager.jsonGoodsList.Count);
        }   
        MaskWindow.UnlockUI();
    }


	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("backGround_1");
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
            UiManager.Instance.rechargeWWindow = null;
			finishWindow ();
		} else if (gameObj.name == "buttonvip") {
			UiManager.Instance.switchWindow<VipWindow> ();
		} else if (gameObj.name == "buttonbuy") {
		}
	}
 
}
