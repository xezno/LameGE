using System.Collections.Generic;
using ECSEngine;
using ECSEngine.Entities;
using ECSEngine.Managers;
using SpaceGame.Entities;

namespace SpaceGame
{
    internal sealed class SpaceGame : Game
    {
        public SpaceGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
            var entities = new List<IEntity>
            {
                new PlayerEntity(),
                new LevelModelEntity()
            };

            foreach (IEntity entity in entities)
                SceneManager.instance.AddEntity(entity);
        }
    }
}
