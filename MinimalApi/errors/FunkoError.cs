public static class FunkoError
{
    public static NotFoundError NotFound(long id) => NotFoundError.FromResource("Funko", id.ToString());
    public static ConflictError NombreDuplicado(string nombre) => ConflictError.Duplicate("Funko", nombre);
}