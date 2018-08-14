using UnityEngine;
using System.Collections;

public class ShowStars : MonoBehaviour {
    public GameObject[] stars;
    public void init(MagicWeapon magic,int type) {
        int level = 0;
        if (magic != null) {
            level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magic.sid).starLevel;
            initStar(level, type);
        }
    }
    public void initStar(int starLevel,int type) {
        for (int i = 0; i < stars.Length; i++) {
            stars[i].SetActive(false);
        }
        for (int i = 0; i < starLevel; i++) {
            stars[i].SetActive(true);
        }
        if (type == CardSampleManager.USEDBYCARDITEM) {
            if (starLevel == CardSampleManager.ONESTAR) {
                this.gameObject.transform.localPosition = new Vector3(33, -61, 0);
            } else if (starLevel == CardSampleManager.TWOSTAR) {
                this.gameObject.transform.localPosition = new Vector3(22, -61, 0);
            } else if (starLevel == CardSampleManager.THREESTAR) {
                this.gameObject.transform.localPosition = new Vector3(11, -61, 0);
            } else if (starLevel == CardSampleManager.FOURSTAR) {
                this.gameObject.transform.localPosition = new Vector3(0, -61, 0);
            }
        } else if (type == CardSampleManager.USEDBYCARD) {
            this.gameObject.transform.localPosition = new Vector3(-25, -88, 0);
        } else if (type == CardSampleManager.USEDINBATTLEPREPARE) {
            this.gameObject.transform.localPosition = new Vector3(-5, -77, 0);
        } else if (type == CardSampleManager.USEDBYINTENSIFY) {
            this.gameObject.transform.localPosition = new Vector3(-25, -45, 0);
        } else if (type == CardSampleManager.USEDBYSHOW) {
            this.gameObject.transform.localPosition = new Vector3(-25, -95, 0);
            this.gameObject.transform.localScale = new Vector3(0.9f, 0.9f,1);
        } else if (type == MagicWeaponManagerment.USEDBYMAGICITEM) {
            if (starLevel == CardSampleManager.ONESTAR) {
                this.gameObject.transform.localPosition = new Vector3(33, -28, 0);
            } else if (starLevel == CardSampleManager.TWOSTAR) {
                this.gameObject.transform.localPosition = new Vector3(22, -28, 0);
            } else if (starLevel == CardSampleManager.THREESTAR) {
                this.gameObject.transform.localPosition = new Vector3(11, -28, 0);
            } else if (starLevel == CardSampleManager.FOURSTAR) {
                this.gameObject.transform.localPosition = new Vector3(0, -28, 0);
            }
        } else if (type == MagicWeaponManagerment.USEDBUMAGIC_ATTRSHOW) {
            if (starLevel == CardSampleManager.ONESTAR) {
                this.gameObject.transform.localPosition = new Vector3(33, -34, 0);
            } else if (starLevel == CardSampleManager.TWOSTAR) {
                this.gameObject.transform.localPosition = new Vector3(22, -34, 0);
            } else if (starLevel == CardSampleManager.THREESTAR) {
                this.gameObject.transform.localPosition = new Vector3(11, -34, 0);
            } else if (starLevel == CardSampleManager.FOURSTAR) {
                this.gameObject.transform.localPosition = new Vector3(0, -34, 0);
            }
        } else if (type == MagicWeaponManagerment.USEDBUMAGIC_AWARD) {
            if (starLevel == CardSampleManager.ONESTAR) {
                this.gameObject.transform.localPosition = new Vector3(33, -86, 0);
            } else if (starLevel == CardSampleManager.TWOSTAR) {
                this.gameObject.transform.localPosition = new Vector3(22, -86, 0);
            } else if (starLevel == CardSampleManager.THREESTAR) {
                this.gameObject.transform.localPosition = new Vector3(11, -86, 0);
            } else if (starLevel == CardSampleManager.FOURSTAR) {
                this.gameObject.transform.localPosition = new Vector3(0, -86, 0);
            }
        }
    }
}
