using Shared;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace MyApp.Data
{    
    public class DatabaseRepository
    {

        SQLiteAsyncConnection Connection { get; set; }

        public static DatabaseRepository Instance { get; } = new DatabaseRepository();
              
        public DatabaseRepository()
        {
            Connection = new SQLiteAsyncConnection(
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "local.db"));
        }
        public async Task InitAsync()
        {
            await Connection.CreateTableAsync<Customer>();
        }

        public Task<List<Customer>> FindAsync(Expression<Func<Customer, bool>> expr)
        {
            return Connection.Table<Customer>().Where(expr).ToListAsync();
        }

        public async Task InsertAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Id))
                customer.Id = Guid.NewGuid().ToString();

            customer.LastUpdated = DateTime.UtcNow;

            await Connection.InsertAsync(customer);
        }

        public async Task UpdateAsync(Customer customer)
        {
            customer.LastUpdated = DateTime.UtcNow;

            await Connection.UpdateAsync(customer);
        }

        internal async Task<bool> Exists(string id)
        {
            return (await Connection.Table<Customer>().Where(x => x.Id == id).CountAsync()) == 1;
        }

        public async Task DeleteAsync(Customer customer)
        {
            customer.LastUpdated = DateTime.UtcNow;
            customer.Deleted = DateTime.UtcNow;

            await Connection.UpdateAsync(customer);
        }
    }
}
