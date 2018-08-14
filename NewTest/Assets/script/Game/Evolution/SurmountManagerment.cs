using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurmountManagerment : SampleConfigManager {

	//单例
	private static SurmountManagerment instance;

	private List<SurmountSample> surInfo;
	
	public SurmountManagerment ()
	{
		base.readConfig (ConfigGlobal.CONFIG_SURMOUNT);
	}
	public static SurmountManagerment Instance {
		get{
			if(instance==null)
				instance=new SurmountManagerment();
			return instance;
		}
	}

	//解析配置
	public override void parseConfig (string str)
	{  
		SurmountSample be = new SurmountSample (str);
		if(surInfo == null)
			surInfo = new List<SurmountSample>();
		surInfo.Add(be);
	}

	/** 获取所有突破信息 */
	public List<SurmountSample> getSurInfoList()
	{
		return surInfo;
	}

	/** 获取指定(类型)突破信息 */
	public SurmountSample getSurInfoByType (Card card)
	{
		if(surInfo == null)
			return null;
		for(int i = 0;i<surInfo.Count;i++)
		{
			if(surInfo[i].getCardType() == card.getEvolveNextSid())
				return surInfo[i];
		}
		return null;
	}

	/** 是否能突破 */
	public bool isCanSurLevel (Card card)
	{
		if(isMaxSurLevel(card))
			return false;
		return card.getEvoLevel () >= getCardLevel (card);
	}

	/** 是否已满足突破的条件(不带提示) */
	public bool isCanSurByCon(Card card)
	{	
		int evoLevel = SurmountManagerment.Instance.getCardLevel(card);

		//突破等级已满
		if(SurmountManagerment.Instance.isMaxSurLevel(card))
			return false;

		//进化等级不足
		if(!SurmountManagerment.Instance.isCanSurLevel(card))
			return false;

		//突破金钱不足
		if(SurmountManagerment.Instance.getNeedMoney(card) > UserManager.Instance.self.getMoney())
			return false;
		
		EvolutionCondition[] evoCon = getEvoCon(card);
		
		if(evoCon != null){

			Prop showProp;
			
			for (int i = 0; i < evoCon.Length; i++) {
				
				showProp = StorageManagerment.Instance.getProp(evoCon[i].costSid);
				return showProp == null ? false:(evoCon[i].num > showProp.getNum() ? false:true);
//				bool num = showProp == null ? false:(evoCon[i].num > showProp.getNum() ? false:true);
//				if(!num)
//				{
//					Prop prop = PropManagerment.Instance.createProp(evoCon[i].costSid);
//					return false;
//				}
			}
		}
		
		return true;
	}
	/** 是否已满足突破的条件(不带提示) */
	public bool isCanSurByConOnly(Card card)
	{	
		int evoLevel = SurmountManagerment.Instance.getCardLevel(card);
		
		//突破等级已满
		if(SurmountManagerment.Instance.isMaxSurLevel(card))
			return false;
		EvolutionCondition[] evoCon = getEvoCon(card);
		
		if(evoCon != null){
			
			Prop showProp;
			
			for (int i = 0; i < evoCon.Length; i++) {
				
				showProp = StorageManagerment.Instance.getProp(evoCon[i].costSid);
				return showProp == null ? false:(evoCon[i].num > showProp.getNum() ? false:true);
				//				bool num = showProp == null ? false:(evoCon[i].num > showProp.getNum() ? false:true);
				//				if(!num)
				//				{
				//					Prop prop = PropManagerment.Instance.createProp(evoCon[i].costSid);
				//					return false;
				//				}
			}
		}
		
		return true;
	}

	/** 是否已突破到极限 */
	public bool isMaxSurLevel (Card card)
	{
		SurmountSample info = getSurInfoByType(card);
		if(info == null)
			return true;
		return (card.getSurLevel() >= info.getMaxSurNum()) ? true:false;
	}

	/** 获取极限突破等级 */
	public int getMaxSurLevel (Card card)
	{
		SurmountSample info = getSurInfoByType(card);
		if(info == null)
			return 0;
		return info.getMaxSurNum();
	}

	/** 获取当前突破到下一级的需求培养等级 */
	public int getCardLevel(Card card)
	{
		if(isMaxSurLevel(card))
			return 0;
		SurmountSample info = getSurInfoByType(card);
		return info == null ? 0 : info.getSurLevel()[card.getSurLevel()];
	}

	/** 获取当前卡片的品质 */
	public int getQuitlyLevel(Card card)
	{
		if((card.getSurLevel() - 1) < 0)
			return CardSampleManager.Instance.getRoleSampleBySid (card.sid).qualityId;
		SurmountSample info = getSurInfoByType(card);
		return info == null ? 0 : info.getQuitlyLevel()[card.getSurLevel() - 1];
	}

	/** 获取当前卡片的形象 */
	public int getImageSid(Card card)
	{
		SurmountSample info = getSurInfoByType(card);
		if(info == null || info.getIcoSid() == null)
			return CardSampleManager.Instance.getRoleSampleBySid (card.sid).imageID;
		else
			return info.getIcoSid()[card.getEvoLevel() - 1];
	}

 
	/** 突破后增加等级上限 */
	public int getAddMaxSurLevel(Card card)
	{
		if((card.getSurLevel() - 1) < 0)
			return 0;
		SurmountSample info = getSurInfoByType(card);
		return info == null ? 0 : info.getMaxSurLevel()[card.getSurLevel() - 1];
	}

	/** 获取当前卡片突破到下一级的品质 */
	public int getNextSurLevelQuitlyLevel(Card card)
	{
		SurmountSample info = getSurInfoByType(card);
		if(isMaxSurLevel(card))
			return info == null ? 0 : info.getQuitlyLevel()[card.getSurLevel() - 1];
		else
			return info == null ? 0 : info.getQuitlyLevel()[card.getSurLevel()];
	}

	/** 获取下次突破是否改变品质 */
	public bool isNextSurChangeQuitly(Card card)
	{
		return getQuitlyLevel(card) != getNextSurLevelQuitlyLevel(card);
	}

	/** 获取当前卡片突破到下一级的需求金钱消耗 */
	public int getNeedMoney(Card card)
	{
		if(isMaxSurLevel(card))
			return 0;
		SurmountSample info = getSurInfoByType(card);
		if(info == null)
			return 0;
		int upMoney = info.getNeedMoney()[card.getSurLevel()];
		int downMoney = (card.getSurLevel() - 1) < 0 ? 0:info.getNeedMoney()[card.getSurLevel() - 1];
		return upMoney - downMoney;
	}

	/** 获取当前卡片突破到下一级的需求消耗 */
	public EvolutionCondition[] getEvoCon(Card card)
	{
		if(isMaxSurLevel(card))
			return null;
		SurmountSample info = getSurInfoByType(card);
		if(info != null && info.getConditions() != null)
			return info.getConditions()[card.getSurLevel()];//取下一级消耗需求，不需要-1
		else 
			return null;
	}
	
	/** 获取当前卡片的附加效果 */
	public AttrChangeSample[] getAddEffect(Card card)
	{
		if((card.getSurLevel() - 1) < 0)
			return null;
		SurmountSample info = getSurInfoByType(card);
		if(info == null || info.getAddEffects() == null)
			return null;
		return info.getAddEffects()[card.getSurLevel() - 1];	//取当前突破等级卡牌效果，需要-1
	}

	/** 获取当前卡片突破到下一级的附加效果文字说明 */
	public string[] getNextLevelAddEffectByString(Card card)
	{
		if(isMaxSurLevel(card))
			return null;
		SurmountSample info = getSurInfoByType(card);
		return info == null ? null : info.getAddEffectByString()[card.getSurLevel()];	//取下一级卡牌效果，不需要-1
	}

	/** 获取当前卡片的附加效果文字说明 */
	public string[] getAddEffectByString(Card card)
	{
		if((card.getSurLevel() - 1) < 0)
			return null;
		SurmountSample info = getSurInfoByType(card);
		return info == null ? null : info.getAddEffectByString()[card.getSurLevel() - 1];	//取当前突破等级卡牌效果，需要-1
	}

	/** 获取当前卡片的所有附加效果文字说明 */
	public string[][] getAddEffectByStringByAll(Card card)
	{
		SurmountSample info = getSurInfoByType(card);
		return info == null ? null : info.getAddEffectByString();
	}

	/** 获得当前突破属性加成 */
	public CardBaseAttribute getSurmountAttr (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();  
		AttrChangeSample[] effects = getAddEffect(card);
		if (effects == null || effects.Length < 1)
			return attr;
		//基础值 + (卡片等级 - 1) * 成长值
		for (int j = 0; j < effects.Length; j++) {
			
			if (effects [j].getAttrType () == AttrChangeType.HP) {
				attr.hp += effects [j].getAttrValue (card.getLevel()); 
			} else if (effects [j].getAttrType () == AttrChangeType.ATTACK) {
				attr.attack += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.DEFENSE) {
				attr.defecse += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.MAGIC) {
				attr.magic += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.AGILE) {
				attr.agile += effects [j].getAttrValue (card.getLevel());
			}
		} 
		
		return attr;
	}

	/** 获得当前突破属性比例加成 */
	public CardBaseAttribute getSurmountAttrPer (Card card)
	{
		CardBaseAttribute attr = new CardBaseAttribute ();  
		AttrChangeSample[] effects = getAddEffect(card);
		if (effects == null || effects.Length < 1)
			return attr;
		//基础值 + (卡片等级 - 1) * 成长值
		for (int j = 0; j < effects.Length; j++) {
			
			if (effects [j].getAttrType () == AttrChangeType.PER_HP) {
				attr.perHp += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_ATTACK) {
				attr.perAttack += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_DEFENSE) {
				attr.perDefecse += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_MAGIC) {
				attr.perMagic += effects [j].getAttrValue (card.getLevel());
			} else if (effects [j].getAttrType () == AttrChangeType.PER_AGILE) {
				attr.perAgile += effects [j].getAttrValue (card.getLevel());
			}
		} 
		
		return attr;
	}
}
