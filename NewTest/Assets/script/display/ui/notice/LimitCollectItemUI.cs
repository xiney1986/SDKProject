using UnityEngine;
using System.Collections;

/// <summary>
/// 限时收集条目
/// </summary>
public class LimitCollectItemUI : MonoBase {
	/** 领取按钮 */
	public ButtonBase receiveButton;
	/** 标题 */
	public UILabel titleLabel;
	/** goodsView预制 */
	public GameObject goodsView;
	/** 动态容器 */
	public UIGrid content;
	/** 奖励的位置 */
	public Transform prizePos;
	private WindowBase win;
    private LimitCollectSample sample;
	public void initUI(LimitCollectSample sample,WindowBase win ){
        this.sample = sample;
        this.win = win;
        titleLabel.text = sample.title;
		receiveButton.fatherWindow = win;
        receiveButton.onClickEvent = receiveReward;
		initReceiveButton ();
		initContent ();
		initPrize ();
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 初始化领取按钮
	/// </summary>
	private void initReceiveButton(){
        if (sample.isReceived)
        {
            receiveButton.disableButton(true);
            receiveButton.textLabel.text = Language("recharge02");
        }
        else
        {
            if (isCanReceive())
            {
                receiveButton.disableButton(false);
            }
            else
            {
                receiveButton.disableButton(true);
            }
        }
	}

	/// <summary>
	/// 初始化奖励
	/// </summary>
	private void initPrize(){
		GameObject go = Instantiate (goodsView) as GameObject;
		go.transform.parent = prizePos;
		go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
		GoodsView view = go.GetComponent<GoodsView>();
        view.fatherWindow = win;
        view.init(sample.prize);
	}

	/// <summary>
	/// 初始化条件content
	/// </summary>
	private void initContent( ){
		Utils.RemoveAllChild (content.transform);
		foreach (LimitCollectCondition condition in sample.conditions) {
			GameObject go  = Instantiate(goodsView) as GameObject;
			go.transform.parent = content.transform;
			go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
			GoodsView view = go.GetComponent<GoodsView>();
            view.fatherWindow = win;
            view.init(condition.prize);
            view.specialName.gameObject.SetActive(true);
            view.specialName.text = getConditionString(condition.collected, condition.need);
		}
		content.Reposition();
	}

	/// <summary>
	/// 是否可以领取
	/// </summary>
	private bool isCanReceive(){
        foreach (LimitCollectCondition condition in sample.conditions)
        {
            
			if(condition.collected< condition.need)
				return false;
		}
		return true;
	}

	private void receiveReward(GameObject go){

        string str = LanguageConfigManager.Instance.getLanguage("notice36");
        if(StorageManagerment.Instance.checkStoreFull(sample.prize,out str))
        {
            UiManager.Instance.createMessageLintWindow(str);
            MaskWindow.UnlockUI();
            return;
        }
        NoticeGetActiveAwardFPort port = FPortManager.Instance.getFPort("NoticeGetActiveAwardFPort") as NoticeGetActiveAwardFPort;
        port.access(sample.sid, receiveRewardCallBack);
	}

    private void receiveRewardCallBack(bool resultState) {
        if (resultState)
        {
            (NoticeActiveManagerment.Instance.getActiveInfoBySid(sample.sid) as LimitCollectSample).isReceived = true;
            sample.isReceived = true;
            initReceiveButton();
            UiManager.Instance.createMessageLintWindow(Language("s0120"));
        }
        MaskWindow.UnlockUI();
    }

    private string getConditionString(int current,int need) {
        if (current < need)
        {
            return Colors.RED + current + "/" + need;
        }
        else {
            return Colors.GREEN + current + "/" + need;
        }

    }

}
