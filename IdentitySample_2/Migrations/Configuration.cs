namespace IdentitySample_2.Migrations
{
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentitySample_2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(IdentitySample_2.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            //insert role
            context.Roles.AddOrUpdate(new IdentityRole { Name="admin" });
            
            //insert user
            context.Users.AddOrUpdate(
              p => p.UserName,
              new Models.ApplicationUser { UserName = "sooraj" }
            );
            context.SaveChanges();
            //assign the role to the user
            var userRecord = context.Users.Where(m => m.UserName == "sooraj").FirstOrDefault();
            var Role = context.Roles.Where(m => m.Name == "admin").FirstOrDefault();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            userManager.AddToRole(userRecord.Id, Role.Name);
            context.SaveChanges();






        }
    }
}
