using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Serilog;

public static class MongoContextConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        Log.Debug(
            "[DBCONTEXT-CONFIG] Configurando entidades para MongoDB");

        // FUNKO
        modelBuilder.Entity<Funko>(entity =>
        {
            ///<summary> Funko como collección </summary>
            entity.ToCollection("funkos");
            ///<summary> llave id  </summary>
            entity.HasKey(f => f.Id);
            ///<summary> nombre de la clave </summary>
            entity.Property(f => f.Id)
                .HasElementName("_id");
            ///<summary> Nombre como indice </summary>
            entity.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);
            ///<summary> Eliminado por defecto en false </summary>
            entity.HasIndex(f => f.Name)
                .IsUnique();
            ///<summary> Largo maximo de 100 </summary>
            entity.Property(f => f.Franchise)
                .HasMaxLength(100);
            ///<summary> Número no es requerido </summary>
            entity.Property(f => f.Number)
                .IsRequired();
            ///<summary> Precio es requerido </summary>
            entity.Property(f => f.Price)
                .IsRequired();
            ///<summary> Exclusivo falso por defecto </summary>
            entity.Property(f => f.Exclusive)
                .HasDefaultValue(false);
            ///<summary> Eliminado por defecto en false </summary>
            entity.Property(f => f.IsDelete)
                .HasDefaultValue(false);
            ///<summary> Fecha de creación </summary>
            entity.Property(f => f.CreatedAt)
                .IsRequired();
            ///<summary> fecha de actualización inicializa en null </summary>
            entity.Property(f => f.UpdatedAt)
                .IsRequired(false);
            ///<summary> fecha de eliminación inicializa en null </summary>
            entity.Property(f => f.DeleteAT)
                .IsRequired(false);
            ///<summary> Filtro para no mostrar eliminado </summary>
            entity.HasQueryFilter(f => !f.IsDelete);
        });

        // SUPPLIER
        modelBuilder.Entity<Supplier>(entity =>
        {
            ///<summary> Proveedores como collección </summary>
            entity.ToCollection("suppliers");
            ///<summary> llave id  </summary>
            entity.HasKey(s => s.Id);
            ///<summary> nombre de la clave </summary>
            entity.Property(s => s.Id)
                .HasElementName("_id");
            ///<summary> Nombre es requerido </summary>
            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
            ///<summary> Nombre como indice </summary>
            entity.HasIndex(s => s.Name);
            ///<summary> Eliminado por defecto en false </summary>
            entity.Property(s => s.IsDeleted)
                .HasDefaultValue(false);
            ///<summary> Fecha de creación </summary>
            entity.Property(s => s.CreateAt)
                .IsRequired();
            ///<summary> fecha de actualización inicializa en null </summary>
            entity.Property(s => s.UpdateAt)
                .IsRequired(false);
            ///<summary> fecha de eliminación inicializa en null </summary>
            entity.Property(s => s.DeleteAt)
                .IsRequired(false);
            ///<summary> Conrol de concurrencia </summary>
            // entity.Property(s => s.RowVersion)
            //     .IsRowVersion(); // cuando se agreguen el resto de metodos
            ///<summary> Filtro para no mostrar eliminado </summary>
            entity.HasQueryFilter(s => !s.IsDeleted);

            if(!AppConfig.shouldRecreateDb) return;
            entity.HasData(
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Funko Europe", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Collector Store", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Pop In A Box", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Entertainment Earth", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "BigBadToyStore", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Amazon Collectibles", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Geek Store", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Comic World", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Pop Culture Shop", IsDeleted = false },
                new Supplier { Id = ObjectId.GenerateNewId().ToString(), Name = "Collector's Corner", IsDeleted = false }
            );

            Log.Information(
                "[MONGODB] Se han insertado {Count} proveedores iniciales",
                10);
        });

        
    }

}