using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public abstract class Paginate
    {
        public interface Callbacks
        {
            void onLoadMore();
            bool isLoading();
            bool hasLoadedAllItems();
        }

        abstract public void setHasMoreDataToLoad(bool hasMoreDataToLoad);
        abstract public void unbind();

        public static RecyclerPaginate.PaginateBuilder with(RecyclerView recyclerView, Callbacks callback)
        {
            return new RecyclerPaginate.PaginateBuilder(recyclerView, callback);
        }

        //public static AbsListViewPaginate.Builder with(AbsListView absListView, Callbacks callback)
        //{
        //    return new AbsListViewPaginate.Builder(absListView, callback);
        //}
    }
}