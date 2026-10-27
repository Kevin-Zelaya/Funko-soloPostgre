public class MappersTests
{
    
    [Test]
    public void AllMappers_InOneTest()
    {
        // Arrange
        var create = new FunkoCreateRequestDto
        (
            Name : "Spider-Man",
            Franchise : "Marvel",
            Number : 100,
            Exclusive : false,
            Price : (double)15.99m,
            CategoryId : Guid.Parse("10000000-0000-0000-0000-000000000000"),
            SupplierId : "dsaf5623d"
            
        );
        // Arc
        var funko = create.ToModel();
        funko.Supplier =  new Supplier
        {
            Id = "4563asdfe",
            Name = "Proveedor de prueba",
            RowVersion = 954
        };
        funko.Category = new Category
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
            Name = "Prueba",
            RowVersion = 954
        };
        var dto = funko.ToDto();
        var supplier = funko.Supplier.ToDto();
        var category = funko.Category.ToDto();
        // Assett
        Assert.That(create.Name, Is.EqualTo(funko.Name));
        Assert.That(dto.Name, Is.EqualTo(funko.Name));
        Assert.That(supplier.Name, Is.EqualTo(funko.Supplier.Name));
        Assert.That(category.Name, Is.EqualTo(funko.Category.Name));
    }
}