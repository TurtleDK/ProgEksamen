using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    //  Header til unity og variabler 
    [Header("UI")]
    public TMP_Text messageText;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public void RegisterButton()
    {
        // Tjek på længden af password
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password too short";
            return;
        }
        // Registrerings logik 
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucess, OnError);
    }
    //Hvis registrering var succesfuld, send feedback til spiller og load slot
    void OnRegisterSucess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registered and logged in :)";
        StartCoroutine(LoadNextScene());
    }
    //Hvis der forekommer en fejl sender PLayfab dens egen error beskeder
    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
    //Login logik 
    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text, 
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSucess, OnError);
    }
    // login feedback til spilleren & load næste scene + Debug besked til konsollen
    void OnLoginSucess(LoginResult result)
    {
        messageText.text = "Logged in :)";
        Debug.Log("Account sucessfully logged in :O");
        StartCoroutine(LoadNextScene());
    }
    // Glemt kodeord
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest 
        {
            Email = emailInput.text,
            // Id jeg har fået af playfab - Krav fra deres side af
            TitleId = "66710"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }
    //Hvis der findes en konto med mailen, giver den feedback.
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset email send :)"; 

    }

    //Logik til at loade næste scene 
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(+1);
        Debug.Log("Loaded next scene");
    }
}
