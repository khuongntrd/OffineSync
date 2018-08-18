using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Customer
    {
        public Customer()
        {
            LastUpdated = DateTime.UtcNow;
        }

#if CLIENT
        [SQLite.PrimaryKey]
        public string Id { get; set; }
#else
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
#endif
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Picture { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }

        // Timestamp // 
        public DateTime LastUpdated { get; set; }

#if !CLIENT
        public DateTime ClientLastUpdated { get; set; }        
#endif
        public DateTime? Deleted { get; set; }
    }
}
