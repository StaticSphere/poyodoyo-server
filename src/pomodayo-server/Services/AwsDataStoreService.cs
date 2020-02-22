using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using pomodayo.server.Models;
using pomodayo.server.Services.Contracts;

namespace pomodayo.server.Services
{
    public class AwsDataStoreService : IDataStoreService
    {
        private readonly AmazonDynamoDBClient _dataStore;

        public AwsDataStoreService(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
            if (hostingEnvironment.IsDevelopment())
            {
                var credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);

                _dataStore = new AmazonDynamoDBClient(credentials, region);
            }
            else
            {
                _dataStore = new AmazonDynamoDBClient(region);
            }
        }

        public async Task<UserModel> GetUserAsync(string username)
        {
            var context = new DynamoDBContext(_dataStore);
            return await context.LoadAsync<UserModel>(username).ConfigureAwait(false);
        }

        public async Task<UserModel> CreateUserAsync(string username, string password)
        {
            var user = await GetUserAsync(username).ConfigureAwait(false);
            if (user != null)
                return null;

            var context = new DynamoDBContext(_dataStore);
            user = new UserModel { UserName = username, Password = password };

            await context.SaveAsync(user).ConfigureAwait(false);

            return user;
        }
    }
}
