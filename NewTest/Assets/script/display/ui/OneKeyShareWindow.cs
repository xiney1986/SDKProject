using UnityEngine;
using System.Collections;

public class OneKeyShareWindow : WindowBase
{


    //public UITexture momoTexutre;
    public UILabel shareCountLabel;
    //public UISprite sdkfriendBg;
    public GameObject shareButton;
    public GameObject momoShareButton;
    public UILabel titleLabel;
    public UILabel titleLabel_1;
    private int shareCount;
    public GameObject mask;
    private int type;
    private CallBack callBack;
    public void intoType(int typee, CallBack callBackk)
    {
        type = typee;
        callBack = callBackk;
    }
    public void Awake()
    {
        titleLabel.text = LanguageConfigManager.Instance.getLanguage("ShareAward");
        titleLabel_1.text = LanguageConfigManager.Instance.getLanguage("ShareAward_01");
    }

    protected override void begin()
    {
        base.begin();

        //SdkOneKeyShareFport fport = FPortManager.Instance.getFPort("SdkOneKeyShareFport") as SdkOneKeyShareFport;
        //fport.getSdkOneKeyShareCount((count) =>
        //{
        //    shareCount = count;
        //    shareCountLabel.text = "今天已完成" + count + "次分享";
        //});
        //shareCountLabel.text = LanguageConfigManager.Instance.getLanguage("ShareTodayComplete", "10");
        MaskWindow.UnlockUI();
        //StartCoroutine (waitForTime(2.5f));
    }

    public void initWin()
    {
        SdkOneKeyShareFport fport = FPortManager.Instance.getFPort("SdkOneKeyShareFport") as SdkOneKeyShareFport;
        fport.getSdkOneKeyShareCount((count) =>
        {
            shareCountLabel.text = LanguageConfigManager.Instance.getLanguage("ShareTodayComplete", "" + count);
        });
        momoShareButton.SetActive(false);
        shareButton.SetActive(true);
        shareButton.transform.localPosition = new Vector3(-7, 78, 0);
    }



    //IEnumerator waitForTime(float time)
    //{
    //    yield return new WaitForSeconds (time);
    //    MaskWindow.UnlockUI ();		
    //}	
    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        MaskWindow.UnlockUI();
        switch (gameObj.name)
        {
            case "close":
                mask.SetActive(false);
                finishWindow();
                break;
        }
    }
    /// <summary>
    /// 增加抽奖次数
    /// </summary>
    void addShareDraw()
    {
        if (type == 1)
        {
            if (ShareDrawManagerment.Instance.isFirstShare == 0)
            {
                SdkOneKeyShareFport fport = FPortManager.Instance.getFPort("SdkOneKeyShareFport") as SdkOneKeyShareFport;
                fport.sendDrawShareSuccess(SdkOneKeyShareFport.TYPE_SHARE_DRAW, () =>
                {
                    if (callBack != null) callBack();
                    callBack = null;
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                    {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("shareDraw04"));
                    });
                });
            }
            else
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("shareDraw04"));
                });

            }
        }
    }
}