using UnityEngine;
using System.Collections;

public class SetResolution : MonoBehaviour {

	public int width;
	public int height;

	
	void Start ()
	{
		Screen.SetResolution (width, height, true);
	}
}
