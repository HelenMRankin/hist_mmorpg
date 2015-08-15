using UnityEngine;
//using UnityEditor.Events;
using System.Collections;
using UnityEngine.UI;
using System;
public class RowClickDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var button = this.gameObject.GetComponent<Button>();
		ListDisplay listDisplay =FindObjectOfType<ListDisplay>();

		button.onClick.AddListener(delegate { listDisplay.getDetails(button.name); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
