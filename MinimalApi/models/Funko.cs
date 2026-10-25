public class Funko
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Franchise { get; set; } = string.Empty;
    public int Number { get; set; }
    public string SupplierId {get; set;} = string.Empty;
    public Supplier? Supplier{get; set;}
    public Category Category { get; set; } = new();
    public bool Exclusive { get; set; }
    public double Price { get; set; }
    public bool IsDelete {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeleteAT {get; set;}
    // control de concurrencia
    public uint RowVersion {get; set;}
}
