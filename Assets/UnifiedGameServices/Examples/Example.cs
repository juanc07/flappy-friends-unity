using UnityEngine;

public class Example : MonoBehaviour
{
	private int padding = 10;
	
	protected int top { get { return padding; } }
	protected int left { get { return padding; } }
	protected int bottom { get { return Screen.height - padding; } }
	protected int right { get { return Screen.width - padding; } }
	protected int width { get { return right - left; } }
	protected int height { get { return bottom - top; } }
	
	private string connectionStatus = "Not Connected";
	
	protected void InitConnectionCallbacks()
	{
		Ugs.Client.OnConnected += () =>
		{
			connectionStatus = "Connected";
			Debug.Log("Connected to Google Play Games");
		};
		
		Ugs.Client.OnConnectionFailed += () =>
		{
			var result = Ugs.Client.ConnectionResult;
			connectionStatus = "ConnectionFailed: " + result + " (" + result.ErrorCode() + ")";
			connectionStatus += "\n" + Ugs.Client.ConnectionResult.Details();
			Debug.LogWarning("Failed to connect to Google Play Games.");
		};
		
		Ugs.Client.OnDisconnected += () =>
		{
			connectionStatus = "Disconnected";
			Debug.Log("Disconnected from Google Play Games");
		};
	}
	
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
	
	protected void BeginGUI(int placeholdersCount = 10)
	{
		var buttonHeight = height / Mathf.Max(1, placeholdersCount);
		
		GUILayout.BeginArea(new Rect(left, top, width, height));
		GUI.skin.button.fixedHeight = buttonHeight;
		GUI.skin.button.fixedWidth = width;
		GUI.skin.textField.wordWrap = true;
		GUI.skin.textField.fixedWidth = width;
		GUI.skin.textField.fixedHeight = 3 * buttonHeight;
	}
	
	protected void EndGUI()
	{
		GUILayout.EndArea();
	}
	
	protected void BeginButtonsRow(int buttonsCount)
	{
		GUI.skin.button.fixedWidth = width / buttonsCount;
		GUILayout.BeginHorizontal();
	}
	
	protected void EndButtonsRow()
	{
		GUILayout.EndHorizontal();
		GUI.skin.button.fixedWidth = width;
	}
	
	protected void LoginScreen()
	{
		GUILayout.Label("Connection Status:");
		GUILayout.TextField(connectionStatus);
		
		if (GUILayout.Button("Connect"))
		{
			Ugs.Client.Connect();
		}
		
		if (GUILayout.Button("Sign In"))
		{
			Ugs.Client.SignIn();
		}
	}
}
