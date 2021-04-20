using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectionWorker.Order
{
    public class Order
    {
        public Order(Guid orderId)
        {
            OrderId = orderId;
        }

        [BsonId] public ObjectId ObjectId { get; set; }
        public Guid OrderId { get; set; }
    }
}