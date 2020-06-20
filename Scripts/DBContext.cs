using Microsoft.EntityFrameworkCore;

namespace SocNetParser
{ 


    //настойка для бд 
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
            
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<SocialAccount> SocialAccount { get; set; }
        public virtual DbSet<Website> Website { get; set; }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryName> CategoryName { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseNpgsql("Host=ec2-54-217-213-79.eu-west-1.compute.amazonaws.com;Port=5432;Database=dfk02j1lhd6hlm;Username=kvjmykppnhsuqo;Password=41721ff1a9bf73399d063feb41574edd4b00ea810b6238cfade421c6e0b1147b;SslMode=Require;Trustservercertificate=true");
            }
        }

       

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum(null, "social_type", new[] { "facebook", "instagram", "vk", "ok" })
                .HasPostgresEnum(null, "status", new[] { "active", "closed" });



            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => new { e.IdOrg, e.IdCat })
                      .HasName("pk");

                entity.Property(e => e.IdOrg).HasColumnName("id_org");

                entity.Property(e => e.IdCat).HasColumnName("id_cat");

                entity.HasOne(d => d.IdCatNavigation)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.IdCat)
                    .HasConstraintName("Category_id_cat_fkey");

                entity.HasOne(d => d.IdOrgNavigation)
                    .WithMany(p => p.CategoryNavigation)
                    .HasForeignKey(d => d.IdOrg)
                    .HasConstraintName("Category_id_org_fkey");
            });

           
                modelBuilder.Entity<CategoryName>(entity =>
                {
                    entity.Property(e => e.Id).HasColumnName("id");

                    entity.Property(e => e.Name)
                        .HasColumnName("name")
                        .HasColumnType("character varying");
                });
          


            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.adress)
                    .HasColumnName("address")
                    .HasColumnType("character varying");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("character varying");

                entity.Property(e => e.Coordinates)
                    .HasColumnName("coordinates")
                    .HasColumnType("character varying");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("character varying");

                entity.Property(e => e.Organization).HasColumnName("organization");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.OrganizationNavigation)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.Organization)
                     .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Address_organization_fkey");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Closed).HasColumnName("closed");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Inn)
                    .HasColumnName("inn")
                    .HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Opened).HasColumnName("opened");
            });

            modelBuilder.Entity<SocialAccount>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Auditory).HasColumnName("auditory");

                entity.Property(e => e.LastUpdate).HasColumnName("last_update");

                

                entity.Property(e => e.Link)
                    .HasColumnName("link")
                    .HasColumnType("character varying");
                
                entity.Property(e => e.type).HasColumnName("type").HasColumnType("social_type");  

                entity.Property(e => e.Organization).HasColumnName("organization");

                entity.HasOne(d => d.OrganizationNavigation)
                    .WithMany(p => p.SocialAccount)
                    .HasForeignKey(d => d.Organization)
                     .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SocialAccount_organization_fkey");
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Domain)
                    .HasColumnName("domain")
                    .HasColumnType("character varying");

                entity.Property(e => e.LastUpdate).HasColumnName("last_update");

                entity.Property(e => e.LatestMd5)
                    .HasColumnName("latest_md5")
                    .HasColumnType("character varying");

                entity.Property(e => e.Organization).HasColumnName("organization");

                entity.Property(e => e.Prolongated).HasColumnName("prolongated");

                entity.Property(e => e.Registred).HasColumnName("registred");

                entity.Property(e => e.ServerCountry)
                    .HasColumnName("server_country")
                    .HasColumnType("character varying");
               
               
               
                entity.HasOne(d => d.OrganizationNavigation)
                    .WithMany(p => p.Website )
                    .HasForeignKey(d => d.Organization)
                     .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Website_organization_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
