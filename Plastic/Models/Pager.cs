using Microsoft.EntityFrameworkCore;

namespace Plastic.Models
{
    public class Pager  //<T> generic class-->>  allowing a create a class that can work with different data types without specifying those types at compile time
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager(int totalItems, int currentPage, int pageSize = 10)
        {
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            StartPage = Math.Max(1, currentPage - 5);
            EndPage = Math.Min(TotalPages, currentPage + 4);

            if (TotalPages <= 10)
            {
                StartPage = 1;
                EndPage = TotalPages;
            }
        }

    }
}
