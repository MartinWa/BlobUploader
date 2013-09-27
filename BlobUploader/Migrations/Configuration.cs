using AdminAppTest.WebSec;

namespace AdminAppTest.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.UsersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.UsersContext context)
        {
            WebSecurityInitializer.Instance.EnsureInitialize();
        }
    }
}
