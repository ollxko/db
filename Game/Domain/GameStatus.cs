using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public enum GameStatus
    {
        WaitingToStart,
        Playing,
        Finished,
        Canceled
    }
}