using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 卡牌训练
/// </summary>
public class CardTrainingWindow : WindowBase
{
    public CardTrainingItemView[] UI_Items;
    public Card []cardInfo = new Card[3];

    private int mSelecteCardIndex = -1;

    protected override void begin()
    {
        base.begin();
		if (GuideManager.Instance.isEqualStep(131003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
        MaskWindow.UnlockUI();
    }

    public override void OnBeginCloseWindow()
    {
        UI_Items[0].SetData(null);
        UI_Items[1].SetData(null);
        UI_Items[2].SetData(null);
        base.OnBeginCloseWindow();
    }


    public override void OnStart()
    {
        base.OnStart();
        InitCardTraining fport = FPortManager.Instance.getFPort("InitCardTraining") as InitCardTraining;
        fport.access(onReceiveInit);
    }
    /// <summary>
    /// 断线重连
    /// </summary>
    public override void OnNetResume() {
        base.OnNetResume();
        CardTrainingTimeWindow win = UiManager.Instance.getWindow<CardTrainingTimeWindow>();
        if (win != null) {
            InitCardTraining fport = FPortManager.Instance.getFPort("InitCardTraining") as InitCardTraining;
            fport.access(win.onReceiveInit);
        } else {
            InitCardTraining fport = FPortManager.Instance.getFPort("InitCardTraining") as InitCardTraining;
            fport.access(onReceive);
        }
    }
    private void onReceiveInit(int cardIndex, int time)
    {
        if (time <= 0) return;
        //UI_Items[cardIndex].SetCD(time);
        CardTrainingManagerment.Instance.UpdateTime(cardIndex, time);
    }
    private void onReceive(int cardIndex, int time) {
        if (time <= 0) return;
        //UI_Items[cardIndex].SetCD(time);
        CardTrainingManagerment.Instance.UpdateTime(cardIndex, time);
        UI_Items[0].SetData(null);
        UI_Items[1].SetData(null);
        UI_Items[2].SetData(null);
    }
    protected override void DoUpdate()
    {
        base.DoUpdate();

    }

    

    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
		if (gameObj.name == "close") {
			ArmyManager.Instance.cleanAllEditArmy();
			finishWindow();
		} else if (gameObj.name == "ButtonHelp") {
			UiManager.Instance.openDialogWindow<CardTrainingHelpWindow>();
		}

    }


    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="card"></param>
    public void setCardData(Card card)
    {
        if (mSelecteCardIndex == -1) return;
        CardTrainingItemView itemView = UI_Items[mSelecteCardIndex];
        if (mSelecteCardIndex == 0) {
            cardInfo[0] = card;
        } else if (mSelecteCardIndex == 1) {
            cardInfo[1] = card;
        } else if (mSelecteCardIndex == 2) {
            cardInfo[2] = card;
        }
        itemView.SetData(card);
        mSelecteCardIndex = -1;
    }


    /// <summary>
    /// 获得卡牌item
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CardTrainingItemView getCardItem(int index)
    {
        return UI_Items[index];
    }


    /// <summary>
    /// 打开卡牌选择窗口,并记录是第几个栏位
    /// </summary>
    /// <param name="index"></param>
    public void showChooseCard(int index)
    {
        mSelecteCardIndex = index;
        UiManager.Instance.openWindow<CardChooseWindow>((win) => {
            win.Initialize(CardChooseWindow.CARDTRAINING);
        });
    }


}

