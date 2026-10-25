public static class FunkoMapper
{
    
    public static Funko ToModel(this FunkoCreateRequestDto dto) => new()
    {
        Name = dto.Name,
        Category = new Category
        {
            Name = dto.Category.Name,
            Description = dto.Category.Description
        },
        Exclusive = dto.Exclusive,
        Number = dto.Number,
        SupplierId = dto.SupplierId,
        Price = dto.Price,
        Franchise = dto.Franchise
    };

    public static Funko ToModel(this FunkoDto dto) => new()
    {
        Name = dto.Name,
        Category = new Category
        {
            Name = dto.Category.Name,
            Description = dto.Category.Description
        },
        Exclusive = dto.Exclusive,
        Number = dto.Number,
        SupplierId = dto.Supplier.Id,
        Price = dto.Price,
        Franchise = dto.Franchise,
        RowVersion = dto.RowVersion
    };
    public static FunkoDto ToDto(this Funko entity) => new FunkoDto(
        Id: entity.Id,
        Name: entity.Name,
        Category: new CategoryDto(
            Name: entity.Category.Name,
            Description: entity.Category.Description
        ),
        Exclusive: entity.Exclusive,
        Number: entity.Number,
        Price: entity.Price,
        Franchise: entity.Franchise,
        Supplier: new SupplierDto(
            Id: entity.Supplier.Id,
            Name: entity.Supplier.Name
        ),
        RowVersion : entity.RowVersion
    );



    


    public static SupplierDto ToDto(this Supplier entity) => new(
        Id: entity.Id,
        Name: entity.Name
    );
    public static CategoryDto ToDto(this Category category) => new(
        Name: category.Name,
        Description: category.Description
    );
}