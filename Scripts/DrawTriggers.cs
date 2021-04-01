using UnityEngine;
using System.Collections;

public class DrawTriggers : MonoBehaviour {
	
	public Color triggerColor;
	
	void OnDrawGizmos() {
		
		Gizmos.color = triggerColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero,Vector3.one);   
	}
	 
	void OnDrawGizmosSelected() {
		
		Gizmos.color = triggerColor;
		triggerColor.a = .25f;    
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero,Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero,Vector3.one); 
	}
}