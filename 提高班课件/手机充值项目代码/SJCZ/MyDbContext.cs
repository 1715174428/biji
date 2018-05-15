using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SJCZ
{
    public class MyDbContext:DbContext
    {
        public MyDbContext():base("name=conn1")
        {

        }

        public DbSet<HFOrder> HFOrders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var eOrder = modelBuilder.Entity<HFOrder>();
            eOrder.ToTable("T_HFOrders");
            eOrder.Property(e => e.Version).IsRowVersion();
        }
    }
}