using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using System.Collections.Generic;
//using System.Diagnostics;

[System.Serializable]
public struct PokemonTypeDisplay
{
    public Image pokemonTypePanel; 
    public TextMeshProUGUI pokemonType;
}

public class PokemonDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pokemonName;
    public Image pokemonImage;
    public Sprite transparent;
    public Slot[] slots;
    public TextMeshProUGUI pokemonDescription;
    public TextMeshProUGUI groupIndicator;
    public PokemonTypeDisplay[] pokemonsTypeDisplays;
    public CanvasGroup slotsCanvasGroup;
    public CanvasGroup previousAndNextCanvasGroup;

    [Header("Dependencies")]
    public PokemonLoader pokemonLoader;

    public int currentPokemonId = 1; //private
    public int currentGroupId = 0; //private
    public int maxGroupId = 19;

    //public string fetchedName;
    //public string fetchedDescription;
    //public Sprite fetchedImage;

    [SerializeField] public Dictionary<string, Color> pokemonTypePanelColors = new Dictionary<string, Color>()
    {
        {"BUG", new Color32(136, 150, 14, 255)},
        {"DARK", new Color32(61, 45, 36, 255)},
        {"DRAGON", new Color32(114, 93, 222, 255)},
        {"ELECTRIC", new Color32(252, 186, 22, 255)},
        {"FAIRY", new Color32(240, 169, 237, 255)},
        {"FIGHTING", new Color32(131, 56, 31, 255)},
        {"FIRE", new Color32(199, 34, 0, 255)},
        {"FLYING", new Color32(93, 115, 214, 255)},
        {"GHOST", new Color32(90, 94, 170, 255)},
        {"GRASS", new Color32(108, 191, 49, 255)},
        {"GROUND", new Color32(213, 178, 90, 255)},
        {"ICE", new Color32(119, 215, 246, 255)},
        {"NORMAL", new Color32(198, 191, 183, 255)},
        {"POISON", new Color32(147, 68, 144, 255)},
        {"PSYCHIC", new Color32(237, 72, 129, 255)},
        {"ROCK", new Color32(155, 135, 60, 255)},
        {"STEEL", new Color32(157, 157, 171, 255)},
        {"WATER", new Color32(49, 145, 237, 255)}
    };

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

    public void SetSlotsInteractable(bool isInteractable)
    {
        if (slotsCanvasGroup == null) return;

        slotsCanvasGroup.interactable = isInteractable;
        slotsCanvasGroup.blocksRaycasts = isInteractable;
    }

    public void SetSlotsInteractableToTrue()
    {
        if (slotsCanvasGroup == null) return;

        slotsCanvasGroup.interactable = true;
        slotsCanvasGroup.blocksRaycasts = true;
    }

    public void SetPreviousAndNextInteractable(bool isInteractable)
    {
        if (previousAndNextCanvasGroup == null) return;

        previousAndNextCanvasGroup.interactable = isInteractable;
        previousAndNextCanvasGroup.blocksRaycasts = isInteractable;
    }

    public void SetPreviousAndNextInteractableToTrue()
    {
        if (previousAndNextCanvasGroup == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotLoading.activeInHierarchy)
            {
                Debug.Log("Ainda há um slot carregando");
                Invoke(nameof(SetPreviousAndNextInteractableToTrue), 0.5f);
                return;
            }
        }

        Debug.Log("Slot carregado com sucesso");
        previousAndNextCanvasGroup.interactable = true;
        previousAndNextCanvasGroup.blocksRaycasts = true;
    }

    public void UpdateSlotImages(string pokemonId)
    {
        int _pokemonId = int.Parse(pokemonId);
        int lastId = _pokemonId + slots.Length;

        SetPreviousAndNextInteractable(false);

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

        Invoke(nameof(SetPreviousAndNextInteractableToTrue), 1f);
    }

    private void UpdateMainDisplay(string pokemonId)
    {
        SetSlotsInteractable(false);

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

            UpdatePokemonTypes(pokemonId); //Test
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
            pokemonDescription.text = description;
        });
    }

    private void UpdateGroupIndicator()
    {
        string cleanGroupIndicator = (currentGroupId + 1).ToString() + "/" + (maxGroupId + 1).ToString(); 
        groupIndicator.text = cleanGroupIndicator;
    }

    private void UpdatePokemonTypes(string pokemonId)
    {
        pokemonLoader.FetchPokemon(pokemonId, (pokemonData) =>
        {
            if (pokemonData.types.Length < 2)
            {
                pokemonsTypeDisplays[1].pokemonType.text = "";
                pokemonsTypeDisplays[1].pokemonTypePanel.gameObject.SetActive(false);
            } else
            {
                pokemonsTypeDisplays[1].pokemonTypePanel.gameObject.SetActive(true);
            }

            //string textTypes = "";
            for (int i = 0; i < pokemonData.types.Length; i++)
            {
                //textTypes += pokemonData.types[i].type.name.ToUpper();
                //if (i < pokemonData.types.Length - 1) textTypes += " /";

                string textType = pokemonData.types[i].type.name.ToUpper();

                pokemonsTypeDisplays[i].pokemonType.text = textType;
                pokemonsTypeDisplays[i].pokemonTypePanel.color = pokemonTypePanelColors[textType];
            }

            Invoke(nameof(SetSlotsInteractableToTrue), 1f);
        });
    }
}

//Função anônima é simples: (p) => {}

//(nome de parametro) =>
//{}