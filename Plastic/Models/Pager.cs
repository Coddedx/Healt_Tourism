using Microsoft.EntityFrameworkCore;

namespace Plastic.Models
{
    public class Pager  //<T> generic class-->>  allowing a create a class that can work with different data types without specifying those types at compile time
    {
        // public List<T> Items { get; set; }
        //public int TotalItems { get; set; }
        //public int CurrentPage { get; set; }
        //public int PageSize { get; set; } //her sayfada gösterilcek veri sayısı
        //public int TotalPages { get; private set; }
        //public int StartPage { get; private set; }
        //public int EndPage { get; private set; }


        //public Pager()
        //{

        //}
        //public Pager(int totalItems, int page, int pageSize = 5)
        //{
        //    int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        //    int currenPage = page;

        //    int startPage = CurrentPage - 5;
        //    int endPage = CurrentPage + 4;

        //    if (startPage <= 0)
        //    {
        //        endPage = endPage - (startPage - 1);
        //        startPage = 1;
        //    }

        //    if (endPage > totalPages)
        //    {
        //        if (endPage > 10)
        //        {
        //            startPage = endPage - 9;
        //        }
        //    }

        //    TotalItems = totalItems;
        //    CurrentPage = currenPage;
        //    PageSize = pageSize;
        //    TotalPages = totalPages;
        //    StartPage = startPage;
        //    EndPage = endPage;
        //}


        //public int PageIndex { get; set; }  //o anki açık sayfanın numarası

        //public PaginetedList(List<T> items, int count, int pageIndex, int pageSize)
        //{
        //    PageIndex = pageIndex;
        //    //Items = items;
        //    TotalPages =(int)Math.Ceiling( count/ (double)pageSize);TotalPages =(int)Math.Ceiling( count/ (double)pageSize);
        //}





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
