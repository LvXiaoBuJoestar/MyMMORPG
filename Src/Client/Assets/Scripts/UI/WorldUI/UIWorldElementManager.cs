using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    [SerializeField] private GameObject uINameBar;

    Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject goUINameBar = Instantiate(uINameBar, transform);
        goUINameBar.GetComponent<UINameBar>().character = character;
        goUINameBar.GetComponent<UIWorldElement>().owner = owner;
        elements[owner] = goUINameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (!elements.ContainsKey(owner)) return;
        Destroy(elements[owner]);
        elements.Remove(owner);
    }
}
