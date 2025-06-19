using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DILogin : MonoInstaller
{
    public override void InstallBindings()
    {

        string dbPath = "players.sqlite";
        var database = SQLitePlayerDatabase.GetInstance(dbPath);

        Container.Bind<IDatabase<PlayerEntity>>().FromInstance(database).AsSingle();
        Container.Bind<LoginPlayer>().AsTransient();
        Container.Bind<RegisterPlayer>().AsTransient();
        Container.Bind<LogoutPlayer>().AsTransient();
        Container.Bind<IPlayerSessionProvider>()
                .To<PlayerSessionProvider>()
                .AsSingle();
    }

}
