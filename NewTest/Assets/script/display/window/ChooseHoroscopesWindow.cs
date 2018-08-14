using UnityEngine;
using System.Collections;

public class ChooseHoroscopesWindow : WindowBase
{
    public HoroscopesRotate rotate;
    public HoroscopesRotateItem item;
    public UICenterOnChild content;

    public EffectPrinterWord labelDesc;
    public EffectPrinterWord labelSDesc;
    public EffectPrinterWord labelPDesc;

    public UILabel labelName;
    public UILabel labelDate;
    public UITexture iconTexture;
    public UISprite lightSprite;
    public UISprite sprite;//小标签
    private int starType = 1;

    protected override void begin()
    {
        base.begin();
        int sdkType = 0;
        sdkType = Random.Range(1, 13);
        starType = Mathf.Max(sdkType, 1);
        rotate.initItem(starType);
        MaskWindow.UnlockUI();
    }

    protected override void DoEnable()
    {
        base.DoEnable();
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
    }

    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "sure")
        {
            UiManager.Instance.openWindow<TitlesHoroscopesWinow>((win) =>
            {
                win.init(starType);
            });
        }
        if (gameObj.name == "close")
        {
            LoginFPort port = FPortManager.Instance.getFPort("LoginFPort") as LoginFPort;
            //下线操作 
            port.closeContect();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLostConnect(false);
            }
            //	GameManager.Instance.Init ();
        }
    }

    public void updateUI(int _type)
    {
        starType = _type;
        iconTexture.alpha = 0;
        sprite.alpha = 0;
        lightSprite.alpha = 0;
        Horoscopes info = HoroscopesManager.Instance.getStarByType(starType);
        if (CommandConfigManager.Instance.getNvShenClothType() == 0)
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + info.getImageID() + "c", iconTexture);
        else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + info.getImageID(), iconTexture);
        labelName.text = info.getName();
        labelDesc.text = info.getDescribe();
        labelSDesc.text = info.getSkillDescribe();
        labelPDesc.text = info.getPassiveDescribe();
        labelDate.text = info.getDate();
        sprite.spriteName = info.getSpriteName();
        sprite.MakePixelPerfect();
        showEffect();

        //		StartCoroutine(writeString(info.getDescribe()));
    }

    void showEffect()
    {
        TweenAlpha lname = TweenAlpha.Begin(labelName.gameObject, 1f, 1);
        lname.from = 0;
        //		TweenAlpha ldesc = TweenAlpha.Begin (labelDesc.gameObject, 1f, 1);
        //		ldesc.from = 0;
        TweenAlpha ldate = TweenAlpha.Begin(labelDate.gameObject, 1f, 1);
        ldate.from = 0;
        TweenAlpha lTexture = TweenAlpha.Begin(iconTexture.gameObject, 1f, 1);
        lTexture.from = 0;
        TweenAlpha lSprite = TweenAlpha.Begin(sprite.gameObject, 1f, 1);
        lSprite.from = 0;
        TweenAlpha lightSp = TweenAlpha.Begin(lightSprite.gameObject, 1f, 1);
        lightSp.from = 0;
        lightSp.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    //打字机效果
    IEnumerator writeString(string str)
    {
        foreach (char s in str.ToCharArray())
        {
            labelDesc.text += s;
            yield return new WaitForSeconds(0.1f);
        }
    }



    public int getStarType()
    {
        return starType;
    }
}
