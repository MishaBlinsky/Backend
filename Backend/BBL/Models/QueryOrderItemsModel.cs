namespace Backend.BBL.Models
{
    public class QueryOrderItemsModel
    {
        public long[] Ids { get; set; }

        public long[] CustomerIds { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public bool IncludeOrderItems { get; set; }
    }
}