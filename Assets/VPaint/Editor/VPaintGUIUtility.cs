using UnityEditor;
using UnityEngine;
using System;

namespace Valkyrie.VPaint
{
	public class VPaintGUIUtility
	{		
		public static bool FoldoutMenu ()
		{
			bool b = false;
			Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(16));
			EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
			GUILayout.Label("");
			EditorGUILayout.EndHorizontal();
			if(r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				b = true;
				Event.current.Use();
			}
			EditorGUI.Foldout(r, true, GUIContent.none);
			return b;
		}
		
		public static Rect BoxArea (float margin, float border, int boxCount, Action contents)
		{
			return BoxArea(Vector4.one * margin, Vector4.one * border, boxCount, contents);
		}
		
		public static Rect BoxArea (Vector2 margin, Vector2 border, int boxCount, Action contents)
		{
			return BoxArea(new Vector4(margin.x, margin.x, margin.y, margin.y), new Vector4(border.x, border.x, border.y, border.y), boxCount, contents);
		}
		
		public static Rect BoxArea (Vector4 margin, Vector4 border, int boxCount, Action contents)
		{
			return Area(margin, ()=>{
				Rect r = EditorGUILayout.BeginHorizontal();
				int bc = EditorGUIUtility.isProSkin ? boxCount : (int)Mathf.Pow(boxCount, 2);
				for(int i = 0; i < bc; i++) GUI.Box(r, GUIContent.none);
				GUILayout.Space(border.x);
				EditorGUILayout.BeginVertical();
				GUILayout.Space(border.z);
				contents();
				GUILayout.Space(border.w);
				EditorGUILayout.EndVertical();
				GUILayout.Space(border.y);
				EditorGUILayout.EndHorizontal();
			});
		}
		
		public static Rect Area (float margin, Action contents)
		{
			return Area(Vector4.one * margin, contents);
		}
		
		public static Rect Area (Vector2 margin, Action contents)
		{
			return Area(new Vector4(margin.x, margin.x, margin.y, margin.y), contents);
		}
		
		public static Rect Area (Vector4 border, Action contents)
		{
			Rect r = EditorGUILayout.BeginHorizontal();
			GUILayout.Space(border.x);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(border.z);
			contents();
			GUILayout.Space(border.w);
			EditorGUILayout.EndVertical();
			GUILayout.Space(border.y);
			EditorGUILayout.EndHorizontal();
			return r;
		}
		
		static float columnWidth;
		public static int columnViewBoxCount = 1;
		public static void BeginColumnView (float width)
		{
			columnWidth = width;
		}
		public static Rect DrawColumnRow (float height, params Action[] items)
		{
			Action<Rect>[] rectItems = new Action<Rect>[items.Length];
			for(int i = 0; i < rectItems.Length; i++)
			{
				Action a = items[i];
				rectItems[i] = (r)=>{ a(); };
			}
			return DrawColumnRow(height, rectItems);
		}
		
		public static Rect DrawColumnRow (float height, params Action<Rect>[] items)
		{
			float width = columnWidth/items.Length - 3*(items.Length-1) - ((items.Length % 2) * 3);// - 2*items.Length;
			
			Rect r = EditorGUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();
			
			for(int i = 0; i < items.Length; i++)
			{
				BoxArea(0, 4, columnViewBoxCount, ()=>{
					Rect rect = EditorGUILayout.BeginVertical(GUILayout.Height(height), GUILayout.Width(width));
					GUILayout.FlexibleSpace();
					EditorGUILayout.BeginHorizontal();
					items[i](rect);
					EditorGUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
				});
				if(i != items.Length-1) GUILayout.Space(4);
			}
			
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(2);
			
			return r;
		}
		
	}
}
