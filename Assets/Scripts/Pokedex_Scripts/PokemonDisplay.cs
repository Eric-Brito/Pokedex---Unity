using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
//using System.Diagnostics;

public class PokemonDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pokemonName;
    public Image pokemonImage;
    public Sprite transparent;
    public Slot[] slots;
    public TextMeshProUGUI pokemonDescription;
    public TextMeshProUGUI groupIndicator;

    [Header("Dependencies")]
    public PokemonLoader pokemonLoader;

    public int currentPokemonId = 1; //private
    public int currentGroupId = 0; //private
    public int maxGroupId = 19;

    void Awake()
    {
        InitiateSlots();
    }

    void Start()
    {
        UpdateMainDisplay(currentPokemonId.ToString());
        UpdateSlotImages(currentPokemonId.ToString());
        UpdateGroupIndicator();
    }

    public void NextPokemon()
    {
        //currentPokemonId++;

        if (currentGroupId > maxGroupId) return; //quantidade de páginas

        currentGroupId++;
        currentPokemonId = currentGroupId * slots.Length + 1;

        UpdateMainDisplay(currentPokemonId.ToString());
        UpdateSlotImages(currentPokemonId.ToString());
        UpdateGroupIndicator();
    }

    public void PreviousPokemon()
    {
        //if (currentPokemonId <= 1) return;
        //currentPokemonId--;

        if (currentGroupId <= 0) return;

        currentGroupId--;
        currentPokemonId = currentGroupId * slots.Length + 1;

        UpdateMainDisplay(currentPokemonId.ToString());
        UpdateSlotImages(currentPokemonId.ToString());
        UpdateGroupIndicator();
    }

    public void InitiateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotId = i + 1;
        }
    }

    public void SelectSlot(int slotId)
    {
        currentPokemonId = currentGroupId * slots.Length + slotId;
        UpdateMainDisplay(currentPokemonId.ToString());
    }

    public void UpdateSlotImages(string pokemonId)
    {
        int _pokemonId = int.Parse(pokemonId);
        int lastId = _pokemonId + slots.Length;

        for (int i = _pokemonId; i < lastId; i++)
        {
            int index = i - _pokemonId; //closure corrigido
            slots[index].slotLoading.SetActive(true);
            slots[index].slotImage.sprite = transparent;

            pokemonLoader.FetchPokemon(i.ToString(), (pokemonData) =>
            {
                if (pokemonData == null || pokemonData.sprites == null)
                {
                    slots[index].slotImage.sprite = transparent;
                    slots[index].gameObject.SetActive(false);
                    return;
                }

                slots[index].gameObject.SetActive(true);

                if (pokemonData.sprites != null && !string.IsNullOrEmpty(pokemonData.sprites.front_default))
                {
                    pokemonLoader.FetchImage(pokemonData.sprites.front_default, (sprite) => 
                    {
                        slots[index].slotImage.sprite = sprite;
                        slots[index].slotLoading.SetActive(false);
                        //Debug.Log(index);
                    });
                }
            });
        }
        
    }

    private void UpdateMainDisplay(string pokemonId)
    {
        pokemonLoader.FetchPokemon(pokemonId, (pokemonData) =>
        {
            if (pokemonData == null || pokemonData.sprites == null)
            {
                pokemonImage.sprite = transparent;
                return;
            }

            UpdatePokemonName(pokemonData);
            UpdatePokemonImage(pokemonData);
            UpdatePokemonDescription(pokemonId);
        });
    }

    private void UpdateTotalCount()
    {
        pokemonLoader.FetchTotalPokemonCount((totalCount) =>
        {
            Debug.Log("Total de Pokemons na API: " + totalCount);
        });
    }

    private void UpdatePokemonName(PokemonData pokemonData)
    {
        pokemonName.text = "#" + pokemonData.id + " " + pokemonData.name.ToUpper();
    }

    private void UpdatePokemonImage(PokemonData pokemonData)
    {
        if (pokemonData.sprites == null) return;
        if (string.IsNullOrEmpty(pokemonData.sprites.front_default)) return;

        string imageUrl = pokemonData.sprites.front_default;

        pokemonLoader.FetchImage(imageUrl, (sprite) => 
        {
            pokemonImage.sprite = sprite;
        });
    }

    private void UpdatePokemonDescription(string pokemonId)
    {
        pokemonLoader.FetchPokemonDescription(pokemonId, (description) =>
        {
            //Debug.Log(description);
            pokemonDescription.text = description;
        });
    }

    private void UpdateGroupIndicator()
    {
        string cleanGroupIndicator = (currentGroupId + 1).ToString() + "/" + (maxGroupId + 1).ToString(); 
        groupIndicator.text = cleanGroupIndicator;
    }
}
