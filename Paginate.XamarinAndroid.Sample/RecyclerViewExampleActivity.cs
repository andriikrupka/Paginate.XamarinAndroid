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
using Paginate.XamarinAndroid.Sample.Provider;
using Paginate.Droid;

namespace Paginate.XamarinAndroid.Sample
{
    [Activity]
    public class RecyclerViewExampleActivity : BaseActivity, Droid.Paginate.Callbacks
    {
        public const int GRID_SPAN = 3;

        private RecyclerView recyclerView;
        private RecyclerPersonAdapter adapter;
        private bool loading = false;
        private int page = 0;
        private Handler handler;

        private Droid.Paginate paginate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            LayoutInflater.From(this).Inflate(Resource.Layout.recycler_layout, GetContainer(), true);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            handler = new Handler();
            setupPagination();
        }

        public bool hasLoadedAllItems()
        {
            return page == totalPages;
        }

        public bool isLoading()
        {
            return loading;
        }

        public void onLoadMore()
        {
            loading = true;
            // Fake asynchronous loading that will generate page of random data after some delay
            handler.PostDelayed(fakeCallback, networkDelay);
        }

        private void fakeCallback()
        {
            page++;
            adapter.add(DataProvider.getRandomData(itemsPerPage));
            loading = false;
        }

        protected override void setupPagination()
        {
            if (paginate != null)
            {
                paginate.unbind();
            }

            handler.RemoveCallbacks(fakeCallback);
            adapter = new RecyclerPersonAdapter(DataProvider.getRandomData(20));
            loading = false;
            page = 0;

            int layoutOrientation = 0;
            switch (orientation)
            {
                case Orientation.VERTICAL:
                    layoutOrientation = OrientationHelper.Vertical;
                    break;
                case Orientation.HORIZONTAL:
                    layoutOrientation = OrientationHelper.Horizontal;
                    break;
            }


            RecyclerView.LayoutManager layoutManager = null;
            switch (layoutManagerEnum)
            {
                case LayoutManagerEnum.LINEAR:
                    layoutManager = new LinearLayoutManager(this, layoutOrientation, false);
                    break;
                case LayoutManagerEnum.GRID:
                    layoutManager = new GridLayoutManager(this, GRID_SPAN, layoutOrientation, false);
                    break;
                case LayoutManagerEnum.STAGGERED:
                    layoutManager = new StaggeredGridLayoutManager(GRID_SPAN, layoutOrientation);
                    break;
            }

            if (layoutManager is LinearLayoutManager)
            {
                ((LinearLayoutManager)layoutManager).ReverseLayout = reverseLayout;
            }
            else
            {
                ((StaggeredGridLayoutManager)layoutManager).ReverseLayout = reverseLayout;
            }

            recyclerView.SetLayoutManager(layoutManager);
            //recyclerView.SetItemAnimator(new SlideInUpAnimator());
            recyclerView.SetAdapter(adapter);

            paginate = Droid.Paginate.with(recyclerView, this)
                        .setLoadingTriggerThreshold(threshold)
                        .addLoadingListItem(addLoadingRow)
                        .setLoadingListItemSpanSizeLookup(new SampleLoadingListItemSpanLookup())
                        .build();
        }


    }

    public class SampleLoadingListItemSpanLookup : LoadingListItemSpanLookup
    {
        public override int getSpanSize()
        {
            return RecyclerViewExampleActivity.GRID_SPAN;
        }
    }
}