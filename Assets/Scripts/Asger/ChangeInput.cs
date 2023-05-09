// importerer nødvendige biblioteker til at bruge Unity-grafik og GUI-elementer.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeInput : MonoBehaviour
{
    // variabler til brug i menuen 
    EventSystem system;
    public Selectable firstInput;
    public Button submitButton;
    // I denne funktion hentes referencen til "EventSystem" og sætter fokus på det første "Selectable" objekt.
    void Start()
    {
        system = EventSystem.current;
        firstInput.Select();
    }

    //Funktionen "Update" vil køres hver frame i spillet 
    void Update()
    {
        //Hvis brugeren trykker på "Tab" tasten, vil scriptet finde det næste "Selectable" objekt. 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null) // Rykker kun videre til næste objekt, hvis der findes et
            {
                next.Select();
            }
        }
        //Hvis brugeren trykker på "Enter", køres "onClick" på "submitButton" objektet
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            submitButton.onClick.Invoke(); //Køre onClick på submit (Login) knappen
            Debug.Log("Button Pressed"); //Debugging
        }
    }
}
