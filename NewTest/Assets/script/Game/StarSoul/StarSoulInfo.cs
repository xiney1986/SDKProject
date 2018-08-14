using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂信息
/// </summary>
public class StarSoulInfo {

	/* fields */
	/** 猎魂品质 */
	int huntQuality;
	/** 碎片数量 */
	int debrisNumber;
	/** 存储的魂经验 */
	long starSoulExp;

	/* methods */
	public StarSoulInfo () {
	}
	/** 序列化读取可变属性数据 */
	public void bytesRead (int j, ErlArray ea) {
		this.huntQuality = StringKit.toInt (ea.Value [j++].getValueString ());
		this.starSoulExp = StringKit.toLong (ea.Value [j++].getValueString ());
		this.debrisNumber = StringKit.toInt (ea.Value [j++].getValueString ());
	}
	/** 添加碎片 */
	public void addDebrisNumber(int debrisNumber) {
		this.debrisNumber += debrisNumber;
	}
	/** 扣除碎片 */
	public bool delDebrisNumber(int debrisNumber) {
		if (this.debrisNumber >= debrisNumber) {
			this.debrisNumber -= debrisNumber;
			return true;
		}
		return false;
	}
	/** 添加存储的魂经验 */
	public void addStarSoulExp(long starSoulExp) {
		this.starSoulExp += starSoulExp;
	}
	/** 扣除存储的魂经验 */
	public bool delStarSoulExp(long starSoulExp) {
		if (this.starSoulExp >= starSoulExp) {
			this.starSoulExp -= starSoulExp;
			return true;
		}
		return false;
	}
	/***/
	public string ToString() {
		return "[huntQuality="+huntQuality+",debrisNumber="+debrisNumber+",starSoulExp="+starSoulExp+"]";
	}

	/* properties */
	/** 设置猎魂品质 */
	public void setHuntQuality(int huntQuality) {
		this.huntQuality = huntQuality;
	}
	/** 获取猎魂品质 */
	public int getHuntQuality() {
		if (huntQuality <= 0)
			return 0;
		return huntQuality-1;
	}
	/** 获取存储的魂经验 */
	public long getStarSoulExp() {
		return starSoulExp;
	}
	/** 设置星魂碎片 */
	public void setDebrisNumber(int debrisNumber) {
		this.debrisNumber = debrisNumber;
	}
	/** 获取星魂碎片 */
	public int getDebrisNumber() {
		return debrisNumber;
	}
}
