using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Valkyrie.VPaint;

public class VPaintAddColorerWindow : VPaintWindowBase
{
	public override bool LockSelection ()
	{
		return true;
	}
	public override bool OverrideTool ()
	{
		return true;
	}
	public override bool CloseOnInvalid ()
	{
		return true;
	}
	
	public override void OnValidatedEnable ()
	{
		EditorApplication.hierarchyWindowChanged += RedrawHierarchy;
		RedrawHierarchy();
	}
	public override void OnValidatedDisable ()
	{
		EditorApplication.hierarchyWindowChanged -= RedrawHierarchy;
	}
	
	Vector2 scrollPosition;
	public override void OnValidatedGUI ()
	{
		if(hierarchy == null || objectInfo == null) RedrawHierarchy();
		GUILayout.Space(15);
		
		VPaintGUIUtility.BeginColumnView(position.width-48);
		
		VPaintGUIUtility.DrawColumnRow(24, 
		()=>{
			GUILayout.FlexibleSpace();
			GUILayout.Label("Select Objects to Add");
			GUILayout.FlexibleSpace();
		});
		VPaintGUIUtility.DrawColumnRow(1, 
		()=>{
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Select All"))
			{
				selectedObjects = new List<ObjectInfo>();
				foreach(var kvp in objectInfo)
				{
					kvp.Value.foldout = true;
					selectedObjects.Add(kvp.Value);
				}
			}
			if(GUILayout.Button("Contract All"))
			{
				foreach(var kvp in objectInfo)
				{
					kvp.Value.foldout = false;
				}
			}
			if(GUILayout.Button("Expand All"))
			{
				foreach(var kvp in objectInfo)
				{
					kvp.Value.foldout = true;
				}
			}		
			GUILayout.FlexibleSpace();
		});
		
		GUILayout.Space(10);
		
		VPaintGUIUtility.BeginColumnView(position.width-64);
		
		VPaintGUIUtility.Area(0, ()=>{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			
			foreach(var o in hierarchy)
			{
				RecursiveHierarchyGUI(o, 0);
			}
					
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.EndScrollView();
		});
			
		VPaintGUIUtility.columnViewBoxCount = 1;
		
		bool enabledCache = GUI.enabled;
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUI.enabled = selectedObjects.Count != 0;
		if(GUILayout.Button("Add"))
		{
			List<VPaintObject> objectsToAdd = new List<VPaintObject>();
			foreach(var obj in selectedObjects)
			{
				GetColorersRecursive (obj, objectsToAdd);
			}
			VPaint.Instance.AddColorers(objectsToAdd);
			EditorApplication.delayCall += Close;
		}
		EditorGUILayout.EndHorizontal();
		GUI.enabled = enabledCache;
	}
	
	void GetColorersRecursive (ObjectInfo obj, List<VPaintObject> colorers)
	{
		if(obj.colorer && !colorers.Contains(obj.colorer))
		{
			colorers.Add(obj.colorer);
		}
		foreach(var c in obj.children)
		{
			GetColorersRecursive(c, colorers);
		}
	}
	
	void RecursiveHierarchyGUI (ObjectInfo obj, float indent)
	{
		bool isContained = VPaint.Instance.ContainsColorer(obj.colorer);
		bool selected = selectedObjects.Contains(obj);
		if(selected)
		{
			if(EditorGUIUtility.isProSkin)
				VPaintGUIUtility.columnViewBoxCount = 2;
			else
				VPaintGUIUtility.columnViewBoxCount = 1;
		}
		else
		{
			if(EditorGUIUtility.isProSkin)
				VPaintGUIUtility.columnViewBoxCount = 1;
			else
				VPaintGUIUtility.columnViewBoxCount = 0;
		}
		
		VPaintGUIUtility.DrawColumnRow(24, 
		(mainRect)=>{
			EditorGUIUtility.AddCursorRect(mainRect, MouseCursor.Link);
			
			Rect foldoutRect = EditorGUILayout.BeginVertical(GUILayout.Width(10));
			foldoutRect.width += 10;
			GUILayout.Label("");
			EditorGUILayout.EndVertical();
			if(obj.children.Count != 0){
				if(foldoutRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					obj.foldout = !obj.foldout;
					Event.current.Use();
				}
				EditorGUI.Foldout(foldoutRect, obj.foldout, GUIContent.none);
			}
			
			GUILayout.Space(indent);
			GUILayout.Label(obj.transform.name);
			
			if(isContained)
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("[Added]");
			}
			else if(obj.colorer)
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("[Colorer]");
			}
			else if(obj.transform.GetComponent<MeshFilter>() && obj.transform.GetComponent<MeshRenderer>())
			{
				GUILayout.FlexibleSpace();
				if(VPaintGUIUtility.FoldoutMenu())
				{
					var menu = new GenericMenu();
//					if(obj.transform.GetComponent<MeshFilter>() && obj.transform.GetComponent<MeshRenderer>())
//					{
						menu.AddItem(new GUIContent("Add Colorer"), false, ()=>{
							obj.colorer = obj.transform.gameObject.AddComponent<VPaintObject>();
						});
//					}
//					else
//					{
//						menu.AddDisabledItem(new GUIContent("Add Colorer (Needs MeshFilter and MeshRenderer)"));
//					}
					menu.ShowAsContext();
				}
			}
			
			if(mainRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				if(Event.current.shift)
				{
					if(!selected)
					{
						selectedObjects.Add(obj);
					}
					else
					{
						selectedObjects.Remove(obj);
					}
				}
				else
				{
					selectedObjects = new List<ObjectInfo>();
					selectedObjects.Add(obj);
				}
				EditorGUIUtility.PingObject(obj.transform);
				Event.current.Use();
			}
		});
		if(obj.foldout)
		{
			foreach(var o in obj.children)
			{
				RecursiveHierarchyGUI(o, indent + 12);
			}
		}
	}
	
	class ObjectInfo
	{
		public Transform transform;
		public VPaintObject colorer;
		public bool foldout;
		
		public List<ObjectInfo> children;
	}
	Dictionary<Transform, ObjectInfo> objectInfo;
	List<ObjectInfo> hierarchy;
	List<ObjectInfo> selectedObjects;
	void RedrawHierarchy ()
	{
		objectInfo = new Dictionary<Transform, ObjectInfo>();
		hierarchy = new List<ObjectInfo>();
		selectedObjects = new List<ObjectInfo>();
		
		var all = GameObject.FindSceneObjectsOfType(typeof(Transform));
		foreach(var a in all)
		{
			var transform = a as Transform;
			GetOrAddObjectInfo(transform);
		}
		
		SortHierarchyRecursive(hierarchy);
	}
	
	void SortHierarchyRecursive (List<ObjectInfo> info)
	{
		info.Sort((x,y)=>{return String.Compare(x.transform.name, y.transform.name);});
		foreach(var obj in info)
		{
			SortHierarchyRecursive(obj.children);
		}
	}
	
	ObjectInfo GetOrAddObjectInfo (Transform transform)
	{
		if(objectInfo.ContainsKey(transform)) return objectInfo[transform];
			
		ObjectInfo info = new ObjectInfo();
		info.transform = transform;
		info.colorer = transform.GetComponent<VPaintObject>();
		info.children = new List<ObjectInfo>();
		
		objectInfo.Add(transform, info);
		
		if(transform.parent)
		{
			GetOrAddObjectInfo(transform.parent).children.Add(info);
		}
		else
		{
			hierarchy.Add(info);
		}
		
		return info;
	}
}