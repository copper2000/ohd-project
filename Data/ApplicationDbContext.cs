using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OHD.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }        

        public DbSet<Facility> Facility { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Reason> Reason { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<Severity> Severity { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
