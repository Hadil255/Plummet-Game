using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

public class PlayerData
{
    public string plummie_tag;
    public int collisions;
    public int steps;

    // Ajout pour la sauvegarde
    private static string connectionString = "mongodb+srv://hadillahar:myPassword123@cluster.j0sfx.mongodb.net/progression?retryWrites=true&w=majority";
    private static IMongoDatabase database;

    // Méthode pour connecter à la base de données
    private static void InitializeDatabase()
    {
        var client = new MongoClient(connectionString);
        database = client.GetDatabase("progression"); // Nom de la base de données
    }

    // Méthode asynchrone pour sauvegarder le score
    public static async Task SaveScoreAsync(string playerName, int score)
    {
        InitializeDatabase();
        var collection = database.GetCollection<BsonDocument>("progression"); // Collection progression

        // Crée un document JSON pour le score
        var scoreDocument = new BsonDocument
        {
            { "game", playerName + "_fall_guy" },
            { "score", score },
            { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        // Insère le document dans la base de données
        await collection.InsertOneAsync(scoreDocument);
        UnityEngine.Debug.Log("Score saved to MongoDB Atlas successfully!");
    }

    public string Stringify()
    {
        return JsonUtility.ToJson(this);
    }

    public static PlayerData Parse(string json)
    {
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
