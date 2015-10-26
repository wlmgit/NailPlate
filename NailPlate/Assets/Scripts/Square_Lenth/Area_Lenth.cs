using UnityEngine;
using System.Collections;
using VectorList;
using System;
using System.Collections.Generic;

public class Area_Lenth{
	
	public float getLenth(Vector3[] pointPos)
	{
		float lenth = 0;
		for (int i=0; i<pointPos.Length-1; i++) {
			lenth+=Vector3.Distance(pointPos[i],pointPos[i+1]);
		}
		lenth = lenth /GameControl.EdgeLenth;
		return lenth;
	}
//	public float getArea(Vector3[] pointPos)
//	{
//		ScanLine scan = new ScanLine ();
//	}
}
