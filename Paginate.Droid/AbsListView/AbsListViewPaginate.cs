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
using Android.Database;

namespace Paginate.Droid
{
    public class AbsListViewPaginate : Paginate, EndScrollListener.Callback
    {
        private AbsListView absListView;
        private Callbacks callbacks;
        private EndScrollListener scrollListener;
        private AbsWrapperAdapter wrapperAdapter;
        private AbsPaginateDataSetObserver dataSetObserver;

        public AbsListViewPaginate(AbsListView absListView,
                        Paginate.Callbacks callbacks,
                        int loadingTriggerThreshold,
                        AbsListView.IOnScrollListener onScrollListener,
                        bool addLoadingListItem,
                        AbsListViewLoadingListItemCreator loadingListItemCreator)
        {
            this.absListView = absListView;
            this.callbacks = callbacks;
            // Attach scrolling listener in order to perform end offset check on each scroll event
            scrollListener = new EndScrollListener(this);
            scrollListener.setThreshold(loadingTriggerThreshold);
            scrollListener.setDelegate(onScrollListener);
            absListView.SetOnScrollListener(scrollListener);

            if (addLoadingListItem)
            {
                BaseAdapter adapter;
                if (absListView.Adapter is BaseAdapter) {
                    adapter = (BaseAdapter)absListView.Adapter;
                } else if (absListView.Adapter is HeaderViewListAdapter) {
                    adapter = (BaseAdapter)((HeaderViewListAdapter)absListView.Adapter).WrappedAdapter;
                } else {
                    throw new IllegalStateException("Adapter needs to be subclass of BaseAdapter");
                }

                // Wrap existing adapter with new adapter that will add loading row
                wrapperAdapter = new AbsWrapperAdapter(adapter, loadingListItemCreator);
                dataSetObserver = new AbsPaginateDataSetObserver(wrapperAdapter, callbacks);
                adapter.RegisterDataSetObserver(dataSetObserver);
                
                absListView.Adapter = wrapperAdapter;
            }
        }


        public override void SetHasMoreDataToLoad(bool hasMoreDataToLoad)
        {
            if (wrapperAdapter != null)
            {
                wrapperAdapter.SetDisplayLoadingRow(hasMoreDataToLoad);
            }
        }

        public void onEndReached()
        {
            if (!callbacks.isLoading() && !callbacks.hasLoadedAllItems())
            {
                callbacks.onLoadMore();
            }
        }
        public override void Unbind()
        {
            // Swap back original scroll listener
            absListView.SetOnScrollListener(scrollListener.getDelegateScrollListener());

            // Swap back source adapter
            if (absListView.Adapter is AbsWrapperAdapter) {
                var wrapperAdapter = (AbsWrapperAdapter)absListView.Adapter;
                BaseAdapter adapter = (BaseAdapter)wrapperAdapter.WrappedAdapter;
                adapter.UnregisterDataSetObserver(dataSetObserver);
                absListView.Adapter = adapter;
            }
        }

        public class AbsPaginateDataSetObserver : DataSetObserver
        {
            private Callbacks callbacks;
            private AbsWrapperAdapter wrapperAdapter;

            public AbsPaginateDataSetObserver(AbsWrapperAdapter wrapperAdapter, Callbacks callbacks)
            {
                this.wrapperAdapter = wrapperAdapter;
                this.callbacks = callbacks;
            }

            protected AbsPaginateDataSetObserver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public override void OnChanged()
            {
                wrapperAdapter.SetDisplayLoadingRow(!callbacks.hasLoadedAllItems());
                wrapperAdapter.NotifyDataSetChanged();
            }


            public override void OnInvalidated()
            {
                wrapperAdapter.SetDisplayLoadingRow(!callbacks.hasLoadedAllItems());
                wrapperAdapter.NotifyDataSetInvalidated();
            }
        }
    }
}