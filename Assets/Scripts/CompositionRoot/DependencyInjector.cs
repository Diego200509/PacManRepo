using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class DependencyInjector : MonoInstaller
{
    [SerializeField] private GameBoardView gameBoardView;

    public override void InstallBindings()
    {
        // Puerta de acceso al tablero
        Container.Bind<IGameBoardGateway>()
                 .FromInstance(gameBoardView)
                 .AsSingle();

        Container.Bind<GameBoardView>()
             .FromInstance(gameBoardView)
             .AsSingle();

        // Vista de Pac-Man
        Container.Bind<PacManView>()
                 .FromComponentInHierarchy()
                 .AsSingle();


        // Caso de uso para mover a Pac-Man
        Container.Bind<IMovePacManUseCase>()
                 .To<MovePacManUseCase>()
                 .AsTransient();

        // Caso de uso para consumir pellets
        Container.Bind<IConsumePelletUseCase>()
         .To<ConsumePelletUseCase>()
         .AsTransient();

    }
}
