using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TWD_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // Apumuuttuja, joka ilmaisee onko hiiren kursori painikkeen päällä.
    private bool isHovered = false;


    public bool GetIsHovered() { return isHovered; }

    // Kun hiiren kursori liikkuu tämän peliobjektin päälle.
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // Määritetään apumuuttujan arvo.
        isHovered = true;
    }

    // Kun hiiren kursori poistuu tämän peliobjektin päältä.
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // Määritetään apumuuttujan arvo.
        isHovered = false;
    }
}
