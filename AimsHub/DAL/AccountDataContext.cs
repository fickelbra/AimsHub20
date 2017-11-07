using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AimsHub.Models;

namespace AimsHub.DAL
{
    public class AccountDataContext : DbContext
    {

        public AccountDataContext()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<AccountModel> AccountViewModels { get; set; }
    }
}