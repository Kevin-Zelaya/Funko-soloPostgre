public record PaginatedResouce<T>(
    List<T> Items,
    int Count,
    int TotalPages,
    bool HasPreviusPage,
    bool HasNextPage
);