using UnityEngine;
using System.Collections;

public class LoadingWindow: WindowBase
{
	public Transform loading;
	public UILabel progressLabel;
	public UISlider progressBar;
	public UILabel descLabel;
	public bool justLoading;
	public UITexture loadingTexture;
	public static bool isShowProgress=true;

	private int randomNum;
	private string sid;
	private string loadingTextureId;


	public override void OnAwake ()
	{
		base.OnAwake ();
		dialogCloseUnlockUI=false;
		randomNum=2049+Random.Range(1,12);
//        if(CommandConfigManager.Instance.getNvShenClothType() == 0)
//		    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH +randomNum+"c",loadingTexture);
//        else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + randomNum, loadingTexture);
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + randomNum, loadingTexture);
		UiManager.Instance.ActiveLoadingWindow = this;
	}

	protected override void begin ()
	{
		base.begin ();
		if(isShowProgress)
			animRoot.gameObject.SetActive(true);

		isShowProgress=true;
		progressLabel.text = "0%";
		progressBar.value = 0;
		//StringKit.intToFixString(1);
		string tipMessage=RadioManager.RandomTipMessage(0,16); 
		descLabel.text = tipMessage;
	}
	 
 

	public void setProgress (float  data)
	{
		progressBar.value  = data;
		progressLabel.text = (int)(data * 100) + "%";
	}

	public void showProgress ()
	{
		progressLabel.color = new Color (1, 1, 1, 1);
	}
}
