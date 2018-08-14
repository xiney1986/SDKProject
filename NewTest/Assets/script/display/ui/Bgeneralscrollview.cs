 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 普通坐骑面板
/// </summary>
public class Bgeneralscrollview : dynamicContent {

	/* enum */
	/** 属性窗口类型 */
	public enum ButtonStateType {
		acitve, // 激活
		ride, // 乘骑
		stop, // 休息
	}

	/* gameobj filed */
	public GameObject mountStoreItemPerfab;//每个坐骑显示的预制体
	/** 排序对象 */
	MountsItemComp comp = new MountsItemComp ();

	/*  filed */
	Exchange[] exchangeList;
	List<Exchange> exlist;
	private int exchangeType;
	bool bl=false;
	/* methods */
	/** 初始化 */
	public void init(WindowBase win,int type,bool showUseMount) {
		this.fatherWindow=win;
		bl=showUseMount;
		if(type==MountStoreContent.TAP_AGENRAL_CONTENT){
			exlist=ExchangeManagerment.Instance.getAllExchangesMount (ExchangeType.MOUNT);
			if(showUseMount)
			updateMountList(exlist);
			exchangeList=exlist.ToArray();
			(fatherWindow as MountsWindow).setButtonState(MountStoreContent.SHOW_RIDE_MOUNT);
			SetKit.sort (exchangeList, comp);
			cleanAll();
			updateUI();
		}else if(type==MountStoreContent.TAP_BACTICE_CONTENT){
			if(GuildManagerment.Instance.getGuild()!=null){
				MountsHaveGuildFPort fport = FPortManager.Instance.getFPort<MountsHaveGuildFPort> ();
				fport.initGuildIinfo (initConetnt);
			}else{
				initConetnt();
			}
		}

	}
	void initConetnt(){
		exlist=ExchangeManagerment.Instance.getAllExchangesMount (ExchangeType.BACTICMOUNT);
		if(bl)
			updateMountList(exlist);
		exchangeList=exlist.ToArray();
		(fatherWindow as MountsWindow).setButtonState(MountStoreContent.SHOW_RIDE_MOUNT);
		SetKit.sort (exchangeList, comp);
		cleanAll();
		updateUI();
	}
	/// <summary>
	/// 对排序进行特殊更新
	/// </summary>
	/** 更新UI */
	public void updateUI() {
		if(exchangeList!=null)
			base.reLoad (exchangeList.Length);
	}
	/** 容器更新完毕 */
	public override void jumpToPage (int index)
	{
		base.jumpToPage (index);
		if (GuideManager.Instance.isEqualStep (134005000) || GuideManager.Instance.isEqualStep (134006000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
	/**排处正常骑乘的坐骑 */
	private void updateMountList(List<Exchange> exchangeList){
		MountsManagerment mountsManagerment=MountsManagerment.Instance;
		for(int i=0;i<exchangeList.Count;i++){
			Mounts activeMounts=mountsManagerment.getMountsBySid(exchangeList[i].getExchangeSample().exchangeSid);
			if(activeMounts!=null&&activeMounts.isInUse()){
				exchangeList.RemoveAt(i);
				break;
			}
		}
	}
	/** 更新条目 */
	public override void updateItem (GameObject item, int index) {
		MountStoreItem mountItem = item.GetComponent<MountStoreItem> ();
		mountItem.updateItem(exchangeList[index],this.fatherWindow);
	}
	/** 初始化按钮 */
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, mountStoreItemPerfab);
		}
	}
	/** 设置按钮类型 */
	public void setExchangeType(int type) {
		this.exchangeType=type;
	}

	/** 坐骑排序 */
	class MountsItemComp : Comparator {
		
		public int compare (object o1, object o2) {
			//排序 先显示激活的（骑乘，非骑乘（品质 品质一样用战斗力））
			if (o1 == null)
				return 1;
			if (o2 == null)
				return -1;
			if (!(o1 is Exchange) || !(o2 is Exchange))
				return 0;
			Exchange obj1 = (Exchange)o1;
			Exchange obj2 = (Exchange)o2;
			ExchangeSample temp1=obj1.getExchangeSample();
			ExchangeSample temp2=obj2.getExchangeSample();
			MountsSample ms1= MountsSampleManager.Instance.getMountsSampleBySid(temp1.exchangeSid);
			MountsSample ms2= MountsSampleManager.Instance.getMountsSampleBySid(temp2.exchangeSid);
			if(ms1.sortIndex<ms2.sortIndex){
				return 1;
			}
			MountsManagerment manager=MountsManagerment.Instance;
			Mounts isHave1=manager.getMountsBySid(temp1.exchangeSid);
			Mounts isHave2=manager.getMountsBySid(temp2.exchangeSid);
			if(isHave1!=null&&isHave2!=null) {
				int quality1 = isHave1.getQualityId ();
				int quality2 = isHave2.getQualityId ();
				if(isHave1.isInUse()) {
					return -1;
				} else if(isHave2.isInUse()) {
					return 1;
				} else {
					if (quality1 == quality2) {
						int combat1 = isHave1.getMaxLevel ();
						int combat2 = isHave2.getMaxLevel ();
						if (combat1 > combat2)
							return -1;
						if (combat1 < combat2)
							return 1;
						return 0;
					} else if (quality1 > quality2){
						return -1;
					} else {
						return 1;
					}
				}
			} else if(isHave1!=null&&isHave2==null) {
				return -1;
			} else if(isHave1==null&&isHave2!=null) {
				return 1;
			}else if(isHave1==null&&isHave2==null){
				ExchangeManagerment instanc=ExchangeManagerment.Instance;
				if(((instanc.isCheckPremises(temp1,0)&&instanc.isCheckConditions(temp1,0))||instanc.isCheckPremises(temp1,1))&&
				   (!(instanc.isCheckPremises(temp2,0)&&instanc.isCheckConditions(temp2,0))||!instanc.isCheckPremises(temp2,1)))return -1;
				if(((instanc.isCheckPremises(temp2,0)&&instanc.isCheckConditions(temp2,0))||instanc.isCheckPremises(temp2,1))&&
				   (!(instanc.isCheckPremises(temp1,0)&&instanc.isCheckConditions(temp1,0))||!instanc.isCheckPremises(temp1,1)))return 1;
			}
			return 0;
		}
	}
}