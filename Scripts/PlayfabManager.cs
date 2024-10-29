using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Networking.UnityWebRequest;

public class Playf : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI message;
    public TextMeshProUGUI pswErrorText;
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPswInput;

    [Header("Button")]
    public Button registerButton;
    public Button loginButton;

    [Header("DeactivateOnSuccess")]
    public GameObject Register;
    public GameObject Login;
    public GameObject Switch;
    public GameObject InputFields;

    [Header("ActivateOnSuccess")]
    public GameObject RegOutline;
    public GameObject LoginOutline;

    [Header("UI to Activate")]
    public GameObject ExtraGroup;

    private string username = "";
    private bool logged = false;

    // Start is called before the first frame update
    void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPswInput.contentType = TMP_InputField.ContentType.Password;

        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnCLogSuccess, OnCLogFailure);
    }

    private void OnCLogSuccess(LoginResult result) { }
    private void OnCLogFailure(PlayFabError error) { }

    public void RegisterButton()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            message.text = "Please enter a username.";
            Invoke("ResetMessage", 1f);
            return;
        }
        if (string.IsNullOrEmpty(emailInput.text))
        {
            message.text = "Please enter an Email.";
            Invoke("ResetMessage", 1f);
            return;
        }
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            message.text = "Please enter a password.";
            Invoke("ResetMessage", 1f);
            return;
        }
        if (passwordInput.text.Length > 6)
        {
            message.text = "Password must be at least 6 characters.";
            Invoke("ResetMessage", 1f);
            return;
        }

        if (passwordInput.text != confirmPswInput.text)
        {
            pswErrorText.text = "Paswords do not match.";
            pswErrorText.color = Color.red;
            return;
        }
        else
        {
            pswErrorText.text = null;
            pswErrorText.color = Color.white;
        }

        var getUsernm = new GetAccountInfoRequest { Username = usernameInput.text };
        PlayFabClientAPI.GetAccountInfo(getUsernm, OnGetAccountFoundUser, OnGetAccountNewUser);
    }

    void OnGetAccountFoundUser(GetAccountInfoResult result)
    {
        if (result != null)
        {
            message.text = "Username already exists.";
            Invoke("ResetMessage", 1f);
            return;
        }
    }

    void ResetMessage()
    {
        if (logged)
        {
            message.text = "Welcome " + username;
            PlayerPrefs.SetString("PlayerName", username);
        }
        else
        {
            message.text = "Register/Log in";
        }
    }

    void OnGetAccountNewUser(PlayFabError error)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = true
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        message.text = "Welcome, " + result.Username;
        username = result.Username;
        logged = true;
        Debug.Log("Registration Successful!");
        Debug.Log("PlayFabId: " + result.PlayFabId);

        var linkRequest = new LinkCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            ForceLink = true
        };
        PlayFabClientAPI.LinkCustomID(linkRequest, OnLinkSuccess, OnLinkFailure);

        var updtDispName = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = usernameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(updtDispName, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);


        ToggleOnSuccess(true);
    }

    void ToggleOnSuccess(bool deactivate)
    {
        Register.SetActive(!deactivate);
        if (deactivate)
        {
            Login.SetActive(!deactivate);
        }
        Switch.SetActive(!deactivate);
        InputFields.SetActive(!deactivate);
        RegOutline.SetActive(deactivate);
        LoginOutline.SetActive(deactivate);
        ExtraGroup.SetActive(deactivate);
    }

    void OnRegisterFailure(PlayFabError error)
    {
        message.text = "Registration failed with error: " + error.ErrorMessage;
        Invoke("ResetMessage", 3f);
        Debug.Log(error.GenerateErrorReport());
    }

    public void LogInButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
    void OnLinkSuccess(LinkCustomIDResult result)
    {
        Debug.Log("Link successful!");
        Debug.Log("Linked successfully with PlayFabId: " + result);
    }

    void OnLinkFailure(PlayFabError error)
    {
        Debug.Log("Link failed with error: " + error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLoginSuccess(LoginResult result)
    {
        message.text = "Login successful!";
        var usnmRequest = new GetAccountInfoRequest { Email = emailInput.text };
        PlayFabClientAPI.GetAccountInfo(usnmRequest, OnGetAccountInfo, OnGetAccountInfoFailure);

        Invoke("ResetMessage", 2f);
        Debug.Log("Session Ticket: " + result.SessionTicket);

        ToggleOnSuccess(true);
    }
    void OnGetAccountInfo(GetAccountInfoResult result)
    {
        username = result.AccountInfo.Username;
        logged = true;
    }
    void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.Log("Failed to get account info: " + error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLoginFailure(PlayFabError error)
    {
        message.text = error.ErrorMessage;
        Invoke("ResetMessage", 3f);
        Debug.Log(error.GenerateErrorReport());
    }
    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully");
    }

    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update display name: " + error.GenerateErrorReport());
    }

    public void LogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        logged = false;
        message.text = "Logged Out!";
        Invoke("ResetMessage", 3f);
        username = null;
        ToggleOnSuccess(false);
        Start();
    }
}
