using UnityEngine;
using System.Collections;

public class ButtonBeast : ButtonBase
{
	public UITexture cardImage;
	public UITexture ice_bg;
	BeastEvolve beastEvo;
	
	public void updateBeast (BeastEvolve evo)
	{
		beastEvo = evo;
		
		updateAll();
	}

	public void updateAll()
	{
		if (beastEvo == null)
			return;
		Card tmpCard = beastEvo.getBeast();
		
		


	    if (tmpCard.uid == "")
	    {
            //cardImage.color=new Color(0.1f,0.1f,0.1f,1);
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "nvshen_ice", ice_bg);
            ice_bg.gameObject.SetActive(true);
	    }
		else{
			cardImage.color=Color.white;
			ice_bg.gameObject.SetActive(false);
        }
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH, beastEvo.getBeast(), cardImage);
	}

	public BeastEvolve getBeastEvo()
	{
		return beastEvo;
	}
}
