/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScatterBrush{

	public enum AlignVector {Left,Front,right,Back,Up,down};

	public float size=0;
	public int amount=1;
	public float flux=5;
	public float minSlope=0;
	public float maxSlope=90;

	public LayerMask pickableLayer = -1;//1<<0;

	public bool align2View = false;
	public bool align2Surface = false;
	public bool align2Previous = false;
	public bool loop = false;
	public AlignVector alignVector;

	public bool overRide = false;
	public int layer=0;
	public string tag="Untagged";

	public bool lightmapStatic=false;
	public bool batchingStatic=false;
	public bool occludeeStatic=false;

	public bool advanced = false;
	
}
