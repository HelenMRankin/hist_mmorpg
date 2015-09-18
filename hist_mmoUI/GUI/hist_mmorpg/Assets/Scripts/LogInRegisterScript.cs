using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
public class LogInRegisterScript : MonoBehaviour {
	public GameObject emailErrorText;
	public Button LogInButton;
	public InputField emailField;
	public InputField passwordField;
	public Text passErrorText;
	public NetworkScript networkScript;
	private bool passOK;
	private bool usrOK;
	// Use this for initialization
	void Start () {
		emailErrorText.SetActive (false);
		passErrorText.enabled=false;
		LogInButton.interactable=false;
		passOK=false;
		usrOK=false;
	}

	//Verify an email address, if invalid disable login and show message
	public void verifyEmail() {
		string email = emailField.textComponent.text;
		bool isValid =  Regex.IsMatch(email,"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$");
		if(!isValid) {
			emailErrorText.SetActive(true);
			LogInButton.interactable=false;
			usrOK=false;
		}
		else {
			emailErrorText.SetActive (false);
			usrOK=true;
			if(usrOK&&passOK) {
				LogInButton.interactable=true;
			}
		}
	}

	public void hasPassword() {
		usrOK=true;
		string password = passwordField.textComponent.text;
		if(password==""||password == null) {
			Debug.Log ("was null");
			passErrorText.text = "* Please enter a password.";
			passErrorText.enabled=true;
			LogInButton.interactable=false;
			passOK=false;
		}
		if(password.Length<5) {
			passErrorText.text = "* A password must be at least 5 characters long.";
			passErrorText.enabled=true;
			LogInButton.interactable=false;
			passOK=false;
		}
		else {
			passErrorText.enabled=false;
			passOK=true;
			if(usrOK&&passOK) {
				LogInButton.interactable=true;
			}
		}
	}

	public void LogIn() {

		networkScript.Connect (emailField.textComponent.text,passwordField.GetComponent<InputField>().text);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
