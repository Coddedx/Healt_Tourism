using Microsoft.EntityFrameworkCore;

namespace Plastic.Models
{
    public class PaginetedList<T>  //generic class-->>  allowing a create a class that can work with different data types without specifying those types at compile time
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; } 
        public int PageIndex { get; set; }  //o anki açık sayfanın numarası
        public int PageSize { get; set; } //her sayfada gösterilcek veri sayısı
        public int TotalPages { get; private set; }

        public PaginetedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            Items = items;
            TotalPages =(int)Math.Ceiling( count/ (double)pageSize);
        }

        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPAge => (PageIndex < TotalPages);
        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1 ;
        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalPages);

        //public static async Task<PaginetedList<T>> CreateAsync(List<T> source, int pageIndex, int pageSize)  //list yerine ıqueryable yazmıştı ama ben veri tabanındaki tüm verileri çekiceğim için list yaptım 
        //    //pageIndex- current page number retrieve(geri almak),  pageSize-number of items to display per page
        //{
        //    var count = await source.CountAsync();  //asynchronously returns to number of elements in a sequence
        //}

    }
}
