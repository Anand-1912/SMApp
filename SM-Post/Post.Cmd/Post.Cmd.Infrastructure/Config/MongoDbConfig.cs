namespace Post.Cmd.Infrastructure.Config;
public class MongoDbConfig
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string Collection { get; set; } = null!;

}
