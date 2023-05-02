using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password too short";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucess, OnError);
    }
    void OnRegisterSucess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registered and logged in :)";
    }
    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text, 
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSucess, OnError);
    }
    void OnLoginSucess(LoginResult result)
    {
        messageText.text = "Logged in :)";
        Debug.Log("Account sucessfulled logged in :O");

    }
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest 
        {
            Email = emailInput.text,
            TitleId = "66710"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset email send :)"; 
    }

}
