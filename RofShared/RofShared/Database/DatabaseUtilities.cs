using System;

namespace RofShared.Database
{
    public static class DatabaseUtilities
    {
        public static int GetTotalPages(int numOfRecords, int pageSize, int pageRequested)
        {
            var numOfFullPages = numOfRecords / pageSize; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
            var remaining = numOfRecords % pageSize; //remaining will be 3 which will be an extra page

            var totalPages = (remaining > 0) ? numOfFullPages + 1 : numOfFullPages;  //therefore total pages is sum of full pages plus one more page is any remains.

            if (pageRequested > totalPages)
            {
                throw new Exception("Page requested is more than total number of pages");
            }

            return totalPages;
        }


    }
}
