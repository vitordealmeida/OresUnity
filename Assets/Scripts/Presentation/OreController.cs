using System;
using Domain.Entities;
using UnityEngine;

public class OreController : MonoBehaviour
{
    private Ore _ore;
    private Action<Ore> _clickCallback;
    public GameObject destructionEffectPrefab;
    public void Init(Ore ore, Action<Ore> clickCallback)
    {
        _ore = ore;
        _clickCallback = clickCallback;
    }

    public void OnMouseUpAsButton()
    {
        _clickCallback(_ore);
    }

    public GameObject DestructionEffectPrefab()
    {
        return destructionEffectPrefab;
    }
}
