using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//神格替换升级界面
//李程
public class ShenGeGroupWindow : WindowBase
{
	
	public Prop chooseProp;
	public PrizeSample prize;
	public UILabel propName;
    public UILabel ValueLabel;
    public UILabel valueType;//类型
    public UILabel DescribeLabel;
    public UILabel heChengLabel;
    public UILabel addValueLabel;
	public UITexture propImage;
	public UISprite quality;
    public ButtonBase buttonChange;
    public ButtonBase buttonGroup;
    public ButtonBase buttonCompound;//
    public UILabel[] CostInfoLabels;
    public UILabel currentPropName;
    public UILabel desc;//神格升级资源简要描述
	private Vector3 arrowPosition;
    private MessageHandle msg;
    private CallBackMsg callback;
    private int localIndex;
    private Prop nextProp;
    private int intoWinType ;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public void Initialize (Prop chooseItem,int index,int intoType)
	{
	    localIndex = index;
		chooseProp = chooseItem;
	    intoWinType = intoType;
        nextProp = chooseProp;
	    for (int i = 0; i < CostInfoLabels.Length; i++)
	    {
	        CostInfoLabels[i].text = "";
	    }
	    if (ShenGeManager.Instance.getShowShenGeList(chooseProp).Count > 0)
	    {
	        buttonChange.disableButton(false);
	    }
	    else
	    {
            buttonChange.disableButton(true);
	    }
	    if(chooseProp != null) 
        {
            if (intoType == ShenGeManager.STORAGE) {
                buttonChange.gameObject.SetActive(false);
                buttonGroup.gameObject.SetActive(false);
            } else if (intoType == ShenGeManager.SHENGEWINDOW) {
                buttonCompound.gameObject.SetActive(false);
            }
            string str = "";
            switch (chooseProp.getType()) {
                case PropType.PROP_SHENGE_HP:
                    str = LanguageConfigManager.Instance.getLanguage("s0005");
                    break;
                case PropType.PROP_SHENGE_ATT:
                    str = LanguageConfigManager.Instance.getLanguage("s0006");
                    break;
                case PropType.PROP_SHENGE_DEF:
                    str = LanguageConfigManager.Instance.getLanguage("s0007");
                    break;
                case PropType.PROP_SHENGE_MAG:
                    str = LanguageConfigManager.Instance.getLanguage("s0008");
                    break;
                case PropType.PROP_SHENGE_AGI:
                    str = LanguageConfigManager.Instance.getLanguage("s0009");
                    break;
            }
            valueType.text = str;
            currentPropName.text = QualityManagerment.getQualityColor(chooseProp.getQualityId()) + chooseProp.getName();
            if (!ShenGeManager.Instance.checkCanGroup(chooseProp, intoType))
            {
                DescribeLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_022");
                PropSample tmpSample = PropSampleManager.Instance.getPropSampleBySid(chooseProp.getNextShenGeSid());
                if (tmpSample == null)
                {
                    DescribeLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_030");
                    desc.text = "";
                }
                else
                {
                    nextProp = PropManagerment.Instance.createProp(tmpSample.sid);
                    desc.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_038");
                }
                ResourcesManager.Instance.LoadAssetBundleTexture(
                    ResourcesManager.ICONIMAGEPATH + chooseProp.getIconId(), propImage);
                //propName.text = QualityManagerment.getQualityColor(chooseProp.getQualityId()) + chooseProp.getName();
                //propName.gameObject.transform.localPosition = new Vector3(130,23.5f,0);
                propName.text = "";
                heChengLabel.text = "";
                quality.spriteName = QualityManagerment.qualityIDToIconSpriteName(chooseProp.getQualityId());
                ValueLabel.text = chooseProp.getEffectValue()+ "";
                addValueLabel.text = "";
                buttonGroup.disableButton(true);
                return;
            }
            buttonGroup.disableButton(false);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(chooseProp.getNextShenGeSid());
		    if (sample == null) return;
            else nextProp = PropManagerment.Instance.createProp(sample.sid);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + sample.iconId, propImage);
            heChengLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_021");
            propName.text = QualityManagerment.getQualityColor(sample.qualityId) + sample.name;
            propName.gameObject.transform.localPosition = new Vector3(183, 23.5f, 0);
            quality.spriteName = QualityManagerment.qualityIDToIconSpriteName(sample.qualityId);
            ValueLabel.text = chooseProp.getEffectValue()+ "";
            addValueLabel.text = "+"+ (sample.effectValue - chooseProp.getEffectValue()) +"";
            List<ShenGeInfo> tmpList = ShenGeManager.Instance.shengeList;
            for (int i = 0; i < tmpList.Count; i++)
            {
                Prop tmpProp = PropManagerment.Instance.createProp(tmpList[i].sid);
                if (tmpProp != null)
                {
                    CostInfoLabels[i].text = QualityManagerment.getQualityColor(tmpProp.getQualityId()) + tmpProp.getName() + "X" + tmpList[i].num;
                }
            }
        }
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonGroup" || gameObj.name == "buttonCompound") {//升级(合成)
		    ShenGeManager.Instance.checkCanGroup(chooseProp, intoWinType);
		    List<ShenGeInfo> list = ShenGeManager.Instance.shengeList;
			string str = "";
		    for (int i = 0; i < list.Count; i++)
		    {
		        if (i == list.Count - 1)
		            str += (list[i].sid + "," + list[i].num);
		        else
		            str += (list[i].sid + "," + list[i].num + ";");
		    }
		    if (gameObj.name == "buttonCompound")
		    {
                ShenGeGroupFPort fports = FPortManager.Instance.getFPort("ShenGeGroupFPort") as ShenGeGroupFPort;
                fports.access(chooseProp.sid, str, () => {
                    Prop tmp = PropManagerment.Instance.createProp(chooseProp.getNextShenGeSid());
                    finishWindow();
                    UiManager.Instance.openDialogWindow<MessageLineWindow>(win => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_016",
                            QualityManagerment.getQualityColor(tmp.getQualityId()) + tmp.getName()),false);
                    });
                    if (UiManager.Instance.getWindow<StoreWindow>() != null)
                        UiManager.Instance.getWindow<StoreWindow>().updateContent();
                });
		        return;
		    }
		    ShenGeGroupFPort fport = FPortManager.Instance.getFPort("ShenGeGroupFPort") as ShenGeGroupFPort;
            fport.access(chooseProp.sid, localIndex, str, () =>
            {
                Prop tmp = PropManagerment.Instance.createProp(chooseProp.getNextShenGeSid());
                EffectManager.Instance.CreateEffectCtrlByCache(propImage.transform.parent, "Effect/UiEffect/Reinforced_SyntheticTwo", null);
                MaskWindow.LockUI();
                StartCoroutine(Utils.DelayRun(() =>
                {
                    Initialize(nextProp, localIndex, intoWinType);
                    UiManager.Instance.openDialogWindow<MessageLineWindow>(win => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_016",
                            QualityManagerment.getQualityColor(tmp.getQualityId()) + tmp.getName()),false);
                    });
                    if (UiManager.Instance.getWindow<NvShenShenGeWindow>() != null)
                        UiManager.Instance.getWindow<NvShenShenGeWindow>().init();
                }, 2));
            });

		} else if (gameObj.name == "buttonChange") {//替换
            //打开仓库
            UiManager.Instance.openWindow<ShenGeAloneWindow>((win) =>
            {
                win.init(ShenGeManager.CHANGE,chooseProp,localIndex);
                finishWindow();
            });
        } else if (gameObj.name == "buttonClose")
        {
            finishWindow();
            MaskWindow.UnlockUI();
        }
	}
}
