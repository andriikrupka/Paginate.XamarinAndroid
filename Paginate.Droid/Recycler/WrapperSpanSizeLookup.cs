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
            if (wrapperAdapter.isLoadingRow(position))
            {
                return loadingListItemSpanLookup.getSpanSize();
            }
            else
            {
                return wrappedSpanSizeLookup.GetSpanSize(position);
            }
        }

        public GridLayoutManager.SpanSizeLookup getWrappedSpanSizeLookup()
        {
            return wrappedSpanSizeLookup;
        }
    }
}