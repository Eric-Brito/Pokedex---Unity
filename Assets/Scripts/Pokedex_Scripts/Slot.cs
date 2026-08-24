using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotId;
    public Image slotImage;
    public GameObject slotLoading;
    public PokemonDisplay pokemonDisplay;

    public void SelectThisSlot()
    {
        pokemonDisplay.SelectSlot(slotId);
    }
}
