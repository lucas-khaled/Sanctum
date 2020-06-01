/***********************************************
                    Easy Scatter
	Copyright © 2014-2015 The Hedgehog Team
      http://www.thehedgehogteam.com/Forum/
		
	     The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System;

[System.Serializable]
public class ScatteredObject: System.ICloneable{

	public bool enable;
	public GameObject prefab;
	public bool isPrefab= false;

	public bool isScaleOption = false;

	public bool isScale = false;
	public bool uniformScale = true;
	public bool isScaleParticle = true;
	public bool isScaleLight = true;
	public float xMinScale =0.8f;
	public float xmaxScale =1.2f;

	public float yMinScale =0.8f;
	public float yMaxScale =1.2f;

	public float zMinScale =0.8f;
	public float zMaxScale =1.2f;
		

	public bool isRotationOption = false;
	public bool isRotationX = false;
	public float xMinRot =-25;
	public float xMaxRot =25;

	public bool isRotationY = false;
	public float yMinRot =-180;
	public float yMaxRot =180;

	public bool isRotationZ = false;
	public float zMinRot =-25;
	public float zMaxRot =25;

	public float offset=0;

	public bool isWait2delete = false;
	public bool showOption = false;

	public Texture2D preview;

	public object Clone(){
		return this.MemberwiseClone();
	}
	
	public static void ApplyRS( Transform tr, ScatteredObject scatter){

		// Scale
		if (scatter.isScale){
			if (scatter.uniformScale ){
				float scale = UnityEngine.Random.Range(scatter.xMinScale,scatter.xmaxScale);
				tr.localScale = new Vector3(scale,scale,scale);
				ScaleEffect( tr, scale, scatter.isScaleLight, scatter.isScaleParticle);
			}
			else{
				tr.localScale = new Vector3(UnityEngine.Random.Range(scatter.xMinScale,scatter.xmaxScale),UnityEngine.Random.Range(scatter.yMinScale,scatter.yMaxScale),UnityEngine.Random.Range(scatter.zMinScale,scatter.zMaxScale));
				ScaleEffect( tr, (tr.localScale.x+tr.localScale.y+tr.localScale.z)/3f, scatter.isScaleLight, scatter.isScaleParticle);
			}
		}
		
		// Rotation
		if ( scatter.isRotationX || scatter.isRotationY || scatter.isRotationZ){
			Vector3 rotation = tr.eulerAngles;
			if (scatter.isRotationX){
				rotation.x = UnityEngine.Random.Range( scatter.xMinRot,scatter.xMaxRot);
			}
			if (scatter.isRotationY){
				rotation.y = UnityEngine.Random.Range( scatter.yMinRot,scatter.yMaxRot);
			}
			if (scatter.isRotationZ){
				rotation.z = UnityEngine.Random.Range(  scatter.zMinRot,scatter.zMaxRot);
			}
			tr.eulerAngles = rotation;
		}
	}

	public static void ScaleEffect(Transform tr, float scaleFactor, bool isScaleLight, bool isScaleParticle){
		
		#if UNITY_EDITOR
		// Light
		if (isScaleLight){
			Light[] lights = tr.GetComponentsInChildren<Light>();
			foreach(Light light in lights){
				light.range *= scaleFactor;
			}
		}
		
		// Particles system
		if (isScaleParticle){
			ParticleSystem[] systems = tr.GetComponentsInChildren<ParticleSystem>();
			
			foreach (ParticleSystem system in systems)
			{
                            
                SerializedObject so = new SerializedObject(system);
                so.FindProperty("InitialModule.startSize.scalar").floatValue *= scaleFactor;
       
                so.FindProperty("InitialModule.startSpeed.scalar").floatValue *= scaleFactor;

                so.FindProperty("EmissionModule.rateOverDistance.scalar").floatValue *= scaleFactor;
                so.FindProperty("InitialModule.gravityModifier.scalar").floatValue *= scaleFactor;

                so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
				so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
				so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
				so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
				so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
				so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
				so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
				so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
				so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
				so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
				so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
				so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
				so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;
				
				so.ApplyModifiedProperties();
			}
		}
		#endif
	}
}
