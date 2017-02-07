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
using Java.Lang;

namespace Paginate.Droid
{
    public class AbsWrapperAdapter : BaseAdapter, IWrapperListAdapter
    {
        private BaseAdapter wrappedAdapter;
        private AbsListViewLoadingListItemCreator loadingListItemCreator;
        private bool displayLoadingRow = true;

        public AbsWrapperAdapter(BaseAdapter wrappedAdapter, AbsListViewLoadingListItemCreator loadingListItemCreator)
        {
            this.wrappedAdapter = wrappedAdapter;
            this.loadingListItemCreator = loadingListItemCreator;
        }

        public override int Count
            => displayLoadingRow ? wrappedAdapter.Count + 1 : wrappedAdapter.Count;

        public IListAdapter WrappedAdapter => wrappedAdapter;

        public override Java.Lang.Object GetItem(int position)
        {
            return isLoadingRow(position) ? null : wrappedAdapter.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return isLoadingRow(position) ? -1 : wrappedAdapter.GetItemId(position);
        }

        public override int GetItemViewType(int position)
        {
            return isLoadingRow(position) ? ViewTypeCount - 1 : wrappedAdapter.GetItemViewType(position);
        }

        public override int ViewTypeCount
        {
            get
            {
                return displayLoadingRow ? wrappedAdapter.ViewTypeCount + 1 : wrappedAdapter.ViewTypeCount;
            }
        }

        public override bool IsEnabled(int position)
        {
            return !isLoadingRow(position) && wrappedAdapter.IsEnabled(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (isLoadingRow(position))
            {
                if (convertView == null)
                {
                    convertView = loadingListItemCreator.newView(position, parent);
                }
                loadingListItemCreator.bindView(position, convertView);
                return convertView;
            }
            else
            {
                return wrappedAdapter.GetView(position, convertView, parent);
            }
        }


        public void SetDisplayLoadingRow(bool displayLoadingRow)
        {
            if (this.displayLoadingRow != displayLoadingRow)
            {
                this.displayLoadingRow = displayLoadingRow;
                NotifyDataSetChanged();
            }
        }

        bool isLoadingRow(int position)
        {
            return displayLoadingRow && position == getLoadingRowPosition();
        }

        private int getLoadingRowPosition()
        {
            return displayLoadingRow ? Count - 1 : -1;
        }
    }
}