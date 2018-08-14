using UnityEngine;
using System.Collections;

public class ButtonShenGe : ButtonBase
{

    public UITexture iconTexture;//图标
    public UITexture bgSprite;//背景
    public UITexture addSprite;//可镶嵌标志
    public UISprite groupSprite;//可升级标志
    public UISprite changeSprite;//有比当前神格更高级的神格
    public UILabel lockLabel;//解锁神格条件描述
    private int index;//槽位
    private int state;//槽位状态（四种）
    private Prop thisProp;

    /// <summary>
    /// 初始化
    /// </summary>
    public void init(Prop selecteProp,int localIndex)
    {
        thisProp = selecteProp;
        index = localIndex;
        lockLabel.text = "";
        if (ShenGeManager.Instance.isLocked(index))//该槽未解锁
        {
            state = ShenGeManager.LOCKED;
            lockLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_002", CommandConfigManager.Instance.getNumOfBeast()[index -1].ToString());
            lockLabel.gameObject.SetActive(true);
            return;
        }
        if (thisProp == null)
        {
            //检测是否有可以装备的神格
            if (ShenGeManager.Instance.isHasCanEquipShenGe())//有可以装备的神格
            {
                state = ShenGeManager.HAVECANEQUIP;
                addSprite.gameObject.SetActive(true);
                return;
            }
            state = ShenGeManager.NOCANEQUIP;
            return;
        }
        state = ShenGeManager.EQUIPED;
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + thisProp.getIconId(),
            iconTexture);
        lockLabel.gameObject.SetActive(false);
        addSprite.gameObject.SetActive(false);

        //检测是否有更高级的同类型神格
        if (ShenGeManager.Instance.isHasBatterProp(thisProp))//有更好的可装备神格
        {
            changeSprite.gameObject.SetActive(true);
            return;
        }
        //检测是否可以升级
        if (ShenGeManager.Instance.checkCanGroup(thisProp,ShenGeManager.SHENGEWINDOW))//可升级
        {
            groupSprite.gameObject.SetActive(true);
        }
    }

    public override void DoClickEvent()
    {
        base.DoClickEvent();
        switch (state)
        {
            case ShenGeManager.LOCKED:
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_005"));
                });
                break;
            case ShenGeManager.NOCANEQUIP:
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_006"));
                });
                break;
            case ShenGeManager.EQUIPED:
                //打开神格详细信息界面（包含升级、替换按钮）
                UiManager.Instance.openDialogWindow<ShenGeGroupWindow>((win) => {
                    win.Initialize(thisProp, index,ShenGeManager.SHENGEWINDOW);
                });
                break;
            case ShenGeManager.HAVECANEQUIP:
                //打开仓库
                UiManager.Instance.openWindow<ShenGeAloneWindow>((win) =>
                {
                    win.init(ShenGeManager.EQUIP,null,index);
                });
                break;
        }
    }
}
