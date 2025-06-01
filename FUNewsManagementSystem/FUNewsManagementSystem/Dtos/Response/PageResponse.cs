namespace FUNewsManagementSystem.Dtos.Response
{
    public class PageResponse<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public long TotalElements { get; set; }
        public List<T>? Data { get; set; }
    }
}
