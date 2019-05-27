using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnSolve.ExchangeRateForecast.Core.Entities;


namespace OnSolve.ExchangeRateForecast.Infrastructure.Data
{
    public class ForecastContext : DbContext
    {
        public ForecastContext(DbContextOptions<ForecastContext> options) : base(options)
        {
        }

        public DbSet<CurrrencyItem> CurrrencyItems { get; set; }
        public DbSet<RateItem> RateItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CurrrencyItem>(ConfigureCurrency);
            builder.Entity<RateItem>(ConfigureRateItem);
        }
        private void ConfigureCurrency(EntityTypeBuilder<CurrrencyItem> builder)
        {
            builder.ToTable("Currency");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .IsRequired();

            builder.Property(cb => cb.Code)
                .IsRequired()
                .HasMaxLength(3);
        }
        private void ConfigureRateItem(EntityTypeBuilder<RateItem> builder)
        {
            builder.ToTable("RateCurrency");

            builder.Property(ci => ci.Id)
                .IsRequired();

            builder.Property(ci => ci.TimeStamp)
                .IsRequired(true);

            builder.Property(ci => ci.Rate)
                .IsRequired(true);

            builder.Property(ci => ci.CurrencyId)
               .IsRequired(true);

            builder.HasOne(ci => ci.CurrencyItem)
              .WithMany()
              .HasForeignKey(ci => ci.CurrencyId);
        }

    }
}
