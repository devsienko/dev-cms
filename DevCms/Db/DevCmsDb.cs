using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DevCms.Db;
using DevCms.ContentTypes;

namespace DevCms.Models
{
    public class DevCmsDb : DbContext
    {
        public DevCmsDb()
        {
        }

        public DevCmsDb(DbContextOptions<DevCmsDb> options)
            : base(options)
        { }
        
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<EntityType> ContentTypes { get; set; }
        public virtual DbSet<Attribute> ContentAttrs { get; set; }
        public virtual DbSet<Entity> Content { get; set; }
        public virtual DbSet<AttrValue> AttrValues { get; set; }
        public virtual DbSet<Dictionary> Dictionaries { get; set; }
        public virtual DbSet<DictionaryItem> DictionaryItems { get; set; }
        public virtual DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}