using UnityEngine;
using System.Collections;

public enum MiningTypePage {
    MiningGold = 1,
    MiningGem = 3

}

public enum MiningSize {
    Small = 0,
    Medium = 1,
    Large = 2
}

public class MiningTypeWindow : WindowBase {
    public GameObject chooiceType;
    public GameObject chooiceGemSize;
    public GameObject chooiceGlodSize;
    public UILabel[] gemDesk;
    public UILabel[] goldDesk;

    MiningTypePage pageIndex;

    protected override void begin() {
        base.begin();
        MaskWindow.UnlockUI();
    }

    public void Init(MiningTypePage pageIndex) {
        this.pageIndex = pageIndex;
        switch (pageIndex) {
            case MiningTypePage.MiningGem:
                chooiceType.SetActive(false);
                chooiceGemSize.SetActive(true);
                break;
            case MiningTypePage.MiningGold:
                chooiceType.SetActive(false);
                chooiceGlodSize.SetActive(true);
                break;
        }
        gemDesk[0].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGem, MiningSize.Small).time / 3600).ToString());
        gemDesk[1].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGem, MiningSize.Medium).time / 3600).ToString());
        gemDesk[2].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGem, MiningSize.Large).time / 3600).ToString());

        goldDesk[0].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGold, MiningSize.Small).time / 3600).ToString());
        goldDesk[1].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGold, MiningSize.Medium).time / 3600).ToString());
        goldDesk[2].text = LanguageConfigManager.Instance.getLanguage("mining_time", (MiningSampleManager.Instance.GetMiningSample(MiningTypePage.MiningGold, MiningSize.Large).time / 3600).ToString());


    }

    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "close") {
            finishWindow();
        }
        if (gameObj.name == "GoldType") {
            Init(MiningTypePage.MiningGold);
        }
        if (gameObj.name == "GemType") {
           Init(MiningTypePage.MiningGem);
        }
        if (gameObj.name == "GemLarge") {
            MiningMineral(MiningTypePage.MiningGem, MiningSize.Large);
        }
        if (gameObj.name == "GemMedium") {
            MiningMineral(MiningTypePage.MiningGem, MiningSize.Medium);

        }
        if (gameObj.name == "GemSmall") {
            MiningMineral(MiningTypePage.MiningGem, MiningSize.Small);

        }
        if (gameObj.name == "GoldLarge") {
            MiningMineral(MiningTypePage.MiningGold, MiningSize.Large);

        }
        if (gameObj.name == "GoldMedium") {
            MiningMineral(MiningTypePage.MiningGold, MiningSize.Medium);

        }
        if (gameObj.name == "GoldSmall") {
            MiningMineral(MiningTypePage.MiningGold, MiningSize.Small);
        }
    }


    void MiningMineral(MiningTypePage type, MiningSize size) {
        dialogCloseUnlockUI = false;
        finishWindow();
        EventDelegate.Add(OnHide, () => {
            if (ArmyManager.Instance.getArmy(4) == null) {
                ArmyManager.Instance.InitMiningTeam();
                ArmyManager.Instance.SaveMiningArmy(() => {
                    ArmyManager.Instance.cleanAllEditArmy();
                    UiManager.Instance.openWindow<TeamEditWindow>(win => {
                        win.setShowTeam(4);
                        win.setComeFrom(TeamEditWindow.FROM_MINING, true, MiningSampleManager.Instance.GetMiningSample(type, size).sid);
                    });
                });
            } else {
                UiManager.Instance.openWindow<TeamEditWindow>(win => {
                    if (ArmyManager.Instance.getArmy(4).state != 1) {
                        win.setShowTeam(4);

                    } else {
                        win.setShowTeam(5);
                    }
                    win.setComeFrom(TeamEditWindow.FROM_MINING, true, MiningSampleManager.Instance.GetMiningSample(type, size).sid);
                });
            }
        });
    }
}
