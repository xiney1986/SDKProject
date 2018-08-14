using UnityEngine;
using System.Collections;

/// <summary>
/// 精灵控制
/// </summary>
public class SpriteCardCtrl : ButtonBase
{
	public int index;
	/** 正面 */
	public RoleView front;
	public GoodsView _front;
	/** 反面 */
	public UITexture back;
	/** 数量  */
	public UILabel numLabel;
    public GameObject quanquan;
	/** 0:正面 1:反面 */
	private int state = 0;
	/** 是否领取过 */
	private bool isAward;
	/** 反面点击动态效果 */
	public UITexture[] quan;
    public Vector3 oldPostion;
    public UILabel freeCost;
    public UILabel costLabel;
    CallBack<GameObject> callBack;
    CallBack<GameObject> callBackk;
	[HideInInspector]
	public PrizeSample prize;

	public override void DoClickEvent ()
	{
		if (isAward) {
			MaskWindow.UnlockUI ();
			return;		
		}
		base.DoClickEvent ();
	}

	/** 转到正面 */
	public IEnumerator turnToFront (CallBack callBack)
	{
		iTween.RotateTo (this.gameObject, new Vector3 (0f, 0f, 0f), 1f);
		yield return new WaitForSeconds (0.1f);
		back.depth = 4;
		state = 0;
		updateQuan (false);
		yield return new WaitForSeconds (0.9f);
		if (callBack != null) {
			callBack ();
			callBack = null;
		}
	}
    /** 转到正面 */
    public void turnToFrontt(CallBack<GameObject> callBack) {
        this.callBack = callBack;
        iTween.RotateTo(this.gameObject, iTween.Hash("rotation", new Vector3(0f, this.gameObject.transform.localRotation.y + 90f, 0f), "time", 0.01, "oncomplete", "stepOneComepet1", "easeType", iTween.EaseType.linear));
        state = 0;
    }
    void stepOneComepet1() {
        back.depth =0;
        back.gameObject.transform.FindChild("quan").gameObject.SetActive(false);
        iTween.RotateTo(this.gameObject, iTween.Hash("rotation", new Vector3(0f, this.gameObject.transform.localRotation.y + 180f, 0f), "time", 0.2, "oncomplete", "stepOneComepet2", "easeType", iTween.EaseType.linear));
    }
    void stepOneComepet2() {
        if (callBack != null) {
            callBack(this.gameObject);
            callBack = null;
        }
    }
    public void turnToBackk(CallBack<GameObject> callBack) {
        this.callBackk = callBack;
        iTween.RotateTo(this.gameObject, iTween.Hash("rotation", new Vector3(0f, this.gameObject.transform.localRotation.y + 90f, 0f), "time", 0.5, "oncomplete", "stepOneComepett1", "easeType", iTween.EaseType.linear));
        state = 0;
    }
    void stepOneComepett1() {
        back.depth =300;
        back.gameObject.transform.FindChild("quan").gameObject.SetActive(true);
        back.gameObject.transform.localEulerAngles = new Vector3(0f,180f,0f);
        iTween.RotateTo(this.gameObject, iTween.Hash("rotation", new Vector3(0f, this.gameObject.transform.localRotation.y + 180f, 0f), "time", 0.5, "oncomplete", "stepOneComepett2", "easeType", iTween.EaseType.linear));
    }
    void stepOneComepett2() {
        if (callBackk != null) {
            callBackk(this.gameObject);
            callBack = null;
        }
    }

	public void turnToFrontDirect ()
	{
		this.transform.localRotation = Quaternion.identity;
		back.depth = 4;
		state = 0;
		updateQuan (false);
	}
	/** 转到背面 */
	public IEnumerator turnToBack (CallBack callBack)
	{
		iTween.RotateTo (this.gameObject, new Vector3 (0f, 180f, 0f), 1f);
		yield return new WaitForSeconds (0.1f);
		back.depth = 300;
		state = 1;
		updateQuan (true);
		yield return new WaitForSeconds (0.9f);
		if (callBack != null) {
			callBack ();
			callBack = null;
		}
	}

	public void turnToBackDirect ()
	{
		this.transform.localRotation = new Quaternion (0f, 180f, 0f, 1f);
		back.depth = 100;
		state = 1;
		updateQuan (true);
	}

	/** 移动到某个位置 */
	public IEnumerator moveToPosition (Vector3 position, float time)
	{
		iTween.MoveTo (this.gameObject, position, time);
		yield return new WaitForSeconds (time);
	}
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="reward">奖励</param>
	/// <param name="isAward">是否领取过</param>
//	public void init (TurnSpriteReward reward, bool isAward)
//	{
//		this.isAward = isAward;
//		CardSample sample = CardSampleManager.Instance.getRoleSampleBySid (reward.sid);
//		front.init (sample, null, null);
//		numLabel.text = "x" + reward.num.ToString ();
//	}

	public void init (TurnSpriteReward reward, bool isAward)
	{
		this.isAward = isAward;
		_front.clean();
		if (reward.type == "card") {
			CardSample sample = CardSampleManager.Instance.getRoleSampleBySid (reward.sid);
			front.gameObject.SetActive(true);
			front.init (sample, null, null);
			numLabel.text = "x" + reward.num.ToString ();
		} 
		else if(reward.type == "euip") {

			PrizeSample sample = new PrizeSample(PrizeType.PRIZE_EQUIPMENT,reward.sid,reward.num);
			_front.init (sample,true);
			numLabel.text = "x" + reward.num.ToString ();
		}
		else if(reward.type == "goods") {
			
			PrizeSample sample = new PrizeSample(PrizeType.PRIZE_PROP,reward.sid,reward.num);
			_front.gameObject.SetActive(true);
			_front.init (sample,true);
			numLabel.text = "x" + reward.num.ToString ();
		}
//		else if(reward.type == "starsoul") {
//			
//			PrizeSample sample = new PrizeSample(PrizeType.PRIZE_STARSOUL,reward.sid,reward.num);
//			_front.init (sample);
//			numLabel.text = "x" + reward.num.ToString ();
//		}
	}

	public void updateQuan (bool flag)
	{
		foreach (UITexture a in quan) {
			a.gameObject.SetActive (flag);
		}
	}
    public void updateCost(bool flag) {
        if(freeCost != null)
        freeCost.gameObject.SetActive(flag);
        if(costLabel != null)
        costLabel.gameObject.SetActive(flag);
    }

	public bool getIsAward ()
	{
		return isAward;
	}
}
