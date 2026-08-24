using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;
using System.ComponentModel;
//using System.Diagnostics;

public class PokemonLoader : MonoBehaviour
{
    public void FetchPokemon(string pokemonIdOrName, Action<PokemonData> onSucess)
    {
        StartCoroutine(LoadDataRoutine(pokemonIdOrName, onSucess));
    }

    IEnumerator LoadDataRoutine(string pokemonIdOrName, Action<PokemonData> onSucess)
    {
        string url = "https://pokeapi.co/api/v2/pokemon/" + pokemonIdOrName;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                //Debug.LogError("Loading Error: " + webRequest.error);
                onSucess?.Invoke(null);
                yield break;
            }

            string jsonResult = webRequest.downloadHandler.text;
            PokemonData pokemonData = JsonUtility.FromJson<PokemonData>(jsonResult);

            onSucess?.Invoke(pokemonData);
        }
    }

    public void FetchImage(string imageUrl, Action<Sprite> onSucess)
    {
        StartCoroutine(LoadImageRoutine(imageUrl, onSucess));
    }

    IEnumerator LoadImageRoutine(string imageUrl, Action<Sprite> onSucess)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Loading Error: " + webRequest.error);
                yield break;
            }

            //Converte bytes baixados (png) em Texture2D
            byte[] imageBytes = webRequest.downloadHandler.data;
            Texture2D _texture = new Texture2D(2, 2);
            _texture.LoadImage(imageBytes);

            //Coverte textura em Sprite
            Sprite sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
            onSucess?.Invoke(sprite);
        }
    }

    public void FetchTotalPokemonCount(Action<int> onSucess)
    {
        StartCoroutine(LoadTotalCountRoutine(onSucess));
    }

    private IEnumerator LoadTotalCountRoutine(Action<int> onSucess)
    {
        string url = "https://pokeapi.co/api/v2/pokemon?limit=1";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Erro ao buscar total: " + webRequest.error);
                yield break;
            }

            string jsonResult = webRequest.downloadHandler.text;
            PokemonListResponse response = JsonUtility.FromJson<PokemonListResponse>(jsonResult);

            onSucess?.Invoke(response.count);
        }
    }

    public void FetchPokemonDescription(string pokemonId, Action<string> onSucess)
    {
        StartCoroutine(LoadPokemonDescriptionRoutine(pokemonId, onSucess));
    }

    private IEnumerator LoadPokemonDescriptionRoutine(string pokemonId, Action<string> onSucess)
    {
        string url = "https://pokeapi.co/api/v2/pokemon-species/" + pokemonId;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                onSucess?.Invoke("Description indisponible.");
                yield break;
            }

            SpeciesData speciesData = JsonUtility.FromJson<SpeciesData>(webRequest.downloadHandler.text);
            string rawDescription = "";

            if (speciesData != null && speciesData.flavor_text_entries != null)
            {
                foreach (var entry in speciesData.flavor_text_entries)
                {
                    if (entry.language != null && entry.language.name == "en")
                    {
                        rawDescription = entry.flavor_text;
                        break;
                    }
                }
            }

            string cleanDescription = rawDescription
                .Replace("\n", " ")
                .Replace("\f", " ")
                .Replace("\r", " ")
                .Trim();
            
            onSucess?.Invoke(cleanDescription);

        }
    }
}
