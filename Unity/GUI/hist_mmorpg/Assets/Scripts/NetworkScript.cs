using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Send(ProtoMessage message) {
	}
}
