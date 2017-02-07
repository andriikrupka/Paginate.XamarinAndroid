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
    public class DefaultLoadingListItemSpanLookup : LoadingListItemSpanLookup
    {
        private int loadingListItemSpan;

        public DefaultLoadingListItemSpanLookup(RecyclerView.LayoutManager layoutManager)
        {
            if (layoutManager is GridLayoutManager) {
                // By default full span will be used for loading list item
                loadingListItemSpan = ((GridLayoutManager)layoutManager).SpanCount;
            } else {
                loadingListItemSpan = 1;
            }
        }

        public override int getSpanSize()
        {
            return loadingListItemSpan;
        }
    }
}