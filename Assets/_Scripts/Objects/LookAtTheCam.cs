using UnityEngine;
using Zenject;

public class LookAtTheCam : MonoBehaviour
{
    [Inject] private Player _player;
    private Transform _playerTF => _player.transform;
    private void Update()
    {
        transform.LookAt(_playerTF);
    }
}
