using UnityEngine;
using System.Collections.Generic;

public class HappySundayContentItem : MonoBehaviour
{
    public UILabel UI_TitleLabel;
    public UISlider UI_Progress;
    public UILabel UI_ProgressLabel;
    public GameObject UI_GoodsContainer;
    public GoodsView UI_GoodsTemplate;
    public ButtonBase UI_ReceiveBtn;


    private HappySundaySample mBaseData;

    private void Start()
    {
        UIEventListener.Get(UI_ReceiveBtn.gameObject).onClick = onClickReceive;
    }



    private void onClickReceive(GameObject go)
    {
        HappySundayReceive fport = FPortManager.Instance.getFPort("HappySundayReceive") as HappySundayReceive;
        fport.access(mBaseData.Sid, onReceive);

    }


    private void onReceive()
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("s0120"));
        });
        HappySundayManagerment.Instance.AddReceive(mBaseData.Sid);
        updateReceiveBtn();
    }


    private void updateReceiveBtn()
    {
        bool isReceive = HappySundayManagerment.Instance.IsReceive(mBaseData.Sid);
        UI_ReceiveBtn.disableButton(isReceive || HappySundayManagerment.Instance.CurrentScore < mBaseData.MaxScore);
        if (isReceive)
            UI_ReceiveBtn.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
    }


    public void SetData(HappySundaySample baseData, WindowBase faterWindow)
    {
        mBaseData = baseData;

        int currentScore = Mathf.Min(baseData.MaxScore, HappySundayManagerment.Instance.CurrentScore);
        UI_TitleLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("s0564"), "[ffcc00]" + baseData.MaxScore + "[-]");
        UI_Progress.value = (float)currentScore / (float)baseData.MaxScore;
        UI_ProgressLabel.text = currentScore + "/" + baseData.MaxScore;
        
        for (int i = 0; i < baseData.AwardGoods.Length; i++)
		{
            GoodsView goods = NGUITools.AddChild(UI_GoodsContainer.gameObject, UI_GoodsTemplate.gameObject).GetComponent<GoodsView>();
            goods.init(baseData.AwardGoods[i].type, baseData.AwardGoods[i].sid, baseData.AwardGoods[i].num);
            goods.fatherWindow = faterWindow;
		}
        UI_GoodsContainer.GetComponent<UIGrid>().Reposition();

        updateReceiveBtn();
    }



}

