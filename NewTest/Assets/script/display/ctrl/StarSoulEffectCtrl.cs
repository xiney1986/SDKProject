using UnityEngine;
using System.Collections;

public class StarSoulEffectCtrl : EffectCtrl
{
	public UISprite[] sprites;
	public Color[] colors_1;
	public Color[] colors_2;
	public Color[] colors_3;
	public Color[] colors_4;
	public Color[] colors_5;
    public Color[] colors_6;

	public void setColor (int quality)
	{
		switch(quality)
		{
		case 1:
			for (int i=0;i<sprites.Length;i++) {
				sprites[i].color=colors_1[i];
			}
			break;

		case 2:
			for (int i=0;i<sprites.Length;i++) {
				sprites[i].color=colors_2[i];
			}
			break;

		case 3:
			for (int i=0;i<sprites.Length;i++) {
				sprites[i].color=colors_3[i];
			}
			break;

		case 4:
			for (int i=0;i<sprites.Length;i++) {
				sprites[i].color=colors_4[i];
			}
			break;

		case 5:
			for (int i=0;i<sprites.Length;i++) {
				sprites[i].color=colors_5[i];
			}
			break;
        case 6:
            for (int i = 0; i < sprites.Length; i++) {
                sprites[i].color = colors_6[i];
            }
            break;

		}


	}

}
