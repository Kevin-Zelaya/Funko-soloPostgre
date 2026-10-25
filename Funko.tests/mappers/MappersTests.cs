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
            Category : new CategoryUpdateRequestDto
            (
                Name : "Superheroes",
                Description : "Funko de superheroes"
            ),
            SupplierId : 1
            
        );
        // Arc
        var funko = create.ToModel();
        funko.Supplier =  new Supplier
            {
                Name = "Proveedor de prueba"
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