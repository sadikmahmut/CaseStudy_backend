using MongoDB.Bson.Serialization.Attributes;

namespace backend_app.Model
{
    public class Configuration
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string BuildingType { get; set; }
        public float BuildingCost { get; set; }
        public float ConstructionTime { get; set; }
    }
}
