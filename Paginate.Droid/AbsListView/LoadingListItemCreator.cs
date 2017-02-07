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

namespace Paginate.Droid
{
    public abstract class AbsListViewLoadingListItemCreator
    {
        public abstract View newView(int position, ViewGroup parent);
        public abstract void bindView(int position, View view);

        private static Lazy<DefaultAbsListViewLoadingListItemCreator> lazyDefaultCreator = new Lazy<DefaultAbsListViewLoadingListItemCreator>();

        public static AbsListViewLoadingListItemCreator Default => lazyDefaultCreator.Value;

        private class DefaultAbsListViewLoadingListItemCreator : AbsListViewLoadingListItemCreator
        {
            public override void bindView(int position, View view)
            {
                // No binding for default loading row
            }

            public override View newView(int position, ViewGroup parent)
            {
                return LayoutInflater.From(parent.Context).Inflate(Resource.Layout.loading_row, parent, false);
            }
        }
    }
}