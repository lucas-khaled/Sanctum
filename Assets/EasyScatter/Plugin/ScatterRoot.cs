/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterRoot : MonoBehaviour {

	[HideInInspector]
	public List<ScatteredObject> scatteredObjects;

	void Awake(){
		scatteredObjects.Clear();
	}
}
