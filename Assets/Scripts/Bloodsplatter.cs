	
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.DecalSystem;

public class Bloodsplatter : MonoBehaviour {
	public GameObject test;
	
	
	// The prefab which contains the DS_Decals script with already set material and	
	// uv rectangles.	
	public GameObject decalsPrefab;	
	
	
	// The reference to the instantiated prefab's DS_Decals instance.	
	private DS_Decals m_Decals;	
	private Matrix4x4 m_WorldToDecalsMatrix;	
	
	
	// All the projectors that were created at runtime.	
	private List <DecalProjector> m_DecalProjectors = new List <DecalProjector> ();	
	
	
	// Intermediate mesh data. Mesh data is added to that one for a specific projector	
	// in order to perform the cutting.	
	private DecalsMesh m_DecalsMesh;	
	private DecalsMeshCutter m_DecalsMeshCutter;
	
	
	
	// The raycast hits a collider at a certain position. This value indicated how far we need to	
	// go back from that hit point along the ray of the raycast to place the new decal projector. Set	
	// this value to 0.0f to see why this is needed.	
	public float decalProjectorOffset = 0.5f;
	
	
	
	// The size of new decal projectors.	
	public Vector3 decalProjectorScale = new Vector3 (0.2f, 2.0f, 0.2f);	
	public float cullingAngle = 90.0f;	
	public float meshOffset = 0.001f;	
	
	
	// We iterate through all the defined uv rectangles. This one indices which index we are using at	
	// the moment.	
	private int m_UVRectangleIndex;

	public Vector3 target = new Vector3(0,0,0);
	public float radius = 0.5f;
	public float dist = 1;
	// Move on to the next uv rectangle index.	
	private void NextUVRectangleIndex () {		

		m_UVRectangleIndex = m_UVRectangleIndex + 1;
		
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length) {
			
			m_UVRectangleIndex = 0;
			
		}
		
	}
	
	
	
	private void Start () {
		
		
		
		// Instantiate the prefab and get its decals instance.		
		GameObject l_Instance = Instantiate (decalsPrefab) as GameObject;		
		m_Decals = l_Instance.GetComponentInChildren <DS_Decals> ();		
		
		
		if (m_Decals == null) {			
			Debug.LogError ("The 'decalsPrefab' does not contain a 'DS_Decals' instance!");			
		} else {			
			
			
			// Create the decals mesh (intermediate mesh data) for our decals instance.			
			// Further we need a decals mesh cutter instance and the world to decals matrix.			
			m_DecalsMesh = new DecalsMesh (m_Decals);			
			m_DecalsMeshCutter = new DecalsMeshCutter ();			
			m_WorldToDecalsMatrix = m_Decals.CachedTransform.worldToLocalMatrix;			
		}		
	}
	
	
	
	void Update () {		
		if (Input.GetButtonDown ("Fire1")) {			
			CastRay(this.gameObject);
			
		}
	}

	public void CastRay(GameObject goB)
		{
			Ray l_Ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			//evt lav med mindre radius(evt coroutine derstart småt og bliver større)??
		RaycastHit[] sphereHitDown = Physics.SphereCastAll(new Vector3(goB.transform.position.x, goB.transform.position.y +1,goB.transform.position.z), radius, -goB.transform.up , dist);
//			RaycastHit[] sphereHitRight = Physics.SphereCastAll(goB.transform.position , radius, goB.transform.right, dist);
//			RaycastHit[] sphereHitLeft = Physics.SphereCastAll(goB.transform.position , radius, -goB.transform.right, dist);
//			RaycastHit[] sphereHitForward = Physics.SphereCastAll(goB.transform.position , radius, goB.transform.forward, dist);
//			RaycastHit[] sphereHitBack = Physics.SphereCastAll(goB.transform.position, radius, -goB.transform.forward, dist);
//		Instantiate(test,new Vector3(goB.transform.position.x, goB.transform.position.y +1,goB.transform.position.z),Quaternion.identity);
			//			RaycastHit hitDown;
			//			if(Physics.Raycast(this.transform.position, -transform.up,out hitDown, 100f))
			//			{ 
			//					CheckRay(hitDown,l_Ray);
			//			}
			
//			foreach(RaycastHit l_RaycastHit in sphereHitRight)
//			{
//				CheckRay(l_RaycastHit, l_Ray);
//			}
			foreach(RaycastHit l_RaycastHit in sphereHitDown)
			{
				if(l_RaycastHit.collider.gameObject.tag == "bloodplane")
				{
					CheckRay(l_RaycastHit, l_Ray);
				}
			}
//			foreach(RaycastHit l_RaycastHit in sphereHitLeft)
//			{
//				CheckRay(l_RaycastHit, l_Ray);
//			}
//			foreach(RaycastHit l_RaycastHit in sphereHitForward)
//			{
//				CheckRay(l_RaycastHit, l_Ray);
//			}
//			foreach(RaycastHit l_RaycastHit in sphereHitBack)
//			{
//				CheckRay(l_RaycastHit, l_Ray);
//			}
			
			
			target = this.transform.position;
			
			
		}

	void CheckRay(RaycastHit l_RaycastHit, Ray l_Ray)
	{

			if (m_DecalProjectors.Count >= 50) {					
				
				// If there are more than 50 projectors, we remove the first one from					
				// our list and certainly from the decals mesh (the intermediate mesh					
				// format). All the mesh data that belongs to this projector will					
				// be removed.					
				DecalProjector l_DecalProjector = m_DecalProjectors [0];					
				m_DecalProjectors.RemoveAt (0);					
				m_DecalsMesh.RemoveProjector (l_DecalProjector);					
			}
			
			
			
			// Calculate the position and rotation for the new decal projector.			
//			Vector3 l_ProjectorPosition = l_RaycastHit.point - (decalProjectorOffset * l_Ray.direction.normalized);				
			Vector3 l_ForwardDirection = Camera.main.transform.up;				
			Vector3 l_UpDirection = - Camera.main.transform.forward;				
//			Quaternion l_ProjectorRotation = Quaternion.LookRotation (l_ForwardDirection, l_UpDirection);		
//			Quaternion l_ProjectorRotation = Quaternion.LookRotation (-		
			Vector3 l_ProjectorPosition = l_RaycastHit.point;
			Quaternion l_ProjectorRotation = l_RaycastHit.transform.localRotation;
			
			
			// We hit a collider. Next we have to find the mesh that belongs to the collider.					
			// That step depends on how you set up your mesh filters and collider relative to					
			// each other in the game objects. It is important to have a consistent way in order					
			// to have a simpler implementation.					
			
			MeshCollider l_MeshCollider = l_RaycastHit.collider.GetComponent <MeshCollider> ();					
			MeshFilter l_MeshFilter = l_RaycastHit.collider.GetComponent <MeshFilter> ();					
			
			if (l_MeshCollider != null || l_MeshFilter != null) {						
				Mesh l_Mesh = null;						
				if (l_MeshCollider != null) {							
					
					
					// Mesh collider was hit. Just use the mesh data from that one.							
					l_Mesh = l_MeshCollider.sharedMesh;							
				} else if (l_MeshFilter != null) {							
					
					
					// Otherwise take the data from the shared mesh.							
					l_Mesh = l_MeshFilter.sharedMesh;							
				}
				
				
				if (l_Mesh != null) {							
					
					
					// Create the decal projector.							
					DecalProjector l_DecalProjector = new DecalProjector (l_ProjectorPosition, l_ProjectorRotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
					
					
					
					// Add the projector to our list and the decals mesh, such that both are							
					// synchronized. All the mesh data that is now added to the decals mesh							
					// will belong to this projector.							
					m_DecalProjectors.Add (l_DecalProjector);							
					m_DecalsMesh.AddProjector (l_DecalProjector);							
					
					
					// Get the required matrices.							
					Matrix4x4 l_WorldToMeshMatrix = l_RaycastHit.collider.renderer.transform.worldToLocalMatrix;							
					Matrix4x4 l_MeshToWorldMatrix = l_RaycastHit.collider.renderer.transform.localToWorldMatrix;	 						
					
					
					// Add the mesh data to the decals mesh, cut and offset it before we pass it							
					// to the decals instance to be displayed.							
					m_DecalsMesh.Add (l_Mesh, l_WorldToMeshMatrix, l_MeshToWorldMatrix);                       
					m_DecalsMeshCutter.CutDecalsPlanes (m_DecalsMesh);							
					m_DecalsMesh.OffsetActiveProjectorVertices ();							
					m_Decals.UpdateDecalsMeshes (m_DecalsMesh);							
					
					
					// For the next hit, use a new uv rectangle. Usually, you would select the uv rectangle							
					// based on the surface you have hit.							
					NextUVRectangleIndex ();
					
				}
				
			}

	}
		
}
	
