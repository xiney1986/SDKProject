using UnityEngine;
using System.Collections;

public class ArenaAwardItem : MonoBehaviour {
	/** 积分奖励用名字 */
    public UILabel lblName;
	/** 竞猜奖励用背景 */
	public UISprite bg00;
	public UISprite bg01;
//	public UISprite bg02;
    public ButtonBase buttonConfirm;
    public GameObject completedIcon;
    public GameObject content;
    public UILabel lblCondition;
	public GameObject firstAwardLogo;
	public GameObject title;

    [HideInInspector] public ArenaAwardWindow window;

    ArenaAwardInfo info;

    public void init(ArenaAwardInfo info)
    {
        this.info = info;
        bool canReceived = false;
        lblCondition.text = "";
        completedIcon.SetActive(false);
		bg01.gameObject.SetActive (false);
//		bg02.gameObject.SetActive (false);
		firstAwardLogo.gameObject.SetActive (false);
		title.gameObject.SetActive (false);
		/** 决赛奖励 */
        if (info.sample.type == ArenaAwardWindow.TYPE_FINAL)
        {
			lblName.text = "[FFF0C1]" + info.sample.name;
			lblName.effectStyle = UILabel.Effect.Outline;
			lblName.effectColor = new Color(33f/255,59f/255,87f/255);
			lblName.transform.localPosition = new Vector3(0,86,0);
            buttonConfirm.gameObject.SetActive(false);
			if(info.sample.type==ArenaAwardSample.TYPE_FINAL)
			{
				if(info.sample.condition==ArenaAwardSample.CONDITION_TYPE)
				{
//					bg00.spriteName = "img_7";
//					bg02.gameObject.SetActive(true);
					firstAwardLogo.gameObject.SetActive(true);
					lblName.gameObject.SetActive(true);
					title.gameObject.SetActive(false);
					lblName.gameObject.SetActive(false);
				}
				else
				{
					firstAwardLogo.gameObject.SetActive(false);
					title.gameObject.SetActive(true);
					lblName.gameObject.SetActive(true);
					lblName.gameObject.transform.localPosition=new Vector3(0,lblName.gameObject.transform.localPosition.y,lblName.gameObject.transform.localPosition.z);
				}
			}
        }
		/** 竞猜奖励 */
		else if (info.sample.type == ArenaAwardWindow.TYPE_GUESS)
        {
			lblName.transform.localPosition = new Vector3(0,60,0);
			lblName.text = "[A93D12]" + info.sample.name+"[-]";
			content.transform.localPosition = new Vector3(-196,-25,-300);
			buttonConfirm.transform.localPosition = new Vector3(180,-20,0);
			bg01.gameObject.SetActive (true);
            canReceived = info.condition > 0;
            lblCondition.text = LanguageConfigManager.Instance.getLanguage("Arena33", info.condition.ToString());
        }
		/** 积分奖励 */
		else
        {
			lblName.transform.localPosition = new Vector3(-27,0,0);
			lblName.text = "[6E473D]" + info.sample.name + "[-]";
			content.transform.localPosition = new Vector3(-196,-5,-300);
			buttonConfirm.transform.localPosition = new Vector3(180,-5,0);
            completedIcon.SetActive(info.received);
			buttonConfirm.gameObject.SetActive(!info.received);
            canReceived = info.condition >= info.sample.condition;
        }

        if (canReceived && !info.received)
        {
			buttonConfirm.disableButton(false);
        } else
        {
			buttonConfirm.disableButton(true);
        }

        int pos = 0;
        Utils.DestoryChilds(content.gameObject);
        for(int i = 0; i < info.sample.prizes.Length; i++)
        {
            PrizeSample ps = info.sample.prizes[i];
//            if(ps.type != PrizeType.PRIZE_CARD)
//            {
                GameObject obj = NGUITools.AddChild(content.gameObject,window.goodsViewPrefab);
                GoodsView sc = obj.GetComponent<GoodsView>();
                sc.init(ps);
				sc.fatherWindow = window;
                obj.transform.localScale = new Vector3(0.95f,0.95f,1);
                obj.transform.localPosition = new Vector3(pos*110 - 20,0,0);
                pos++;
//            }
        }
    }

    public void OnClick()
    {
        if (info.sample.type == ArenaAwardSample.TYPE_INTEGRAL)
        {
            FPortManager.Instance.getFPort<ArenaReceiveAwardIntegralFPort>().access(OnReceiveBack,info.sample.sid,1);
        }
        else  if (info.sample.type == ArenaAwardSample.TYPE_GUESS)
        {
            FPortManager.Instance.getFPort<ArenaReceiveAwardGuessFPort>().access(OnReceiveBack,info.sample.condition);
        }
         
    }


    void OnReceiveBack(bool result)
    {
        if (result)
        {
            TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("Arena30"));
            info.received = true;
            if(info.sample.type == ArenaAwardWindow.TYPE_GUESS)
            {
                info.condition = 0;
            }
            init(info);
        } else
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage(result ? "Arena30" : "Arena31"));
        }
    }
}
