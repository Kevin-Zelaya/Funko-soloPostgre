using System.Collections.ObjectModel;

public class Category
{
    public Guid Id {get; set;}
    public string Name {get; set;}= string.Empty;
    public string Description {get; set;} = string.Empty;
    public List<Funko> Funkos {get; set;} = new();
}