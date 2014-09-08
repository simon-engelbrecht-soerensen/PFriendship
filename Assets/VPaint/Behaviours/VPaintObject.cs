using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Valkyrie.VPaint;

public class VPaintObjectNotDynamicException : Exception {
	public override string Message {
		get {
			return "Method called requires VPaint Object to be dynamic. Set VPaintObject.isDynamic to true before calling this method.";
		}
	}
}

public delegate Color VPaintObjectPositionalModifier (Color color, float distance);

[AddComponentMenu("VPaint/VPaint Object")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Renderer))]
public class VPaintObject : MonoBehaviour, IVPaintIdentifier //, IVertexPaintable 
{
	
	public static List<VPaintObject> all = new List<VPaintObject>();
	public static List<VPaintObject> OverlapSphere (Vector3 position, float radius) 
	{	
		float sqrRadius = radius * radius;
		List<VPaintObject> selected = new List<VPaintObject>();

		foreach(VPaintObject vc in all)
		{
			if(!vc) continue;

			if(vc.renderer.bounds.SqrDistance(position) < sqrRadius)
				selected.Add(vc);
		}
		return selected;
	}	
	
	[HideInInspector] 
	public Mesh _mesh;
	[NonSerialized]
	public Mesh _meshNonSerialized; //non serialized reference because of that asshole Undo
	
	[HideInInspector] 
	public Material originalMaterial;
	
	[HideInInspector] 
	public Mesh originalMesh;
	
	[NonSerialized] public Color[] colorsBuilder;
	[NonSerialized] public float[] transparencyBuilder;
	[NonSerialized] public int index;
	
	[NonSerialized]	public Color[] myColors;
	[NonSerialized] public Vector3[] myVertices;
	
	[HideInInspector] 
	public MeshCollider editorCollider;
	
	[SerializeField] bool _isDynamic;
	public bool isDynamic 
	{
		get{ return _isDynamic; }
		set{
			if(value != _isDynamic)
			{
				if(!value && editorCollider)
					GameObject.Destroy(editorCollider.gameObject);
				else {
					CreateDynamicCollider();
				}
					
				value = _isDynamic;
			}
		}
	}
	void CreateDynamicCollider ()
	{
		GameObject go = new GameObject(name + " Collider");
		go.hideFlags = HideFlags.HideInHierarchy;
		editorCollider = go.AddComponent<MeshCollider>();
		editorCollider.sharedMesh = originalMesh;
	}
	
	public void OnApplicationQuit () {
		OnDestroy();
	}
	
	public void OnDestroy () {
		if(_mesh) Destroy(_mesh);
		all.Remove(this);
		if(editorCollider) GameObject.Destroy(editorCollider.gameObject);
	}
	
	public void Awake ()
	{
		if(_isDynamic)
		{
			CreateDynamicCollider();
		}
	}
	
	/*IVPaintObject Interface*/
	public Color[] GetDefaultColors ()
	{
		return GetMeshInstance().colors;
	}
	public Vector3[] GetVertices ()
	{
		return GetMeshInstance().vertices;
	}
	public void SetColors (Color[] colors)
	{
		Mesh m = GetMeshInstance();
		if(colors.Length != myVertices.Length)
		{
			Debug.LogWarning("Colors length of " + name + " is different than vertices length");
			return;
		}
		for(int i = 0; i < colors.Length; i++)
			myColors[i] = colors[i];
		m.colors = myColors;
	}
	
	public bool IsEqualTo (IVPaintIdentifier obj)
	{
		return obj == this;
//		var vc = obj as VPaintObject;
//		if(!vc)
//		{
//			return false;
//		}
//		if(vc == this) return true;
//		return false;
	}
	/* */
	
	public Bounds GetBounds ()
	{
		if(editorCollider) return editorCollider.bounds;
		return renderer.bounds;
	}
	
	public void ApplyColorsBuilder ()
	{
		if(colorsBuilder == null) return;
		if(transparencyBuilder == null) return;
		
		Mesh m = GetMeshInstance();
		m.colors = colorsBuilder;
		myColors = colorsBuilder;
		colorsBuilder = null;
		transparencyBuilder = null;
	}
	
	public Mesh GetMeshInstance ()
	{
		if(!_mesh){
			if(_meshNonSerialized)
			{
				_mesh = _meshNonSerialized;
			}
			else
			{
				MeshFilter mf = GetComponent<MeshFilter>();
				if(!mf) return null;
				if(!mf.sharedMesh && !originalMesh) return null;
				if(originalMesh) mf.sharedMesh = originalMesh;
				originalMesh = mf.sharedMesh;
				var vdc = GetComponent<VertexDataCache>();
				if(vdc) _mesh = vdc.GetMeshInstance();
				else _mesh = GameObject.Instantiate(mf.sharedMesh) as Mesh;
				mf.sharedMesh = _mesh;
				myVertices = _mesh.vertices;
				var cols = _mesh.colors;
				if(cols == null || cols.Length != myVertices.Length)
				{
					Color[] colors = new Color[myVertices.Length];
					myColors = colors;
					_mesh.colors = colors;
				}
				if(_mesh.uv2.Length == 0)
				{
					_mesh.uv2 = _mesh.uv;
				}
//				if(_mesh.normals.Length == 0)
//					_mesh.RecalculateNormals();
				_mesh.RecalculateBounds();
			}
		}
		_meshNonSerialized = _mesh;
		if(myVertices == null) myVertices = _mesh.vertices;
		if(myColors == null) myColors = _mesh.colors;
		return _mesh;
	}
	
	public void ResetInstances ()
	{
		if(_mesh) GameObject.DestroyImmediate(_mesh);
		MeshFilter mf = GetComponent<MeshFilter>();
		if(!mf) return;
		mf.sharedMesh = originalMesh;
		ResetMaterial();
	}
	
	public void SetTangents (Color[] colors)
	{
		Mesh m = GetMeshInstance();
		if(colors.Length != myVertices.Length)
		{
			Debug.LogWarning("Colors length of " + name + " is different than vertices length");
			return;
		}
		Vector4[] tgts = new Vector4[colors.Length];
		for(int i = 0; i < tgts.Length; i++)
			tgts[i] = (Vector4)colors[i];
		m.tangents = tgts;
	}
	
	public void FloodColors (Color color)
	{
		var m = GetMeshInstance();
		var c = m.colors;
		for(int i = 0; i < c.Length; i++)
			c[i] = color;
		m.colors = c;
	}
		
	public void ApplyPositionalModifier (Vector3 worldPosition, VPaintObjectPositionalModifier modifier) 
	{
		Mesh m = GetMeshInstance();
		
		Vector3[] vertices = myVertices;
		Color[] colors = myColors;
		if(colors == null) colors = m.colors;
		
		var len = vertices.Length;
		
		for(int i = 0; i < len; i++)
		{
			Vector3 v = transform.TransformPoint(vertices[i]);
			float distance = Vector3.Distance(v, worldPosition);			
			colors[i] = modifier(colors[i], distance);
		}

		m.colors = colors;
		myColors = colors;
	}
	
	public IEnumerator ApplyPositionalModifierAsync (Vector3 worldPosition, VPaintObjectPositionalModifier modifier, float time)
	{
		Mesh m = GetMeshInstance();
		
		Vector3[] vertices = myVertices;
		Color[] colors = myColors;
		if(colors == null) colors = m.colors;
		
		var len = vertices.Length;
		
		Color[] target = new Color[len];
		for(int i = 0; i < len; i++)
		{
			Vector3 v = transform.TransformPoint(vertices[i]);
			float distance = Vector3.Distance(v, worldPosition);			
			target[i] = modifier(colors[i], distance);
		}
		
		var routine = VPaintUtility.LerpColors(this, colors, target, time);
		while(routine.MoveNext()) yield return null;
	}
	
	public void ApplyColorSpherical (Color color, Vector3 worldPosition, float radius, float falloff, float strength)
	{		
		ApplyPositionalModifier(worldPosition,
		(c, d)=>
		{
			if(radius < d) return c;
			
			float f = Mathf.Pow(1 - (d / radius), falloff) * strength;
			return Color.Lerp(c, color, f);
			
		});
	}
	
	public void ApplyAlphaSpherical (float alpha, Vector3 worldPosition, float radius, float falloff, float strength)
	{		
		ApplyPositionalModifier(worldPosition,
		(c, d)=>
		{
			if(radius < d) return c;
			
			float f = Mathf.Pow(1 - (d / radius), falloff) * strength;
			
			c.a = Mathf.Lerp(c.a, alpha, f);
			
			return c;
		});
	}
	
	public void SetInstanceMaterial (Material m)
	{
		if(!renderer) return;
		if(!originalMaterial) originalMaterial = renderer.sharedMaterial;
		renderer.sharedMaterial = m;
	}
	public void ResetMaterial ()
	{
		if(renderer && originalMaterial) renderer.sharedMaterial = originalMaterial;
		originalMaterial = null;
	}
	
}