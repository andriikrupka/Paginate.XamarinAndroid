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
using Android.Support.V7.App;
using Android.Content.Res;
using Android.Support.V4.Widget;

namespace Paginate.XamarinAndroid.Sample
{
    public enum LayoutManagerEnum
    {
        LINEAR,
        GRID,
        STAGGERED
    }

    public enum Orientation
    {
        VERTICAL,
        HORIZONTAL
    }

    public enum AbsListViewType
    {
        LIST_VIEW,
        GRID_VIEW
    }

    public abstract class BaseActivity : AppCompatActivity
    {
        protected int threshold = 4;
        protected int totalPages = 3;
        protected int itemsPerPage = 10;
        protected long networkDelay = 2000;
        protected bool addLoadingRow = true;
        protected bool customLoadingListItem = false;

        protected LayoutManagerEnum layoutManagerEnum = LayoutManagerEnum.LINEAR;
        protected Orientation orientation = Orientation.VERTICAL;
        protected bool reverseLayout = false;

        // AbsListView specific options
        protected AbsListViewType absListViewType = AbsListViewType.LIST_VIEW;
        protected bool useHeaderAndFooter = false;

        private ActionBarDrawerToggle drawerToggle;
        private FrameLayout container;
        private IntegerAdapter thresholdAdapter;
        private IntegerAdapter pagesAdapter;
        private IntegerAdapter itemsPerPageAdapter;
        private IntegerAdapter delayAdapter;
        private EnumAdapter<LayoutManagerEnum> layoutManagerAdapter;
        private EnumAdapter<Orientation> orientationAdapter;
        private EnumAdapter<AbsListViewType> absListViewTypeAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.base_layout_container);
            container = FindViewById<FrameLayout>(Resource.Id.container);

            SetupBasicUI();
            SetupOptions();
        }

        protected ViewGroup GetContainer() => container;

        protected abstract void setupPagination();

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            drawerToggle.OnConfigurationChanged(newConfig);
        }

        private void SetupBasicUI()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetHomeButtonEnabled(true);
            actionBar.SetDisplayShowTitleEnabled(false);

            DrawerLayout drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_closed);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        private void SetupOptions()
        {
            Spinner thresholdView = FindViewById<Spinner>(Resource.Id.spinner_threshold);
            thresholdAdapter = new IntegerAdapter(this, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            thresholdView.Adapter = thresholdAdapter;
            thresholdView.SetSelection(thresholdAdapter.getPositionForValue(threshold));
            thresholdView.ItemSelected += ThresholdView_ItemSelected;

            Spinner pagesView = FindViewById<Spinner>(Resource.Id.spinner_pages);
            pagesAdapter = new IntegerAdapter(this, new int[] { 1, 2, 3, 4, 5, 6, 7 });
            pagesView.Adapter = pagesAdapter;
            pagesView.SetSelection(pagesAdapter.getPositionForValue(totalPages));
            pagesView.ItemSelected += PagesView_ItemSelected;

            Spinner itemsPerPageView = FindViewById<Spinner>(Resource.Id.spinner_items_per_page);
            itemsPerPageAdapter = new IntegerAdapter(this, new int[] { 2, 5, 10, 20, 30 });
            itemsPerPageView.Adapter = itemsPerPageAdapter;
            itemsPerPageView.SetSelection(itemsPerPageAdapter.getPositionForValue(itemsPerPage));
            itemsPerPageView.ItemSelected += ItemsPerPageView_ItemSelected;

            Spinner networkDelayView = FindViewById<Spinner>(Resource.Id.spinner_delay);
            delayAdapter = new IntegerAdapter(this, new int[] { 1000, 2000, 3000, 5000 });
            networkDelayView.Adapter = delayAdapter;
            networkDelayView.SetSelection(delayAdapter.getPositionForValue(networkDelay));
            networkDelayView.ItemSelected += NetworkDelayView_ItemSelected;

            CheckBox addLoadingRowCb = FindViewById<CheckBox>(Resource.Id.cb_add_loading_row);
            addLoadingRowCb.Checked = addLoadingRow;
            addLoadingRowCb.CheckedChange += AddLoadingRowCb_CheckedChange;

            CheckBox customLoadingListItemCb = FindViewById<CheckBox>(Resource.Id.cb_custom_row);
            customLoadingListItemCb.Checked = customLoadingListItem;
            customLoadingListItemCb.CheckedChange += CustomLoadingListItemCb_CheckedChange;

            Spinner layoutManagerView = FindViewById<Spinner>(Resource.Id.spinner_layout_mng);
            layoutManagerAdapter = new EnumAdapter<LayoutManagerEnum>(this);
            layoutManagerView.Adapter = layoutManagerAdapter;
            layoutManagerView.ItemSelected += LayoutManagerView_ItemSelected;

            Spinner orientationView = FindViewById<Spinner>(Resource.Id.spinner_orientation);
            orientationAdapter = new EnumAdapter<Orientation>(this);
            orientationView.Adapter = orientationAdapter;
            orientationView.ItemSelected += OrientationView_ItemSelected;

            CheckBox reverseLayoutCb = FindViewById<CheckBox>(Resource.Id.cb_reverse);
            reverseLayoutCb.Checked = reverseLayout;
            reverseLayoutCb.CheckedChange += ReverseLayoutCb_CheckedChange;

            Spinner absListViewTypeView = FindViewById<Spinner>(Resource.Id.spinner_abs_list_type);
            absListViewTypeAdapter = new EnumAdapter<AbsListViewType>(this);
            absListViewTypeView.Adapter = absListViewTypeAdapter;
            absListViewTypeView.ItemSelected += AbsListViewTypeView_ItemSelected;

            CheckBox userHeaderAndFooterCb = FindViewById<CheckBox>(Resource.Id.cb_header_and_footer);
            userHeaderAndFooterCb.Checked = useHeaderAndFooter;
            userHeaderAndFooterCb.CheckedChange += UserHeaderAndFooterCb_CheckedChange;
        }

        private void UserHeaderAndFooterCb_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (useHeaderAndFooter != e.IsChecked)
            {
                useHeaderAndFooter = e.IsChecked;
                setupPagination();
            }
        }

        private void AbsListViewTypeView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            AbsListViewType selected = absListViewTypeAdapter[e.Position];
            if (selected != absListViewType)
            {
                absListViewType = selected;
                setupPagination();
            }
        }

        private void ReverseLayoutCb_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (reverseLayout != e.IsChecked)
            {
                reverseLayout = e.IsChecked;
                setupPagination();
            }
        }

        private void OrientationView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Orientation selected = orientationAdapter[e.Position];
            if (selected != orientation)
            {
                orientation = selected;
                setupPagination();
            }
        }

        private void LayoutManagerView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            LayoutManagerEnum selected = layoutManagerAdapter[e.Position];
            if (selected != layoutManagerEnum)
            {
                layoutManagerEnum = selected;
                setupPagination();
            }
        }

        private void CustomLoadingListItemCb_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (customLoadingListItem != e.IsChecked)
            {
                customLoadingListItem = e.IsChecked;
                setupPagination();
            }
        }

        private void AddLoadingRowCb_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (addLoadingRow != e.IsChecked)
            {
                addLoadingRow = e.IsChecked;
                setupPagination();
            }
        }

        private void NetworkDelayView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            networkDelay = delayAdapter[e.Position];
        }

        private void ItemsPerPageView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            itemsPerPage = itemsPerPageAdapter[e.Position];
        }

        private void PagesView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int selected = pagesAdapter[e.Position];
            if (selected != totalPages)
            {
                totalPages = selected;
                setupPagination();
            }
        }

        private void ThresholdView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var item = thresholdAdapter[e.Position];
            if (item != threshold)
            {
                threshold = item;
                setupPagination();
            }
        }
    }
}