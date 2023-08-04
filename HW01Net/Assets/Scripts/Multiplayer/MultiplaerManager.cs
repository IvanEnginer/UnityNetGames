using Colyseus;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplaerManager : ColyseusManager<MultiplaerManager>
{
    [SerializeField] private GameObject _player;
    [SerializeField] private EnemyController _enemy;

    private ColyseusRoom<State> _room; 

    protected override void Awake()
    {
        base.Awake();

        Instance.InitializeClient();
        Connect();
    }

    private async void Connect()
    {
        _room = await Instance.client.JoinOrCreate<State>("state_handler");

        _room.OnStateChange += OnChange;
    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;

        state.players.ForEach((string key,Player player) => {
            if (key == _room.SessionId) CreatPlayer(player);
            else CreatEnemy(key, player);
        });

        _room.State.players.OnAdd += CreatEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }

    private void CreatPlayer(Player player)
    {
        var position = new Vector3(player.x, 0, player.y);

        Instantiate(_player, position, Quaternion.identity);
    }

    private void CreatEnemy(string key ,Player player)
    {
        var position = new Vector3(player.x, 0, player.y);

        var enemy = Instantiate(_enemy, position, Quaternion.identity);

        player.OnChange += enemy.OnChange;
    }

    private void RemoveEnemy(string key, Player player)
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _room.Leave();
    }

    public void SendMassage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }
}