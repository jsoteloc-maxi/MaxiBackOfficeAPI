namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    public class PaginationResponse<T>
    {
        public T Data { get; set; }
        public Pagination Pagination { get; set; }
        public List<ErrorDto> Error { get; set; }

    }
}
