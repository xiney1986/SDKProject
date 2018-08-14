using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// GM动态修改内存配置接口
/// </summary>
public interface IGMUpdateConfigManager {

	Hashtable getSamples ();
	void createSample (int sid);
}

