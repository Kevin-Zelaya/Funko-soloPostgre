public class Supplier
{
    public string Id {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public bool IsDeleted {get; set;}
    public DateTime CreateAt {get; set;}
    public DateTime? DeleteAt {get; set;}
    public DateTime? UpdateAt {get; set;}
    public ICollection<Funko> Funkos = []; 
    // Control de concurrencia
    // public uint RowVersion {get; set;}

}