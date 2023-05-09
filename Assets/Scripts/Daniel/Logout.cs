using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    //Function is run by the logout button
    public void LogOut()
    {
        //Switching scene to login
        SceneManager.LoadSceneAsync(0);
    }
}
