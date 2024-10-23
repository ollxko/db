
using System;
using System.Collections.Generic;
using DnsClient;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        public const string CollectionName = "games";
        public IMongoCollection<GameEntity> gameCollection;

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var gameEntity = gameCollection.Find(game => game.Id == gameId).FirstOrDefault();

            return gameEntity;
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(oldGame => oldGame.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            return gameCollection
                .Find(x => x.Status == GameStatus.WaitingToStart)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {

            var result = gameCollection.ReplaceOne(x => x.Id == game.Id && x.Status == GameStatus.WaitingToStart, game);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}