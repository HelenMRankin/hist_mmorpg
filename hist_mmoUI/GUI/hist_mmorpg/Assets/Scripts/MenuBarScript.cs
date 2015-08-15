using UnityEngine;
using System.Collections;

public class MenuBarScript : MonoBehaviour {

	void Awake() {
		if(GameStateManager.gameState.menuBar!=null) {
			Destroy (this.gameObject);
		}
		else {
			GameStateManager.gameState.menuBar=this.gameObject;
			DontDestroyOnLoad(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		//GameStateManager.gameState.menuBar=this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MenuBarClick(string optionChosen) {
		Debug.Log ("Clicked: "+optionChosen);
		GameStateManager.MainMenuClick (optionChosen);
	}


}
