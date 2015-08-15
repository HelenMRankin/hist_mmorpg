using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public class PopUpDialog : MonoBehaviour {
	public delegate void clicked(bool result);
	public static clicked CallBackFunction = null;
	Text panelText;
	// Use this for initialization
	void Start () {
		var canvas = GameObject.Find ("Canvas");
		if(canvas==null) {
			Debug.LogError ("Could not identify Canvas for message display.");
			return;
		}
		this.gameObject.transform.SetParent (canvas.transform,false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ShowPopUpDialog(string message, string confirm = null, string deny=null,clicked callback = null) {
		Debug.Log ("In show popup");
		CallBackFunction=callback;
		GameObject optionContainer = this.gameObject.transform.Find ("Options").gameObject;
		//panelText = this.gameObject.GetComponentInChildren<Text> ();
		panelText = this.gameObject.transform.Find ("ItemDetails").GetComponentInChildren<Text>();
		Debug.Log ("Added panel");
		GameObject confirmBtn = new GameObject();
		confirmBtn.name="Confirm";
		GameObject text = new GameObject();
		text.AddComponent<Text>();
		if(!string.IsNullOrEmpty (deny)) {
			GameObject denyBtn = new GameObject();
			denyBtn.name="Deny";
			GameObject text2 = new GameObject();
			text2.AddComponent<Text>();
			text2.GetComponent<Text>().text = deny;
			text2.GetComponent<Text>().color=Color.black;
			text2.GetComponent<Text>().font=Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			text2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			text2.transform.SetParent (denyBtn.transform);
			denyBtn.AddComponent<RectTransform>();
			denyBtn.AddComponent <Button>();
			denyBtn.GetComponent <Button>().onClick.AddListener (()=>onDenyClick());
			denyBtn.AddComponent<LayoutElement>();
			denyBtn.GetComponent<LayoutElement>().minWidth=80;
			denyBtn.transform.SetParent (optionContainer.transform);
			denyBtn.AddComponent<Image>();
			denyBtn.GetComponent<Image>().overrideSprite= Resources.Load<Sprite>("Images/button");
		}
		if(!string.IsNullOrEmpty (confirm)) {
			text.GetComponent<Text>().text = confirm;
		}
		else {
			text.GetComponent<Text>().text = "Ok";
		}
		text.GetComponent<Text>().color=Color.black;
		text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		text.GetComponent<Text>().font=Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		text.transform.SetParent (confirmBtn.transform);
		confirmBtn.AddComponent<RectTransform>();
		confirmBtn.AddComponent <Button>();
		confirmBtn.GetComponent <Button>().onClick.AddListener (()=>onConfirmClick());
		confirmBtn.AddComponent<LayoutElement>();
		confirmBtn.GetComponent<LayoutElement>().minWidth=80;
		confirmBtn.transform.SetParent (optionContainer.transform);
		confirmBtn.AddComponent<Image>();
		Sprite btnSprite = Resources.Load <Sprite>("Images/button");
		if(btnSprite==null) {
			Debug.LogError ("Sprite is null");
		}
		confirmBtn.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Images/button");
		panelText.text=message;

	}

	void onConfirmClick() {
		if(CallBackFunction!=null) {
			CallBackFunction(true);
		}

		Destroy(this.gameObject);
	}
	void onDenyClick() {
		if(CallBackFunction!=null) {
			CallBackFunction(false);
		}

		Destroy(this.gameObject);
	}
}
