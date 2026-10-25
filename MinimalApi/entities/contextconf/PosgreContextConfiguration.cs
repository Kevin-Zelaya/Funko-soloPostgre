using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Serilog;

public abstract class PosgreContextConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        Log.Debug("[DBCONTEXT-CONFIG] Configurando entidades para el proveedor 'PosgreSql'");
        modelBuilder.Entity<Funko>(entity =>
        {
            entity.HasKey(u => u.Id);

            ///<summary> Nombre como indice </summary>
            entity.HasIndex(p => p.Name)
            .IsUnique();
            entity.Property(p => p.Name)
            .HasMaxLength(100);

            ///<summary> Franquicia </summary>
            entity.Property(p => p.Franchise).HasMaxLength(100);
            ///<summary> número </summary>
            entity.Property(p => p.Number).IsRequired();
            ///<summary> Exclusivo valor por defecto false </summary>
            entity.Property(p => p.Exclusive).HasDefaultValue(false);
            ///<summary> Precio requerido </summary>
            entity.Property(p => p.Price).IsRequired(true);
            ///<summary> Eliminado, valor por defecto false </summary>
            entity.Property(p => p.IsDelete).HasDefaultValue(false);
            ///<summary> Fecha de creado </summary>
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            ///<summary> Fecha de actualizado </summary>
            entity.Property(p => p.UpdatedAt).IsRequired(false).HasDefaultValue(null);
            ///<summary> Fecha de eliminado </summary>
            entity.Property(p => p.DeleteAT).IsRequired(false).HasDefaultValue(null);
            ///<summary> Versión de la fila para control de concurrencia </summary>
            entity.Property(s => s.RowVersion).IsRowVersion();
            //<summary> Conversión de categoría a json </summary>
            entity.ComplexProperty(
                f => f.Category,
                category => Microsoft.EntityFrameworkCore.RelationalComplexPropertyBuilderExtensions
                    .ToJson(category)
            );

            //<summary> Relación con categoria </summary>
            entity.HasOne(f => f.Supplier)
                .WithMany(s => s.Funkos)
                .HasForeignKey(f => f.SupplierId);

            entity.HasQueryFilter(f => !f.IsDelete);
        });
        modelBuilder.Entity<Supplier>(entity =>
        {
            ///<summary> Key de categoria se genera automaticamente </summary>
            entity.HasKey(s => s.Id);
            entity.Property(s=> s.Id)
                .HasValueGenerator<YoutubeIdValueGenerator>()
                .ValueGeneratedOnAdd();
            ///<summary> 
            ///     Nombre como indice 
            ///     requerido y largo máximo de 100
            /// </summary>
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(s => s.Name);
            ///<summary> Eliminado lógico, falso por defecto </summary>
            entity.Property(s=>s.IsDeleted).HasDefaultValue(false);
            ///<summary> Fecha de creación valor por defecto "ahora" </summary>
            entity.Property(s=>s.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            ///<summary> Fecha de actualización valor por defecto "ahora" </summary>
            entity.Property(s=>s.UpdateAt).IsRequired(false).HasDefaultValue(null);
            ///<summary> Fecha de eliminado </summary>
            entity.Property(s => s.DeleteAt).IsRequired(false).HasDefaultValue(null);
            ///<summary> Filtro </summary>
            entity.HasQueryFilter(f => !f.IsDeleted);

            if(!AppConfig.shouldRecreateDb) return;
            ///<summary> Semíllas </summary>
            entity.HasData(
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Funko Europe", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Collector Store", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Pop In A Box", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Entertainment Earth", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "BigBadToyStore", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Amazon Collectibles", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Geek Store", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Comic World", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Pop Culture Shop", IsDeleted = false },
                new Supplier { Id = YoutubeIdValueGenerator.GenerateId(), Name = "Collector's Corner", IsDeleted = false }
            );

            Log.Information(
                "[MONGODB] Se han insertado {Count} proveedores iniciales",
                10);
        });
    }
    
}