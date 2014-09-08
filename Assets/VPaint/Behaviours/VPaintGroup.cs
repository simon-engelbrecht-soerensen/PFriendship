using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Valkyrie.VPaint;

public enum AutoApplySchedule
{
	OnStart,
	OnAwake,
	Never
}

[AddComponentMenu("VPaint/VPaint Group")]
public class VPaintGroup : MonoBehaviour, IVPaintable
{	
//	[HideInInspector]
	public List<VPaintObject> colorers = new List<VPaintObject>();
	
//	[HideInInspector] 
	public VPaintLayerStack layerStack = new VPaintLayerStack();
	
	public AutoApplySchedule autoApplySchedule = AutoApplySchedule.OnStart;
	public bool autoLoadInEditor = true;
	
	[NonSerialized] public VPaintLayer paintLayer;
	
	public VPaintLayerStack GetLayerStack ()
	{
		return layerStack;
	}
	public VPaintObject[] GetVPaintObjects ()
	{
		for(int i = 0; i < colorers.Count; i++)
		{
			if(!colorers[i]) colorers.RemoveAt(i--);
		}
		return colorers.ToArray();
	}
	
	public VPaintLayer GetBaseLayer ()
	{
		return new VPaintLayer();
	}
	
	public void AddColorer (VPaintObject vc)
	{
		if(colorers.Contains(vc)) return;
		colorers.Add(vc);
	}
	public void RemoveColorer (VPaintObject vc)
	{
		colorers.Remove(vc);
	}
	
	void Awake ()
	{
		paintLayer = layerStack.GetMergedLayer();
		if(autoApplySchedule == AutoApplySchedule.OnAwake) Apply();
	}
	
	void Start ()
	{
		if(autoApplySchedule == AutoApplySchedule.OnStart) Apply();
	}
	
	[ContextMenu("Apply")]
	public void Apply ()
	{				
		if(paintLayer == null) paintLayer = layerStack.GetMergedLayer();
		foreach(var pd in paintLayer.paintData)
		{
			var vpd = pd.vpaintObject;
			if(!vpd) continue;
			vpd.SetColors(pd.colors);
		}
	}
	
	public void ApplyToTangents ()
	{
		List<VPaintObject> vcs = new List<VPaintObject>();
		foreach(var t in colorers)
		{
			var cols = t.GetComponentsInChildren<VPaintObject>();
			foreach(var vc in cols)
			{
				if(!vcs.Contains(vc)) vcs.Add(vc);
			}
		}
		
		foreach(var vc in vcs)
		{
			var data = paintLayer.Get(vc);
			if(data == null) continue;
			vc.SetTangents(data.colors);
		}
	}
	
	public IEnumerator BlendTo (VPaintGroup target, float time)
	{
		var myBaseLayer = paintLayer;
		var targetBaseLayer = target.paintLayer;
		
		List<IEnumerator> routines = new List<IEnumerator>();
		foreach(var obj in GetVPaintObjects())
		{
			if(!target.colorers.Contains(obj)) continue;
			
			int colorLength = obj.GetMeshInstance().colors.Length;
			
			var baseData = myBaseLayer.Get(obj);
			Color[] baseColors = null;
			if(baseData == null){
				baseColors = new Color[colorLength];
			}
			else baseColors = baseData.colors;
			
			var targetData = targetBaseLayer.Get(obj);
			Color[] targetColors = null;
			if(targetData == null){
				targetColors = new Color[colorLength];
			}
			else targetColors = targetData.colors;
			
			routines.Add(VPaintUtility.LerpColors (obj, baseColors, targetColors, time));
		}
		
		while(true)
		{
			bool b = true;
			foreach(var r in routines)
			{
				if(r.MoveNext()) b = false;
			}
			if(b) break;
			yield return null;
		}
	}
	
	
#if UNITY_EDITOR
	public void CleanupData ()
	{
		var paintObjects = GetVPaintObjects();
		var list = new List<IVPaintIdentifier>(paintObjects);
		layerStack.Sanitize(list);
	}
#endif
}
