using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public class DefaultLoadingListItemSpanLookup : LoadingListItemSpanLookup
    {
        private int loadingListItemSpan;

        public DefaultLoadingListItemSpanLookup(RecyclerView.LayoutManager layoutManager)
        {
            if (layoutManager is GridLayoutManager)
            {
                // By default full span will be used for loading list item
                loadingListItemSpan = ((GridLayoutManager)layoutManager).SpanCount;
            }
            else
            {
                loadingListItemSpan = 1;
            }
        }

        public override int GetSpanSize() => loadingListItemSpan;
    }
}