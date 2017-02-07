using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public class WrapperSpanSizeLookup : GridLayoutManager.SpanSizeLookup
    {
        private GridLayoutManager.SpanSizeLookup wrappedSpanSizeLookup;
        private LoadingListItemSpanLookup loadingListItemSpanLookup;
        private WrapperAdapter wrapperAdapter;

        public WrapperSpanSizeLookup(GridLayoutManager.SpanSizeLookup gridSpanSizeLookup,
                                     LoadingListItemSpanLookup loadingListItemSpanLookup,
                                     WrapperAdapter wrapperAdapter)
        {
            this.wrappedSpanSizeLookup = gridSpanSizeLookup;
            this.loadingListItemSpanLookup = loadingListItemSpanLookup;
            this.wrapperAdapter = wrapperAdapter;
        }

        public override int GetSpanSize(int position)
        {
            return wrapperAdapter.IsLoadingRow(position)
                  ? loadingListItemSpanLookup.GetSpanSize() : wrappedSpanSizeLookup.GetSpanSize(position);
        }

        public GridLayoutManager.SpanSizeLookup getWrappedSpanSizeLookup()
        {
            return wrappedSpanSizeLookup;
        }
    }
}