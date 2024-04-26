using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {
	public enum Type{Clk, CClk}
	public Type rotateType;
	public float speed = 0.5f;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate (Vector3.forward, Mathf.Abs (speed) * (rotateType == Type.CClk ? 1 : -1));
	}
    
}
