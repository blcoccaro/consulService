using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using consulService.DTOs;

namespace consulService.Memory
{    
    public class Config : DbContext
    {
        public Config(DbContextOptions<Config> options)
            : base(options)
        {
        }
        
        public DbSet<Consul> List { get; set; }

        public void Clear() {
            List.RemoveRange(List);
            this.SaveChanges();
        }
        public Consul Get(string key) {
            var value = this.List.FirstOrDefault(o=>o.Key == key);

            return value;
        }
        public void Set(Consul save) {
            var obj = this.List.FirstOrDefault(o=>o.Key == save.Key);
            
            if (obj != null) {
                List.Remove(obj);
            }
            this.SaveChanges();

            List.Add(save);

            this.SaveChanges();
        }        
    }
}