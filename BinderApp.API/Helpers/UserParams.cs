namespace BinderApp.API.Helpers
{
    public class UserParams
    {
        //Max page size allowed
        private const int MaxPageSize = 50;

        //Default page size
        private int pageSize = 10;

        //Default page number
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get { return pageSize; }

            set 
            { 
              pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public int UserId { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 99;
        
    }
}