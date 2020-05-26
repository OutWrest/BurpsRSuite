using BurpsRSuite.Controllers;
using BurpsRSuite.Models;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BurpsRSuite.Data
{
    public class BurpsRSuiteInitialization
    {
        public async static void ResetDatabase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Deletes all past users and roles from previous session

            List<Task> Tasks = new List<Task>();
            foreach (ApplicationUser user_to_be_deleted in context.Users)
            {
                Tasks.Add(userManager.DeleteAsync(user_to_be_deleted));
            
            }

            foreach (IdentityRole role_to_be_deleted in roleManager.Roles.ToList())
            {
                Tasks.Add(roleManager.DeleteAsync(role_to_be_deleted));
            }

            Task.WaitAll(Tasks.ToArray());

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

            // Create a normal admin user

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin",
                FirstName = "name",
                Email = "admin@admin.com",
                AccountNumber = "1337",
                Answer1 = "a",
                Answer2 = "a"
            };

            Tasks.Add(userManager.CreateAsync(adminUser, "T0tally1337Pa$$").ContinueWith(task =>
                userManager.AddToRoleAsync(adminUser, AuthorizationRoles.Administrator)));

            Task.WaitAll(Tasks.ToArray());

            // Create a secure user

            Random random = new Random();

            byte[] tfUser = new byte[16];
            random.NextBytes(tfUser);


            ApplicationUser secureUser = new ApplicationUser
            {
                UserName = "secureUser",
                FirstName = "Secure",
                LastName = "User",
                Email = "user@protonmail.com",
                AccountNumber = random.Next(0, 999999999).ToString("000000000"),
                Answer1 = AdminController.RandomString(5),
                Answer2 = AdminController.RandomString(5),
                TotpEnabled = true,
                TotpSecret = tfUser
            };

            Tasks.Add(userManager.CreateAsync(secureUser, AdminController.RandomString(5)).ContinueWith(task =>
                userManager.AddToRoleAsync(secureUser, AuthorizationRoles.User)));


            Task.WaitAll(Tasks.ToArray());

            // Create a super secure user

            byte[] tfUser2 = new byte[16];
            random.NextBytes(tfUser2);


            ApplicationUser secureUser2 = new ApplicationUser
            {
                UserName = "secureUser" + random.Next(0,99).ToString("00"),
                FirstName = "VerySecure",
                LastName = "User",
                Email = "secureuser@protonmail.com",
                AccountNumber = random.Next(0, 999999999).ToString("000000000"),
                Answer1 = AdminController.RandomString(5),
                Answer2 = AdminController.RandomString(5),
                TotpEnabled = true,
                TotpSecret = tfUser
            };

            Tasks.Add(userManager.CreateAsync(secureUser2, AdminController.RandomString(5)).ContinueWith(task =>
                userManager.AddToRoleAsync(secureUser2, AuthorizationRoles.User)));


            Task.WaitAll(Tasks.ToArray());

            // Create a secure admin user

            byte[] tfAdmin = new byte[16];
            random.NextBytes(tfAdmin);

            ApplicationUser secureAdmin = new ApplicationUser
            {
                UserName = "secureAdmin",
                FirstName = "Secure",
                LastName = "Admin",
                Email = "admin@protonmail.com",
                AccountNumber = random.Next(0, 999999999).ToString("000000000"),
                Answer1 = AdminController.RandomString(5),
                Answer2 = AdminController.RandomString(5),
                TotpEnabled = true,
                TotpSecret = tfAdmin
            };

            Tasks.Add(userManager.CreateAsync(secureAdmin, AdminController.RandomString(5)).ContinueWith(task =>
                userManager.AddToRoleAsync(secureAdmin, AuthorizationRoles.Administrator)));

            Task.WaitAll(Tasks.ToArray());


            await context.AddAsync(new Item
            {
                Author = adminUser,
                Description = "test"
            });


            foreach (Item item in context.Items)
            {
                Console.WriteLine(item);
                context.Remove(item);
            }










        }

    }
}