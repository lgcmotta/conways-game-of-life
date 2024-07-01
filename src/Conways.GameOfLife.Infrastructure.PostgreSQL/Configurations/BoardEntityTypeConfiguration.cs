using Conways.GameOfLife.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conways.GameOfLife.Infrastructure.PostgreSQL.Configurations;

public class BoardEntityTypeConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(board => board.Id);

        builder.Property(board => board.Id)
            .ValueGeneratedOnAdd();
        
        builder.HasMany<Generation>("_generations")
            .WithOne()
            .HasForeignKey("BoardId");

        builder.Property<DateTime>("CreatedAt")
            .IsRequired();
        
        builder.Property<DateTime?>("UpdatedAt")
            .IsRequired(false);
        
        builder.Ignore(board => board.Generations);
        
        builder.Ignore(board => board.CurrentGeneration);
    }
}