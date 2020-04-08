using System.Collections.Generic;
using ECSEngine;
using ECSEngine.Entities;
using ECSEngine.Managers;
using UlaidGame.Entities;

namespace UlaidGame
{
    internal sealed class UlaidGame : Game
    {
        public UlaidGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
            var entities = new List<IEntity>
            {
                new PlayerEntity(),
                new LevelModelEntity()
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);
        }
    }
}
