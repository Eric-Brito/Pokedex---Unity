using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Diagnostics.SymbolStore;
//using System.Reflection;

[Serializable]
public class PokemonData
{
    public string name;
    public int id;
    public Sprites sprites;
    public TypeEntry[] types;
}

[Serializable]
public class Sprites
{
    public string front_default; //URL main image
}

[Serializable]
public class PokemonListResponse
{
    public int count;
}

//Description Classes
[Serializable]
public class SpeciesData
{
    public FlavorTextEntry[] flavor_text_entries;
}

[Serializable]
public class FlavorTextEntry
{
    public string flavor_text;
    public Language language;
}

[Serializable]
public class Language
{
    public string name;
}

[Serializable]
public class TypeEntry
{
    public int slot;
    public TypeInfo type;
}

[System.Serializable]
public class TypeInfo
{
    public string name;
}
