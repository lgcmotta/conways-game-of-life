using Conways.GameOfLife.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conways.GameOfLife.Infrastructure.PostgresSQL.Configurations;

public class GenerationEntityTypeConfiguration : IEntityTypeConfiguration<Generation>
{
    public void Configure(EntityTypeBuilder<Generation> builder)
    {
        builder.ToTable("Generations");
        
        builder.Property<long>("Id")
            .ValueGeneratedOnAdd();
        
        builder.HasKey("Id");
        
        builder.Property<bool[,]>("_value")
            .HasColumnName("Value")
            .HasColumnType("BOOLEAN[][]")
            .IsRequired();

        builder.Property(generation => generation.Stable)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property<DateTime>("CreatedAt")
            .IsRequired();

    }
}