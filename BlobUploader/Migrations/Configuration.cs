using System.Data.Entity.Migrations;
using BlobUploader.Models;
using BlobUploader.WebSec;

namespace BlobUploader.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UsersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UsersContext context)
        {
            WebSecurityInitializer.Instance.EnsureInitialize();
        }
    }
}
