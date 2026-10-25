public static class FunkoError
{
    public static NotFoundError NotFound(string id) => NotFoundError.FromResource("Funko", id);
    public static ConflictError NombreDuplicado(string nombre) => ConflictError.Duplicate("Funko", nombre);
}