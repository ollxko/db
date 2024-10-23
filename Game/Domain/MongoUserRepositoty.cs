using System;
using System.Collections.Generic;
using DnsClient;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            var keys = Builders<UserEntity>.IndexKeys.Ascending("Login");
            var options = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<UserEntity>(keys, options);
            userCollection.Indexes.CreateOne(indexModel);
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = Builders<UserEntity>.Filter.Eq("Id", id);
            return userCollection.Find(filter).FirstOrDefault();
            throw new NotImplementedException();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var filter = Builders<UserEntity>.Filter.Eq("Login", login);
            var user = userCollection.Find(filter).FirstOrDefault() ?? Insert(new UserEntity(Guid.Empty, login, "last", "first", 2, Guid.NewGuid()));
            return user;
        }
        public void Update(UserEntity user)
        {
            var filter = new BsonDocument("_id", user.Id);
            var result = userCollection.ReplaceOne(filter, user);
            //TODO: Ищи в документации ReplaceXXX
        }

        public void Delete(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            userCollection.DeleteOne(filter);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            var filter = Builders<UserEntity>.Filter.Empty;
            var sort = Builders<UserEntity>.Sort.Ascending("Login");
            var cursor = userCollection.Find(filter).Sort(sort).Skip(skip).Limit(pageSize).ToCursor();
            var documents = cursor.ToList();
            long totalCount = userCollection.CountDocuments(filter);
            return new PageList<UserEntity>(documents, totalCount, pageNumber, pageSize);

        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}
