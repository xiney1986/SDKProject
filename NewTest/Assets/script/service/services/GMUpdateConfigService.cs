using UnityEngine;
using System;
using System.Collections;
using System.Reflection;


/// <summary>
/// gm修改配置表
/// </summary>
public class GMUpdateConfigService : BaseFPort {
	public GMUpdateConfigService () {

	}

	public override void read ( ErlKVMessage message ) {
		ErlArray arr		= message.getValue ("msg") as ErlArray;
		if (arr == null) return;
		for (int i = 0; i < arr.Value.Length; i++) {
			ErlArray data		= arr.Value[i] as ErlArray;
			ErlArray head		= data.Value[0] as ErlArray;
			ErlList body		= data.Value[1] as ErlList;
			string className	= head.Value[0].getValueString ();
			int key				= StringKit.toInt (head.Value[1].getValueString ());

			for (int j = 0; j < body.Value.Length; j++) {
				string fieldName	= (body.Value[j] as ErlArray).Value[0].getValueString ();
				string fieldValue	= (body.Value[j] as ErlArray).Value[1].getValueString ();

				Type type			= Type.GetType (className);
				if (type == null) {
					Debug.LogError ("class no find : " + className);
					return;
				}

				IGMUpdateConfigManager inst	= type.GetProperty ("Instance", BindingFlags.Static | BindingFlags.Public).GetValue (null, null) as IGMUpdateConfigManager;
				if (inst == null) {
					Debug.LogError ("class no instance" + className);
					return;
				}

				if (!inst.getSamples ().ContainsKey (key)) {
					inst.createSample (key);
				}
				MethodInfo method			= inst.getSamples ()[key].GetType ().GetMethod ("parse_" + fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method == null) {
					Debug.LogError ("no find field  GMUpdate Method: perse_" + fieldName + "() (node: this method need private)");
					return;
				}
				method.Invoke (inst.getSamples()[key], new object[] { fieldValue });
				method	= null;
				inst	= null;
			}
		}
	}




}
