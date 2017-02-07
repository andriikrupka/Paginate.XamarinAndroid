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
using Java.Lang;

namespace Paginate.Droid
{
    public class RecyclerPaginate : Paginate
    {
        private RecyclerView recyclerView;
        private Callbacks callbacks;
        private int loadingTriggerThreshold;
        private WrapperAdapter wrapperAdapter;
        private WrapperSpanSizeLookup wrapperSpanSizeLookup;
        private RecyclerView.OnScrollListener mOnScrollListener;
        private RecyclerView.AdapterDataObserver mDataObserver;

        public RecyclerPaginate(RecyclerView recyclerView,
                 Paginate.Callbacks callbacks,
                 int loadingTriggerThreshold,
                 bool addLoadingListItem,
                 RecyclerLoadingListItemCreator loadingListItemCreator,
                 LoadingListItemSpanLookup loadingListItemSpanLookup)
        {
            this.recyclerView = recyclerView;
            this.callbacks = callbacks;
            this.loadingTriggerThreshold = loadingTriggerThreshold;
            this.mOnScrollListener = new PaginateOnScrollListener(checkEndOffset);

            // Attach scrolling listener in order to perform end offset check on each scroll event
            recyclerView.AddOnScrollListener(mOnScrollListener);

            if (addLoadingListItem)
            {
                // Wrap existing adapter with new adapter that will add loading row
                RecyclerView.Adapter adapter = recyclerView.GetAdapter();
                wrapperAdapter = new WrapperAdapter(adapter, loadingListItemCreator);
                mDataObserver = new PaginateAdapterDataObserver(onAdapterDataChanged, wrapperAdapter);
                adapter.RegisterAdapterDataObserver(mDataObserver);
                recyclerView.SetAdapter(wrapperAdapter);

                // For GridLayoutManager use separate/customisable span lookup for loading row
                if (recyclerView.GetLayoutManager() is GridLayoutManager)
                {
                    wrapperSpanSizeLookup = new WrapperSpanSizeLookup(
                            ((GridLayoutManager)recyclerView.GetLayoutManager()).GetSpanSizeLookup(),
                            loadingListItemSpanLookup,
                            wrapperAdapter);
                    ((GridLayoutManager)recyclerView.GetLayoutManager()).SetSpanSizeLookup(wrapperSpanSizeLookup);
                }
            }

            // Trigger initial check since adapter might not have any items initially so no scrolling events upon
            // RecyclerView (that triggers check) will occur
            checkEndOffset();
        }

        public override void setHasMoreDataToLoad(bool hasMoreDataToLoad)
        {
            if (wrapperAdapter != null)
            {
                wrapperAdapter.displayLoadingRow(hasMoreDataToLoad);
            }
        }

        public override void unbind()
        {
            recyclerView.RemoveOnScrollListener(mOnScrollListener);   // Remove scroll listener
            if (recyclerView.GetAdapter() is WrapperAdapter)
            {
                WrapperAdapter wrapperAdapter = (WrapperAdapter)recyclerView.GetAdapter();
                RecyclerView.Adapter adapter = wrapperAdapter.getWrappedAdapter();
                adapter.UnregisterAdapterDataObserver(mDataObserver); // Remove data observer
                recyclerView.SetAdapter(adapter);                     // Swap back original adapter
            }
            if (recyclerView.GetLayoutManager() is GridLayoutManager && wrapperSpanSizeLookup != null)
            {
                // Swap back original SpanSizeLookup
                GridLayoutManager.SpanSizeLookup spanSizeLookup = wrapperSpanSizeLookup.getWrappedSpanSizeLookup();
                ((GridLayoutManager)recyclerView.GetLayoutManager()).SetSpanSizeLookup(spanSizeLookup);
            }
        }

        void checkEndOffset()
        {
            int visibleItemCount = recyclerView.ChildCount;
            int totalItemCount = recyclerView.GetLayoutManager().ItemCount;

            int firstVisibleItemPosition;
            if (recyclerView.GetLayoutManager() is LinearLayoutManager)
            {
                firstVisibleItemPosition = ((LinearLayoutManager)recyclerView.GetLayoutManager()).FindFirstVisibleItemPosition();
            }
            else if (recyclerView.GetLayoutManager() is StaggeredGridLayoutManager)
            {
                // https://code.google.com/p/android/issues/detail?id=181461
                if (recyclerView.GetLayoutManager().ChildCount > 0)
                {
                    firstVisibleItemPosition = ((StaggeredGridLayoutManager)recyclerView.GetLayoutManager()).FindFirstVisibleItemPositions(null)[0];
                }
                else
                {
                    firstVisibleItemPosition = 0;
                }
            }
            else
            {
                throw new ArgumentException("LayoutManager needs to subclass LinearLayoutManager or StaggeredGridLayoutManager");
            }

            // Check if end of the list is reached (counting threshold) or if there is no items at all
            if ((totalItemCount - visibleItemCount) <= (firstVisibleItemPosition + loadingTriggerThreshold)
                    || totalItemCount == 0)
            {
                // Call load more only if loading is not currently in progress and if there is more items to load
                if (!callbacks.isLoading() && !callbacks.hasLoadedAllItems())
                {
                    callbacks.onLoadMore();
                }
            }
        }

        private void onAdapterDataChanged()
        {
            wrapperAdapter.displayLoadingRow(!callbacks.hasLoadedAllItems());
            checkEndOffset();
        }

        private class PaginateOnScrollListener : RecyclerView.OnScrollListener
        {
            private Action _scrolledCallback;

            public PaginateOnScrollListener(Action scrolledCallback)
            {
                _scrolledCallback = scrolledCallback;
            }

            protected PaginateOnScrollListener(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {

            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                _scrolledCallback();

                //TODO:
                //base.OnScrolled(recyclerView, dx, dy);
            }
        }

        private class PaginateAdapterDataObserver : RecyclerView.AdapterDataObserver
        {
            private Action _onAdapterDataChangedCallback;
            private WrapperAdapter _wrapperAdapter;

            public PaginateAdapterDataObserver(Action onAdapterDataChangedCallback,
                WrapperAdapter wrapperAdapter)
            {
                _onAdapterDataChangedCallback = onAdapterDataChangedCallback;
                _wrapperAdapter = wrapperAdapter;
            }

            protected PaginateAdapterDataObserver(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {

            }

            public override void OnChanged()
            {
                _wrapperAdapter.NotifyDataSetChanged();
                _onAdapterDataChangedCallback();

                //TODO: base
            }

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                _wrapperAdapter.NotifyItemRangeInserted(positionStart, itemCount);
                _onAdapterDataChangedCallback();

                //TODO: base
                //base.OnItemRangeInserted(positionStart, itemCount);
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount)
            {
                _wrapperAdapter.NotifyItemRangeChanged(positionStart, itemCount);
                _onAdapterDataChangedCallback();

                //TODO
                //base.OnItemRangeChanged(positionStart, itemCount);
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount, Java.Lang.Object payload)
            {
                _wrapperAdapter.NotifyItemRangeChanged(positionStart, itemCount, payload);
                _onAdapterDataChangedCallback();

                //TODO
                //base.OnItemRangeChanged(positionStart, itemCount, payload);
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                _wrapperAdapter.NotifyItemRangeRemoved(positionStart, itemCount);
                _onAdapterDataChangedCallback();

                //TODO
                //base.OnItemRangeRemoved(positionStart, itemCount);
            }

            public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount)
            {
                _wrapperAdapter.NotifyItemMoved(fromPosition, toPosition);
                _onAdapterDataChangedCallback();

                //TODO
                //base.OnItemRangeMoved(fromPosition, toPosition, itemCount);
            }
        }

        public static PaginateBuilder Builder { get; set; }

        public class PaginateBuilder
        {
            private RecyclerView recyclerView;
            private Paginate.Callbacks callbacks;

            private int loadingTriggerThreshold = 5;
            private bool _addLoadingListItem = true;
            private RecyclerLoadingListItemCreator loadingListItemCreator;
            private LoadingListItemSpanLookup loadingListItemSpanLookup;

            public PaginateBuilder(RecyclerView recyclerView, Paginate.Callbacks callbacks)
            {
                this.recyclerView = recyclerView;
                this.callbacks = callbacks;
            }

            public PaginateBuilder setLoadingTriggerThreshold(int threshold)
            {
                this.loadingTriggerThreshold = threshold;
                return this;
            }

            public PaginateBuilder addLoadingListItem(bool addLoadingListItem)
            {
                _addLoadingListItem = addLoadingListItem;
                return this;
            }

            public PaginateBuilder setLoadingListItemCreator(RecyclerLoadingListItemCreator creator)
            {
                this.loadingListItemCreator = creator;
                return this;
            }

            public PaginateBuilder setLoadingListItemSpanSizeLookup(LoadingListItemSpanLookup loadingListItemSpanLookup)
            {
                this.loadingListItemSpanLookup = loadingListItemSpanLookup;
                return this;
            }

            public Paginate build()
            {
                if (recyclerView.GetAdapter() == null)
                {
                    throw new IllegalStateException("Adapter needs to be set!");
                }
                if (recyclerView.GetLayoutManager() == null)
                {
                    throw new IllegalStateException("LayoutManager needs to be set on the RecyclerView");
                }

                if (loadingListItemCreator == null)
                {
                    loadingListItemCreator = RecyclerLoadingListItemCreator.Default;
                }

                if (loadingListItemSpanLookup == null)
                {
                    loadingListItemSpanLookup = new DefaultLoadingListItemSpanLookup(recyclerView.GetLayoutManager());
                }

                return new RecyclerPaginate(recyclerView, callbacks, loadingTriggerThreshold, _addLoadingListItem,
                        loadingListItemCreator, loadingListItemSpanLookup);
            }
        }
    }
}