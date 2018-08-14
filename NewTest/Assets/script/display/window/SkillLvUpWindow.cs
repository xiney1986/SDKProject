using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillLvUpWindow : WindowBase {

    public ButtonSkillLevelUp[] buttonList;
    public Card card;
    private LevelupInfo[] mskillsLvUpInfo;
    private LevelupInfo[] bskillsLvUpInfo;
    private LevelupInfo[] askillsLvUpInfo;
    private CallBack callBack;

    protected override void begin() {
        base.begin();
        GuideManager.Instance.guideEvent();
        MaskWindow.UnlockUI();
    }

    public void Initialize(Card _oldCard, Card _newCard, CallBack _callBack) {
        this.callBack = _callBack;
        //先存好经验
        LevelupInfo info = new LevelupInfo();
        info.oldLevel = _oldCard.getLevel();
        info.oldExp = _oldCard.getEXP();
        info.oldExpUp = _oldCard.getEXPUp();
        info.oldExpDown = _oldCard.getEXPDown();
        info.orgData = _oldCard.Clone();

        info.newLevel = _newCard.getLevel();
        info.newExp = _newCard.getEXP();
        info.newExpUp = _newCard.getEXPUp();
        info.newExpDown = _newCard.getEXPDown();

        //然后存卡片技能经验
        LevelupInfo[] msk = null;
        Skill[] oldMskills = _oldCard.getSkills();
        Skill[] newMskills = _newCard.getSkills();
        if (oldMskills != null && oldMskills.Length > 0) {
            msk = new LevelupInfo[oldMskills.Length];
            for (int i = 0; i < oldMskills.Length; i++) {
                if (msk[i] == null)
                    msk[i] = new LevelupInfo();
                msk[i].oldLevel = oldMskills[i].getLevel();
                msk[i].oldExp = oldMskills[i].getEXP();
                msk[i].oldExpUp = oldMskills[i].getEXPUp();
                msk[i].oldExpDown = oldMskills[i].getEXPDown();

                msk[i].newLevel = newMskills[i].getLevel();
                msk[i].newExp = newMskills[i].getEXP();
                msk[i].newExpUp = newMskills[i].getEXPUp();
                msk[i].newExpDown = newMskills[i].getEXPDown();
                msk[i].orgData = newMskills[i];
            }
        }

        LevelupInfo[] bsk = null;
        Skill[] oldBskills = _oldCard.getBuffSkills();
        Skill[] newBskills = _newCard.getBuffSkills();
        if (oldBskills != null && oldBskills.Length > 0) {
            bsk = new LevelupInfo[oldBskills.Length];
            for (int i = 0; i < oldBskills.Length; i++) {
                if (bsk[i] == null)
                    bsk[i] = new LevelupInfo();
                bsk[i].oldLevel = oldBskills[i].getLevel();
                bsk[i].oldExp = oldBskills[i].getEXP();
                bsk[i].oldExpUp = oldBskills[i].getEXPUp();
                bsk[i].oldExpDown = oldBskills[i].getEXPDown();

                bsk[i].newLevel = newBskills[i].getLevel();
                bsk[i].newExp = newBskills[i].getEXP();
                bsk[i].newExpUp = newBskills[i].getEXPUp();
                bsk[i].newExpDown = newBskills[i].getEXPDown();
                bsk[i].orgData = newBskills[i];
            }
        }

        LevelupInfo[] ask = null;
        Skill[] oldAskills = _oldCard.getAttrSkills();
        Skill[] newAskills = _newCard.getAttrSkills();
        if (oldAskills != null && oldAskills.Length > 0) {
            ask = new LevelupInfo[oldAskills.Length];
            for (int i = 0; i < oldAskills.Length; i++) {
                if (ask[i] == null)
                    ask[i] = new LevelupInfo();
                ask[i].oldLevel = oldAskills[i].getLevel();
                ask[i].oldExp = oldAskills[i].getEXP();
                ask[i].oldExpUp = oldAskills[i].getEXPUp();
                ask[i].oldExpDown = oldAskills[i].getEXPDown();

                ask[i].newLevel = newAskills[i].getLevel();
                ask[i].newExp = newAskills[i].getEXP();
                ask[i].newExpUp = newAskills[i].getEXPUp();
                ask[i].newExpDown = newAskills[i].getEXPDown();
                ask[i].orgData = newAskills[i];
            }
        }

        Initialize(_newCard, info, msk, bsk, ask);
    }

    //初始化，带入3大类技能的升级信息
    public void Initialize(Card role, LevelupInfo info, LevelupInfo[] msk, LevelupInfo[] bsk, LevelupInfo[] ask) {
        this.card = role;
        this.mskillsLvUpInfo = msk;
        this.bskillsLvUpInfo = bsk;
        this.askillsLvUpInfo = ask;

        step = 0;
        nextSetp = 1;
        canRefresh = true;

    }
    private void updateSkills() {
        List<LevelupInfo> newSkillsInfo = new List<LevelupInfo>();
        List<LevelupInfo> newMaxLvSkillsInfo = new List<LevelupInfo>();

        if (mskillsLvUpInfo != null) {
            for (int i = 0; i < mskillsLvUpInfo.Length; i++) {
                if ((mskillsLvUpInfo[i].orgData as Skill).isMAxLevel())
                    newMaxLvSkillsInfo.Add(mskillsLvUpInfo[i]);
                else
                    newSkillsInfo.Add(mskillsLvUpInfo[i]);
            }
        }
        if (bskillsLvUpInfo != null) {
            for (int i = 0; i < bskillsLvUpInfo.Length; i++) {
                if ((bskillsLvUpInfo[i].orgData as Skill).isMAxLevel()) {
                    if (!card.isMainCard()) {
                        newMaxLvSkillsInfo.Add(bskillsLvUpInfo[i]);
                    } else {
                        continue;
                    }
                } else
                    newSkillsInfo.Add(bskillsLvUpInfo[i]);
            }
        }
        if (askillsLvUpInfo != null) {
            for (int i = 0; i < askillsLvUpInfo.Length; i++) {
                if ((askillsLvUpInfo[i].orgData as Skill).getShowType() == 2) {
                    continue;
                }
                if ((askillsLvUpInfo[i].orgData as Skill).isMAxLevel())
                    newMaxLvSkillsInfo.Add(askillsLvUpInfo[i]);
                else
                    newSkillsInfo.Add(askillsLvUpInfo[i]);
            }
        }

        if (newMaxLvSkillsInfo != null) {
            for (int i = 0; i < newMaxLvSkillsInfo.Count; i++) {
                newSkillsInfo.Add(newMaxLvSkillsInfo[i]);
            }
        }

        for (int i = 0; i < newSkillsInfo.Count && i < buttonList.Length; i++) {
            if (newSkillsInfo[i] == null)
                continue;
            Skill skillData = (Skill)newSkillsInfo[i].orgData;
            //			Debug.LogError ("--->>" + skillData.getName () + ",i=" + i);
            if (skillData != null && skillData.getSkillStateType() == SkillStateType.ATTR) {
                buttonList[i].gameObject.SetActive(false);
            } else {
                buttonList[i].gameObject.SetActive(true);
                buttonList[i].updateButton(newSkillsInfo[i], card);
                buttonList[i].setNum(i);
            }

        }
    }
    int step = 0;
    int nextSetp = 0;
    bool canRefresh = false;

    void Update() {

        if (canRefresh == true) {
            if (step == nextSetp)
                return;
            if (step == 0) {
                nextSetp++;
            } else if (step == 1) {
                nextSetp++;
                updateSkills();
            } else if (step == 2) {
                nextSetp++;
            } else if (step == 3) {
                nextSetp++;
            } else if (step == 4) {
                nextSetp++;
            } else if (step == 5) {
                nextSetp++;
            } else if (step == 6) {
                nextSetp++;
            } else if (step == 7) {
                nextSetp++;
            } else if (step == 8) {
                canRefresh = false;
                nextSetp++;
            }
            step++;
        }
    }

    public override void DoDisable() {
        base.DoDisable();

    }

    void returnIntensifyWindow() {
        IntensifyCardManager.Instance.setMainCard(card);
        IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_SACRIFICE, (fatherWindow as IntensifyCardWindow).getCallBack());
    }

    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "continue") {
            //继续强化
            if (GuideManager.Instance.guideSid == 12012000) {
                if (!GuideManager.Instance.isGuideComplete()) {
                    ArmyManager.Instance.cleanAllEditArmy();
                    GuideManager.Instance.doGuide();
                    finishWindow();
                    UiManager.Instance.openMainWindow();
                    return;
                } else {
                    //这里特殊操作
                    //TODO
                    finishWindow();
                    UiManager.Instance.openMainWindow();
                }
            }
            else {
                finishWindow();
                returnIntensifyWindow();
                MaskWindow.UnlockUI();
            }
        }
    }
}
