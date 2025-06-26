
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todo_web_api.Models;

namespace todo_web_api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOption) : base(dbContextOption)
        {
        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<CloudinaryImage> cloudinaryImages { get; set; }
        public DbSet<UploadResult> UploadResults { get; set; }
    }
}
