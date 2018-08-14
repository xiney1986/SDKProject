using System;
using System.Collections;
using System.Collections.Generic;

public class BloodConfigManager : SampleConfigManager {
    public List<BloodPointSample> sampleList;
    public BloodConfigManager()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
        base.readConfig(ConfigGlobal.CONFIG_BLOODPOINT); 
	}
	
	//单例
    private static BloodConfigManager _Instance;
	private static bool _singleton = true;

    public static BloodConfigManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
                _Instance = new BloodConfigManager();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
    public override void parseConfig(string str) {
        BloodPointSample sample = new BloodPointSample(str);
        if (sampleList == null) {
            sampleList = new List<BloodPointSample>();
        }
        sampleList.Add(sample);
    }
    public BloodPointSample getBloodPointSampleBySid(int sid) {
        for (int i = 0; i < sampleList.Count; i++) {
            if (sid == sampleList[i].sid) {
                return sampleList[i] as BloodPointSample;
            }
        }
        return null;
    }
    /// <summary>
    /// 拿到当前合适的血脉图 那个阶段
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public int[] getCurrentBloodMap(int sid,int lv)
    {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i=0;i<totalItem.Length;i++)
        {
            if(totalItem[i].Length==1&&totalItem[i][0]==0)continue;
            int[] temp = totalItem[i];
            for (int j=0;j<temp.Length;j++)
            {
                if (lv == tempNum) return temp;
                tempNum++;
            }
        }
        for (int j=totalItem.Length-1;j>=0;j--)
        {
            if (totalItem[j][0]!=0)
            {
                return totalItem[j];
            }
        }
        return null;
    }
    public int[] getTotalMap(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        List<int> totalMap = new List<int>();
        int[][] totalItem = bps.itemSid;
        int currentMapIndex = getCurrentMapIndex(sid, lv);
        for (int i = 0; i <= currentMapIndex; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++)
                totalMap.Add(temp[j]);
        }
        return totalMap.ToArray();
    }
    public int[] getBloodItemColor(int sid,int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int currentMapIndex = getCurrentMapIndex(sid, lv);
        List<int> colorList = new List<int>();
        int currentQualityId = cs.qualityId;
        int[][] item = bps.itemSid;
        int k = 0;
        for (int i = currentMapIndex; i >= 0;i-- ) {
            int[] tempItem = item[i];
            if (item[i].Length == 1 && item[i][0] == 0) continue;
            for (int j = 0; j < tempItem.Length;j++ ) {
                colorList.Insert(0,currentQualityId + k);
            }
            k--;
        }
        return colorList.ToArray();
    }
    public int getCurrentMapIndex(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            tempNum += temp.Length;
            if (lv < tempNum) return i;
        }
        return totalItem.Length-1;
    }
    public int getCurrentBloodMapIndex(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++) {
                if (lv == tempNum) return j;
                tempNum++;
            }
        }
        for (int j = totalItem.Length - 1; j >= 0; j--) {
            if (totalItem[j][0] != 0) {
                return totalItem[j].Length-1;
            }
        }
        return 0;
    }
    /// <summary>
    /// 卡片是否拥有了血脉技能
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public bool isActiveSkill(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++) {
                if (lv >= tempNum)
                {
                  BloodItemSample btss= BloodItemConfigManager.Instance.getBolldItemSampleBySid(temp[j]);
                    if (btss != null)
                    {
                        bloodEffect[] bes = btss.effects;
                        for (int m=0;m<bes.Length;m++)
                        {
                            if (bes[m].type == 5) return true;
                        }
                    }
                }
                tempNum++;
            }
        }
        return false;
    }
    /// <summary>
    /// 组装血脉技能
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public List<Skill> isActiveSkillSid(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        List<Skill> sids=new List<Skill>();
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++) {
                if (lv > tempNum) {
                    BloodItemSample btss = BloodItemConfigManager.Instance.getBolldItemSampleBySid(temp[j]);
                    if (btss != null) {
                        bloodEffect[] bes = btss.effects;
                        for (int m = 0; m < bes.Length; m++) {
                            if (bes[m].type == 5)
                            {
                                Skill tempSkill = new Skill(bes[m].skillSid, 0, 0);
                                tempSkill.setSkillStateType(SkillStateType.ATTR);
                                sids.Add(tempSkill);
                            } else if (bes[m].type == 6) {
                                Skill newSkill = new Skill(bes[m].skillSid, 0, 0);
                                Skill oldSkill = new Skill(bes[m].drSkillSid, 0, 0);
                                newSkill.setSkillStateType(SkillStateType.ATTR);
                                oldSkill.setSkillStateType(SkillStateType.ATTR);
                                sids.Add(newSkill);
                                for (int k = 0; k < sids.Count; k++) {
                                    if (oldSkill.sid == sids[k].sid) { 
                                     sids.Remove(sids[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                tempNum++;
            }
        }
        return sids;
    }
    /// <summary>
    /// 被替换掉的技能
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public List<Skill> getIsReplacedSkill(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        List<Skill> sids = new List<Skill>();
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++) {
                if (lv > tempNum) {
                    BloodItemSample btss = BloodItemConfigManager.Instance.getBolldItemSampleBySid(temp[j]);
                    if (btss != null) {
                        bloodEffect[] bes = btss.effects;
                        for (int m = 0; m < bes.Length; m++) {
                            if (bes[m].type == 6) {
                                Skill oldSkill = new Skill(bes[m].drSkillSid, 0, 0);
                                oldSkill.setSkillStateType(SkillStateType.ATTR);
                                sids.Add(oldSkill);
                            }
                        }
                    }
                }
                tempNum++;
            }
        }
        return sids;
    }
    
    public int getCurrentBloodQuality(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        Card card = CardManagerment.Instance.createCard(sid);
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int num = 0;
        int falg = 0;
        for (int i = 0; i < totalItem.Length;i++ ) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) {
                falg++;
                continue;
            }
            if (num + totalItem[i].Length >= lv + 1) {
                return CardSampleManager.Instance.getRoleSampleBySid(sid).qualityId + i - falg;
            }
            num += totalItem[i].Length;
        }
        return CardSampleManager.Instance.getRoleSampleBySid(sid).qualityId + totalItem.Length - falg;
    }
    /// <summary>
    /// 得到具体的节点sid
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public int getCurrentItemSid(BloodPointSample bps, int lv)
    {
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++) {
                if (lv == tempNum) return temp[j];
                tempNum++;
            }
        }
        for (int j = totalItem.Length - 1; j >= 0; j--) {
            if (totalItem[j][0] != 0) {
                return totalItem[j][totalItem[j].Length-1];
            }
        }
        return 0;
    }
    /// <summary>
    /// 得到已经激活的血脉节点Sid
    /// </summary>
    /// <returns></returns>
    public int[] getReadyItemSids(BloodPointSample bps,int lv)
    {
        int[] sids=new int[lv];
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        for (int i=0;i<totalItem.Length;i++)
        {
            if(totalItem[i].Length==1&&totalItem[i][0]==0)continue;
            int[] temp = totalItem[i];
            for (int j = 0; j < temp.Length; j++)
            {
                if (tempNum < lv)
                {
                    sids[tempNum] = temp[j];
                    tempNum++;
                }
                else
                {
                    break;
                }
            }
        }
        return sids;
    }

    public int[] getMapQualiy(int sid, int lv)
    {
        int[] temp = getCurrentBloodMap(sid, lv);
        if (temp == null) return new[] {-1, 0};
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        for (int i=0;i<totalItem.Length;i++)
        {
            if (totalItem[i] == temp)
            {
                return new[] {i,temp.Length};
            }
        }
        return new[] {-1, 0};
    }
    /// <summary>
    /// 拿到当前合适的血脉图 那个阶段
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public int[] getCurrentBloodLvColor(int sid, int lv) {
        CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(sid);//卡片模板
        BloodPointSample bps = getBloodPointSampleBySid(cs.bloodPointSid);
        int[][] totalItem = bps.itemSid;
        int tempNum = 0;
        int num = 0;
        for (int i = 0; i < totalItem.Length; i++) {
            if (totalItem[i].Length == 1 && totalItem[i][0] == 0) continue;
            int[] temp = totalItem[i];
            if (num + totalItem[i].Length == lv) {
                return new[] { i + 1, 0 };
            }
            num += totalItem[i].Length;
            for (int j = 0; j < temp.Length; j++) {
                if (lv == tempNum) return  new []{i,j};
                tempNum++;
            }
        }
        for (int j = totalItem.Length - 1; j >= 0; j--) {
            if (totalItem[j][0] != 0) {
                return new []{j,totalItem[j].Length-1};
            }
        }
        return null;
    }
    public int getTotalItemNum(int sid) { 
        CardSample card = CardSampleManager.Instance.getRoleSampleBySid(sid);
        BloodPointSample sample = BloodConfigManager.Instance.getBloodPointSampleBySid(card.bloodPointSid);
        int length = sample.itemSid.Length;
        int tempNum = 0;
        for (int i = 0; i < length; i++) {
            if (sample.itemSid[i].Length == 1 && sample.itemSid[i][0] == 0)
                continue;
            tempNum += sample.itemSid[i].Length;
        }
        return tempNum;
    }
    /// <summary>
    /// 是否已经突破过品质了
    /// </summary>
    /// <param name="sid"></param>
    /// <param name="lv"></param>
    /// <returns></returns>
    public bool isQualityChanged(int sid, int lv) {
        CardSample card = CardSampleManager.Instance.getRoleSampleBySid(sid);
        BloodPointSample sample = BloodConfigManager.Instance.getBloodPointSampleBySid(card.bloodPointSid);
        if (sample != null) {
            int length = sample.itemSid.Length;
            int tempNum = 0;
            for (int i = 0; i < length; i++) {
                if (sample.itemSid[i].Length == 1 && sample.itemSid[i][0] == 0)
                    continue;
                tempNum = sample.itemSid[i].Length;
                if (lv >= tempNum) return true;
                return false;
            }
        }
        return false;
    }
}
