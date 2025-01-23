namespace ASM_C_4.Models
{
    public class Paginate
    {
        public int TotalItems {  get; private set; } // tong so Item
        public int PageSize { get; private set; } // Tong so Item/trang
        public int CurrentPage { get; private set; } // Trang hien tai
        public int TotalPages { get; private set; } // Tong trang 
        public int StartPage { get; private set; } // trang bat dau
        public int EndPage { get; private set; } // trang ket thuc

        public Paginate()
        {

        }

        public Paginate(int totalItems, int page, int pageSize = 10) // 10 item/trang
        {

            // lam trong tong item/10 item tren 1 trang VD: 16 item / 10 = tron 2 trang
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            int currentPage = page;

            int startPage = currentPage - 5;

            int endPage = currentPage + 4;

            if (startPage <= 0)
            {
                //nếu số trang bắt đầu nhỏ hơn or = 0 thì số trang cuối sẽ bằng 
                endPage = endPage - (startPage - 1); //6 - (-3 - 1) = 10;
                startPage = 1;

            }
            if (endPage > totalPages) //nếu số page cuối > số tổng trang 
            {
                endPage = totalPages; //số page cuối = số tổng trang
                if (endPage > 10) //nếu số page cuối > 10
                {
                    startPage = endPage - 9; //trang bắt đầu = trang cuối - 9
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
