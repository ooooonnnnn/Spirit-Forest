using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiationData
{
	//nessecary data to instantiate a game object, used by GenerationManager
	public GameObject go;
	public Vector3 pos;
	public Quaternion quaternion;

	public InstantiationData(GameObject go, Vector3 pos, Quaternion quaternion)
	{
		this.go = go;
		this.pos = pos;
		this.quaternion = quaternion;
	}
	
	public InstantiationData(){}
}
