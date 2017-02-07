using System;
using Android.Runtime;
using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public class RecyclerPaginate : Paginate
    {
        private RecyclerView recyclerView;
        private Paginate.Callbacks callbacks;
        private int loadingTriggerThreshold;
        private WrapperAdapter wrapperAdapter;
        private WrapperSpanSizeLookup wrapperSpanSizeLookup;
        private RecyclerView.OnScrollListener scrollListener;
        private RecyclerView.AdapterDataObserver dataObserver;

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
            this.scrollListener = new PaginateOnScrollListener(CheckEndOffset);

            // Attach scrolling listener in order to perform end offset check on each scroll event
            recyclerView.AddOnScrollListener(scrollListener);
            if (addLoadingListItem)
            {
                // Wrap existing adapter with new adapter that will add loading row
                var adapter = recyclerView.GetAdapter();
                wrapperAdapter = new WrapperAdapter(adapter, loadingListItemCreator);
                dataObserver = new PaginateAdapterDataObserver(wrapperAdapter, OnAdapterDataChanged);
                adapter.RegisterAdapterDataObserver(dataObserver);
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
            CheckEndOffset();
        }

        public override void SetHasMoreDataToLoad(bool hasMoreDataToLoad)
        {
            if (wrapperAdapter != null)
            {
                wrapperAdapter.DisplayLoadingRow = hasMoreDataToLoad;
            }
        }

        public override void Unbind()
        {
            recyclerView.RemoveOnScrollListener(scrollListener);   // Remove scroll listener
            if (recyclerView.GetAdapter() is WrapperAdapter)
            {
                WrapperAdapter wrapperAdapter = (WrapperAdapter)recyclerView.GetAdapter();
                RecyclerView.Adapter adapter = wrapperAdapter.GetWrappedAdapter();
                adapter.UnregisterAdapterDataObserver(dataObserver); // Remove data observer
                recyclerView.SetAdapter(adapter);                     // Swap back original adapter
            }
            if (wrapperSpanSizeLookup != null && recyclerView.GetLayoutManager() is GridLayoutManager)
            {
                // Swap back original SpanSizeLookup
                GridLayoutManager.SpanSizeLookup spanSizeLookup = wrapperSpanSizeLookup.getWrappedSpanSizeLookup();
                ((GridLayoutManager)recyclerView.GetLayoutManager()).SetSpanSizeLookup(spanSizeLookup);
            }
        }

        private void CheckEndOffset()
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

        private void OnAdapterDataChanged()
        {
            wrapperAdapter.DisplayLoadingRow = !callbacks.hasLoadedAllItems();
            CheckEndOffset();
        }

        private class PaginateOnScrollListener : RecyclerView.OnScrollListener
        {
            private Action scrolledCallback;

            public PaginateOnScrollListener(Action scrolledCallback)
            {
                this.scrolledCallback = scrolledCallback;
            }

            protected PaginateOnScrollListener(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                scrolledCallback();
            }
        }

        private class PaginateAdapterDataObserver : RecyclerView.AdapterDataObserver
        {
            private Action onAdapterDataChangedCallback;
            private WrapperAdapter wrapperAdapter;

            public PaginateAdapterDataObserver(WrapperAdapter wrapperAdapter, Action onAdapterDataChangedCallback)
            {
                this.onAdapterDataChangedCallback = onAdapterDataChangedCallback;
                this.wrapperAdapter = wrapperAdapter;
            }

            protected PaginateAdapterDataObserver(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {

            }

            public override void OnChanged()
            {
                wrapperAdapter.NotifyDataSetChanged();
                onAdapterDataChangedCallback();
            }

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                wrapperAdapter.NotifyItemRangeInserted(positionStart, itemCount);
                onAdapterDataChangedCallback();
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount)
            {
                wrapperAdapter.NotifyItemRangeChanged(positionStart, itemCount);
                onAdapterDataChangedCallback();
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount, Java.Lang.Object payload)
            {
                wrapperAdapter.NotifyItemRangeChanged(positionStart, itemCount, payload);
                onAdapterDataChangedCallback();
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                wrapperAdapter.NotifyItemRangeRemoved(positionStart, itemCount);
                onAdapterDataChangedCallback();
            }

            public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount)
            {
                wrapperAdapter.NotifyItemMoved(fromPosition, toPosition);
                onAdapterDataChangedCallback();
            }
        }

        public static PaginateBuilder Builder { get; set; }

        public class PaginateBuilder
        {
            private RecyclerView recyclerView;
            private Paginate.Callbacks callbacks;

            private int loadingTriggerThreshold = 5;
            private bool addLoadingListItem = true;
            private RecyclerLoadingListItemCreator loadingListItemCreator;
            private LoadingListItemSpanLookup loadingListItemSpanLookup;

            public PaginateBuilder(RecyclerView recyclerView, Paginate.Callbacks callbacks)
            {
                this.recyclerView = recyclerView;
                this.callbacks = callbacks;
            }

            public PaginateBuilder SetLoadingTriggerThreshold(int threshold)
            {
                this.loadingTriggerThreshold = threshold;
                return this;
            }

            public PaginateBuilder AddLoadingListItem(bool addLoadingListItem)
            {
                this.addLoadingListItem = addLoadingListItem;
                return this;
            }

            public PaginateBuilder SetLoadingListItemCreator(RecyclerLoadingListItemCreator creator)
            {
                this.loadingListItemCreator = creator;
                return this;
            }

            public PaginateBuilder SetLoadingListItemSpanSizeLookup(LoadingListItemSpanLookup loadingListItemSpanLookup)
            {
                this.loadingListItemSpanLookup = loadingListItemSpanLookup;
                return this;
            }

            public Paginate Build()
            {
                if (recyclerView.GetAdapter() == null)
                {
                    throw new ArgumentNullException("Adapter needs to be set!");
                }
                if (recyclerView.GetLayoutManager() == null)
                {
                    throw new ArgumentNullException("LayoutManager needs to be set on the RecyclerView");
                }

                if (loadingListItemCreator == null)
                {
                    loadingListItemCreator = RecyclerLoadingListItemCreator.Default;
                }

                if (loadingListItemSpanLookup == null)
                {
                    loadingListItemSpanLookup = new DefaultLoadingListItemSpanLookup(recyclerView.GetLayoutManager());
                }

                return new RecyclerPaginate(recyclerView, callbacks, loadingTriggerThreshold, addLoadingListItem,
                        loadingListItemCreator, loadingListItemSpanLookup);
            }
        }
    }
}