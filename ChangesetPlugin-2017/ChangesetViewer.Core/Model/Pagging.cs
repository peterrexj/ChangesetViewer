using ChangesetViewer.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.Model
{
    public class Paging
    {
        protected const int MaxPageSize = Consts.DefaultSearchPageSize;

        public Paging()
        {
            Page = 1;
            PageSize = SettingsStaticModelWrapper.SearchPageSize;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class PagingModel : Paging
    {
        private int _totalItems = 0;

        public int TotalItems
        {
            get
            {
                return _totalItems;
            }
            set
            {
                _totalItems = value;
            }
        }
        public int GetPageSize
        {
            get
            {
                if (_totalItems < Offset)
                    return _totalItems % PageSize;
                else
                    return PageSize;
            }
        }
        public int Offset
        {
            get
            {
                return Page * PageSize;
            }
        }
        public int TotalPagesAvailable
        {
            get
            {
                return (int)Math.Ceiling(Convert.ToDouble(TotalItems) / Convert.ToDouble(PageSize));
            }
        }
    }

    public static class PaggingExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> source, PagingModel paging)
        {
            return paging.Page > 1 ? source.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize) : source.Take(paging.PageSize);
        }
    }
}
