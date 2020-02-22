using Amazon.DynamoDBv2.DataModel;

namespace pomodayo.server.Models
{
    [DynamoDBTable("Auth")]
    public class UserModel
    {
        [DynamoDBHashKey("Username")]
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
