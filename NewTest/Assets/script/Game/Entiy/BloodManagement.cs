using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodManagement {
	const int MINERAL_NUM = 2;
	/* static methods */
    public static BloodManagement Instance {
		get {
            BloodManagement manager = SingleManager.Instance.getObj("BloodManagement") as BloodManagement;
			return manager;
		}
	}
    PrizeSample[] EvoPrizes;

    public PrizeSample[] prizes {
        get { return EvoPrizes; }
        set { EvoPrizes = value; }
    }

    public void ClearPrizes() {
        EvoPrizes = null;
    }

    public bool HavePrizes() {
        if (EvoPrizes != null && EvoPrizes.Length > 0) {
            return true;
        }
        return false;
    }

}
