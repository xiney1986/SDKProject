using UnityEngine;
using System.Collections;

public class BoxShowWindow : WindowBase {
    /* fields */
    /** 预制件容器数组-钻石商城,神秘商城*/
    /**预制件挂接点 */
    public GameObject itemButtonPrefab;
    public ContentBoxShow content;
    private CallBack callback;
    public UILabel openBoxNum;

    protected override void begin() {
        base.begin();
        initContent();
        MaskWindow.UnlockUI();//解除UI的遮罩
    }
    public override void OnNetResume() {
        base.OnNetResume();
        initContent();
    }

    /// <summary>
    /// 初始化容器
    /// </summary>
    /// <param name="tapIndex">Tap index.</param>
    public void initContent() {
        openBoxNum.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow02", (ClmbTowerManagerment.Instance.getBoxMAxNum()-FuBenManagerment.Instance.getTowerChapter().relotteryNum).ToString());
        content.reLoad(CommandConfigManager.Instance.towerBoxAward.Length);
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="gameObj"></param>
    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "close") {
            finishWindow();
        } else if (gameObj.name == "info") {
            UiManager.Instance.openDialogWindow<TowerInfoShowWindow>();
        }
    }
}
