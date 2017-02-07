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

        abstract public void SetHasMoreDataToLoad(bool hasMoreDataToLoad);
        abstract public void Unbind();

        public static RecyclerPaginate.PaginateBuilder With(RecyclerView recyclerView, Callbacks callback)
        {
            return new RecyclerPaginate.PaginateBuilder(recyclerView, callback);
        }

        //public static AbsListViewPaginate.Builder with(AbsListView absListView, Callbacks callback)
        //{
        //    return new AbsListViewPaginate.Builder(absListView, callback);
        //}
    }
}