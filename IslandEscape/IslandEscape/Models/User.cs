using System.Data.Entity;

namespace IslandEscape.Models
{
    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int AccessLevel { get; set; }

    }

    public class IslandEscapeDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}