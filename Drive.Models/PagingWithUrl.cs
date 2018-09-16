using System;
using System.Collections.Generic;
using XWidget.Linq;

namespace Drive.Models {
    public class PagingWithUrl<TSource> : Paging<TSource> {
        public string Next { get; set; }

        public PagingWithUrl(IEnumerable<TSource> source, int skip, int take) : base(source, skip, take) { }
    }
}
