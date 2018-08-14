using UnityEngine;
using System.Collections;

public class StarSoulStoreAloneWindow : WindowBase {

	/**field**/
	/** 星魂仓库 */
	public GameObject contentPrefab;
	/** 星魂仓库*/
	public GameObject starSoulStorecontent;

	public Card card;
	public StarSoul starSoul;
	public ButtonStoreStarSoul.ButtonStateType type;
	/** 激活window */
	protected override void begin () {
		base.begin ();		
		MaskWindow.UnlockUI ();
	}
	

	/// <summary>
	/// 初始化星魂强化界面
	/// </summary>
	public void init(Card card,StarSoul starSoul,ButtonStoreStarSoul.ButtonStateType type)
	{
		this.card = card;
		this.type = type;
		this.starSoul = starSoul;
		initContent(card,starSoul,type);
	}
	public void initContent(Card card,StarSoul starSoul,ButtonStoreStarSoul.ButtonStateType type) {
		//starSoulStorecontent.SetActive(false);
		Utils.RemoveAllChild(starSoulStorecontent.transform);
		GameObject content=NGUITools.AddChild(starSoulStorecontent,contentPrefab);
		StarSoulStoreContent ssc = content.GetComponent<StarSoulStoreContent> ();
		//筛选条件
		SortCondition sc=new SortCondition();
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		int [] intArray = StarSoulManager.Instance.getCardSoulExistType(card,starSoul);
		//屏蔽掉筛选的同类型星魂
		sc.addSiftCondition (new Condition (SortType.STARSOUL_TYPE, intArray));
		ssc.sc=sc;
		ssc.init(this,type);
	}
	
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}else{
			MaskWindow.UnlockUI();
		}
		
	}
	public void updateUI()
	{

	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		initContent(card,starSoul,type);
		StarSoulManager.Instance.setSoulStarState();
		StarSoulWindow win = UiManager.Instance.getWindow<StarSoulWindow>();
		if(win!=null)
			win.finishWindow();
		finishWindow();
		UiManager.Instance.openWindow<StarSoulWindow>((win_1)=>{win_1.init(2);});

	}

}
