using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
	public GameObject mainMenu;
	public GameObject loginMenu;
	// Use this for initialization
	void Start () {
		MainMenuState();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Enter Main Menu State
	public void MainMenuState() {
		loginMenu.SetActive (false);
		mainMenu.SetActive (true);
	}
	
	public void LogInState() {
		mainMenu.SetActive (false);
		loginMenu.SetActive (true);
	}
}
