namespace ApiCondominio.Application.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int LinesPerPage { get; set; }
}
