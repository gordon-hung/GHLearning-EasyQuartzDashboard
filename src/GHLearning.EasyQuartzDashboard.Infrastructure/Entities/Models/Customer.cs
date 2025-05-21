using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GHLearning.EasyQuartzDashboard.Infrastructure.Entities.Models;

public class Customer
{
	public ObjectId Id { get; set; }

	[BsonElement("name")]
	public string Name { get; set; } = default!;

	[BsonElement("order")]
	public string Order { get; set; } = default!;

	[BsonElement("created_at")]
	[BsonRepresentation(BsonType.String)]
	public DateTimeOffset CreatedAt { get; set; } = default!;
}