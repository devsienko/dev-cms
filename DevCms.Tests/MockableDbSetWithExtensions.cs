using Microsoft.EntityFrameworkCore;

namespace DevCms.Tests
{
    public abstract class MockableDbSetWithExtensions<T> : DbSet<T>
        where T : class
    {
        public abstract void AddOrUpdate(params T[] entities);
    }
}
