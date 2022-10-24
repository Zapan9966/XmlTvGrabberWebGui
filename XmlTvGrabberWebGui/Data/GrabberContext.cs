using Microsoft.EntityFrameworkCore;
using XmlTvGrabberWebGui.Models;

namespace XmlTvGrabberWebGui.Data
{
    public class GrabberContext : DbContext
    {
        public GrabberContext(DbContextOptions<GrabberContext> options)
            : base(options)
        {
        }

        public DbSet<Config> Configs { get; set; }
        public DbSet<XmlUrl> XmlUrls { get; set; }
        public DbSet<TvHeadendCategory> TvHeadendCategories { get; set; }
        public DbSet<XmlCategory> XmlCategories { get; set; }
        public DbSet<Trace> Traces { get; set; }
    }
}
