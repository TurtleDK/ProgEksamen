// importerer n�dvendige biblioteker til at bruge Unity-grafik og GUI-elementer.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeInput : MonoBehaviour
{
    // variabler til brug i menuen 
    EventSystem system;
    public Selectable firstInput;
    public Button submitButton;
    // I denne funktion hentes referencen til "EventSystem" og s�tter fokus p� det f�rste "Selectable" objekt.
    void Start()
    {
        system = EventSystem.current;
        firstInput.Select();
    }

    //Funktionen "Update" vil k�res hver frame i spillet 
    void Update()
    {
        //Hvis brugeren trykker p� "Tab" tasten, vil scriptet finde det n�ste "Selectable" objekt. 
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null) // Rykker kun videre til n�ste objekt, hvis der findes et
            {
                next.Select();
            }
        }
        //Hvis brugeren trykker p� "Enter", k�res "onClick" p� "submitButton" objektet
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            submitButton.onClick.Invoke(); //K�re onClick p� submit (Login) knappen
            Debug.Log("Button Pressed"); //Debugging
        }
    }
}
