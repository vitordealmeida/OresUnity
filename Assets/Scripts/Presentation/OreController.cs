using System;
using Domain.Entities;
using UnityEngine;

public class OreController : MonoBehaviour
{
    private Ore _ore;
    private Action<Ore> _clickCallback;
    public void Init(Ore ore, Action<Ore> clickCallback)
    {
        _ore = ore;
        _clickCallback = clickCallback;
    }

    public void OnMouseUpAsButton()
    {
        _clickCallback(_ore);
    }
}
