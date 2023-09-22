using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Exceptions
{
    public class BookStoreNotFoundException : Exception
    {
        public BookStoreNotFoundException() : base() { }

        public BookStoreNotFoundException(string message) : base(message) { }

        public BookStoreNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
