using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// pvp奖励展示窗口
/// </summary>
public class PvpAwardWindow : WindowBase
{

	/* fields */
	/** 排序对象 */
	GoodsView.GoodsViewComp comp=new GoodsView.GoodsViewComp();
	public UILabel winCountLabel;
	public GameObject awardContent;
	public GameObject closeButton;
	public GameObject goodsViewPrefab;
	/** 连胜次数 */
	int winCount;
	/** 连胜奖励列表 */
	List<GameObject> awardList;
	int setp;
	int nextSetp;

	/* methods */
	/** 初始化 */
	public void init(int winCount,Award[] awards){
		this.winCount = winCount;
		initAwards (awards);
	}
	/** 初始化奖励 */
	void initAwards(Award[] awards)
	{
		if (awards == null) return;
		awardList = new List<GameObject> ();
		Award award=Award.mergeAward (awards);
		CreateGoodsByAward (awardList,award);
		SortAwardItem ();
	}
	/** 奖励条目排序 */
	void SortAwardItem()
	{
		if (awardList==null||awardList.Count<=1) return;
		GameObject[] objs=awardList.ToArray ();
		SetKit.sort (objs,comp);
		awardList.Clear ();
		foreach(GameObject obj in objs)
		{
			awardList.Add(obj);
		}
	}
	/** 创建奖励对象 */
	private void CreateGoodsByAward (List<GameObject> awards,Award aw)
	{
		GameObject obj;
		int nameIndex = 0;
		if (aw.props != null && aw.props.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (PropAward o in aw.props) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += o.num;
				else
					map.Add (o.sid, o.num);
			}
			foreach (int key in map.Keys) {
				obj=CreateGoodsItem (key, map [key], 0);
				nameIndex++;
				obj.name="goodsbutton_"+nameIndex;
				awards.Add (obj);
			}
		}
		if (aw.equips != null && aw.equips.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (EquipAward o in aw.equips) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += 1;
				else
					map.Add (o.sid, 1);
			}
			foreach (int key in map.Keys) {
				obj=CreateGoodsItem (key, map [key], 1);
				nameIndex++;
				obj.name="goodsbutton_"+nameIndex;
				awards.Add (obj);
			}
		}
        if (aw.magicWeapons != null && aw.magicWeapons.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (MagicwWeaponAward o in aw.magicWeapons) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 3);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
		if (aw.cards != null && aw.cards.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (CardAward o in aw.cards) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += 1;
				else
					map.Add (o.sid, 1);
			}
			foreach (int key in map.Keys) {
				obj=CreateGoodsItem (key, map [key], 2);
				nameIndex++;
				obj.name="goodsbutton_"+nameIndex;
				awards.Add (obj);
			}
		}
	}
	//0道具,1装备,2卡片 3,神器
	private GameObject CreateGoodsItem (int sid, int count, int type)
	{
		GameObject obj = NGUITools.AddChild(awardContent,goodsViewPrefab) as GameObject;
		obj.SetActive(false);
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.linkQualityEffectPoint ();
		view.fatherWindow = this;
		if (type == 0) {
			Prop p = PropManagerment.Instance.createProp(sid,count);
			view.init(p);
		} else if (type == 1) {
			Equip e = EquipManagerment.Instance.createEquip(sid);
			view.init(e);
			view.onClickCallback = ()=>{
				hideWindow();
				UiManager.Instance.openWindow<EquipAttrWindow>(
					(winEquip) => {
					winEquip.Initialize(e, EquipAttrWindow.OTHER, ()=>{
						EventDelegate.Add(winEquip.OnHide,()=>{
							restoreWindow();
						});
					});
				}
				);
			};
		} else if (type == 2) {
			Card c =CardManagerment.Instance.createCard(sid);
			view.init(c);
			view.onClickCallback = ()=>{
				hideWindow();
				CardBookWindow.Show(c, CardBookWindow.SHOW, ()=>{
					EventDelegate.Add(UiManager.Instance.getWindow<CardBookWindow>().OnHide,()=>{
						restoreWindow();
					});
				});
			};
		}else if(type==3){
            MagicWeapon mc = MagicWeaponManagerment.Instance.createMagicWeapon(sid);
            view.init(mc);
            view.onClickCallback = () => {
                hideWindow();
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(mc, MagicWeaponType.FORM_OTHER);
                });
            };
        }
		return obj;
	}
	/***/
	protected override void begin ()
	{
		loadShow();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();
		}, 0.2f));
		MaskWindow.UnlockUI ();
	}
	void Update ()
	{
		if (setp == nextSetp)
			return;
		if (setp == 0) {
			winCountLabel.gameObject.SetActive (true);
			TweenScale ts = TweenScale.Begin(winCountLabel.gameObject,0.15f,new Vector3 (1.4f, 1.4f, 1f));
			ts.method = UITweener.Method.EaseIn;
			ts.from = new Vector3 (5, 5, 1);
			EventDelegate.Add (ts.onFinished, () =>
			{
				iTween.ShakePosition (winCountLabel.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (winCountLabel.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				StartCoroutine(Utils.DelayRun(()=>{
					NextSetp ();
				},0.1f));
			}, true);
		}
		if (setp == 1) {
			if (awardList == null || awardList.Count == 0) {
				NextSetp ();
			} else {
				awardContent.SetActive (true);
				TweenPosition tp = TweenPosition.Begin (awardContent, 0.15f, awardContent.transform.localPosition);
				tp.from = new Vector3 (0, -500, 0);
				EventDelegate.Add (tp.onFinished, () =>
				{
					float time = GoodsInAnimation (awardList);
					StartCoroutine (Utils.DelayRun (() =>
					{
						NextSetp ();
					}, time));
				}, true);
			}
		} else if (setp == 2) {
			closeButton.SetActive (true);
		}
		setp++;
	}
	private float GoodsInAnimation (List<GameObject> list)
	{
		float time = 0.3f;
		foreach (GameObject obj in list) {
			obj.SetActive (true);
			GoodsInFireworksEffect(obj);
			TweenScale.Begin (obj, time, obj.transform.localScale).from = new Vector3 (5, 5, 0);
			time += 0.1f;
		}
		return time;
	}
	private void GoodsInFireworksEffect(GameObject obj)
	{
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.showFireworksEffectByQuality ();
	}
	/** 显示UI */
	void loadShow()
	{
		winCountLabel.text = winCount.ToString ();
		loadShowAward ();
	}
	/** 奖励显示 */
	void loadShowAward(){
		if (awardList == null)
			return;
		for (int i = 0; i < awardList.Count; i++) {
			GameObject obj = awardList [i];
			obj.transform.localPosition = new Vector3 (i * 120, 0, 0);
			obj.transform.localScale =  new Vector3(1,1,1);
		}
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close")
		{
			finishWindow();
		}
	}
	public void NextSetp ()
	{
		nextSetp++;
	}
}
