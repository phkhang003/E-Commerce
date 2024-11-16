public interface IPaginationParameters
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
}

public interface ISortableParameters
{
    string? SortBy { get; set; }
    bool IsDescending { get; set; }
}

public interface IFilterableParameters
{
    string? SearchTerm { get; set; }
}
