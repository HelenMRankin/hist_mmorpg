using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UpdateSlider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeValue(float val) {
		this.gameObject.GetComponent<Text>().text=val.ToString ();
	}
}
