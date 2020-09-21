using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    // Onko objekti, jossa tämä komponentti on kiinni, vaarallinen pelaajalle.
    [Tooltip("Onko vaarallinen pelaajalle.")]
    [SerializeField] private bool harmfulToPlayer;

    // Onko objekti, jossa tämä komponentti on kiinni, vaarallinen NPC-hahmoille.
    [Tooltip("Onko vaarallinen NPC-hahmoille.")]
    [SerializeField] private bool harmfulToNPC;

    // Vahinkoarvo objektille.
    [SerializeField] private float value;

    // Palauttaa vahinkarvon.
    public float GetValue() { return value; }

    // Palauttaa tiedon, voiko objekti vahingoittaa pelaajaa.
    public bool IsHarmfulToPlayer() { return harmfulToPlayer; }

    // Palauttaa tiedon, voiko objekti vahingoittaa NPC-hahmoja.
    public bool IsHarmfulToNPC() { return harmfulToNPC; }
}
