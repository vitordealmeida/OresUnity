using System;
using Domain.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class MatchController : MonoBehaviour
    {
        private Game _game;

        private void Awake()
        {
            _game = new Game();
            Debug.Log(_game.ToString());
        }
    }
}