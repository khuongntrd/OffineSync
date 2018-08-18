using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace MyApi.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(c =>
            {
                c.HasKey(x => x.Id);
            });

            base.OnModelCreating(modelBuilder);
        }

        internal void InsertCustomer(Customer customer)
        {            
            if (customer.LastUpdated == DateTime.MinValue)
                customer.LastUpdated = DateTime.UtcNow;
            else
                customer.ClientLastUpdated = customer.LastUpdated;

            Add(customer);

            SaveChanges();
        }

        internal void UpdateCustomer(Customer clientModel)
        {
            if (clientModel.Deleted.HasValue)
            {
                var serverModel = Customers.Single(x => x.Id == clientModel.Id);

                serverModel.Deleted = clientModel.Deleted;
                serverModel.LastUpdated = clientModel.LastUpdated;
            }
            else
            {
                var serverModel = Customers.Single(x => x.Id == clientModel.Id);

                if (serverModel.ClientLastUpdated > clientModel.LastUpdated)
                {
                    // Conflict 
                    // Here you can just do a server, or client wins scenario, on a whole row basis. 
                    // E.g take the servers word or the clients word

                    // e.g. Server - wins - Ignore changes and just update time.
                    serverModel.LastUpdated = DateTime.UtcNow;
                    serverModel.ClientLastUpdated = clientModel.LastUpdated;
                }
                else // Client is new than server
                {
                    serverModel.Firstname = clientModel.Firstname;
                    serverModel.Lastname = clientModel.Lastname;
                    serverModel.Latitude = clientModel.Latitude;
                    serverModel.Longitude = clientModel.Longitude;
                    serverModel.Picture = clientModel.Picture;
                    serverModel.LastUpdated = DateTime.UtcNow;
                    serverModel.ClientLastUpdated = clientModel.LastUpdated;
                }
            }

            SaveChanges();
        }
    }
}
