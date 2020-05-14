using BurpsRSuite.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BurpsRSuite.Data
{
    public class BurpsRSuiteInitialization
    {
        public string test = "test";
        public async static void ResetDatabase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Deletes all past users from previous session
            List<Task> Tasks = new List<Task>();
            foreach (ApplicationUser user_to_be_deleted in context.Users)
            {
                Tasks.Add(userManager.DeleteAsync(user_to_be_deleted));
            }

            // Create Roles

            foreach (string role in AuthorizationRoles.AllRoles)
            {
                Tasks.Add(roleManager.CreateAsync(new IdentityRole(role)));
            }


            // Create a normal user

            ApplicationUser user = new ApplicationUser
            {
                UserName = "user",
                FirstName = "name",
                Email = "user@aol.com",
                AccountNumber = "123",
                Answer1 = "a",
                Answer2 = "a"
            };

            Tasks.Add(userManager.CreateAsync(user, "asd").ContinueWith(task =>
                userManager.AddToRoleAsync(user, AuthorizationRoles.User)));

            Task.WaitAll(Tasks.ToArray());

            // Create admin user

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin",
                FirstName = "name",
                Email = "admin@admin.com",
                AccountNumber = "1337",
                Answer1 = "a",
                Answer2 = "a"
            };

            await userManager.CreateAsync(adminUser, "T0tally1337Pa$$");
            await userManager.AddToRoleAsync(adminUser, AuthorizationRoles.Administrator);
        }
        
    }
}
