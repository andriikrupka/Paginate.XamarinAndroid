using Android.Views;
using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public class WrapperAdapter : RecyclerView.Adapter
    {
        private const int ITEM_VIEW_TYPE_LOADING = int.MaxValue - 50; // Magic

        private RecyclerView.Adapter wrappedAdapter;
        private RecyclerLoadingListItemCreator loadingListItemCreator;
        private bool _displayLoadingRow = true;

        public WrapperAdapter(RecyclerView.Adapter adapter, RecyclerLoadingListItemCreator creator)
        {
            this.wrappedAdapter = adapter;
            this.loadingListItemCreator = creator;
        }

        public override int ItemCount
            => _displayLoadingRow ? wrappedAdapter.ItemCount + 1 : wrappedAdapter.ItemCount;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (isLoadingRow(position))
            {
                loadingListItemCreator.onBindViewHolder(holder, position);
            }
            else
            {
                wrappedAdapter.OnBindViewHolder(holder, position);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == ITEM_VIEW_TYPE_LOADING)
            {
                return loadingListItemCreator.onCreateViewHolder(parent, viewType);
            }
            else
            {
                return wrappedAdapter.OnCreateViewHolder(parent, viewType);
            }
        }

        public override int GetItemViewType(int position)
        {
            return isLoadingRow(position) ? ITEM_VIEW_TYPE_LOADING : wrappedAdapter.GetItemViewType(position);
        }

        public override long GetItemId(int position)
        {
            return isLoadingRow(position) ? RecyclerView.NoId : wrappedAdapter.GetItemId(position);
        }

        public new bool HasStableIds
        {
            get
            {
                return base.HasStableIds;
            }
            set
            {
                base.HasStableIds = value;
                wrappedAdapter.HasStableIds = value;
            }
        }

        public RecyclerView.Adapter getWrappedAdapter()
        {
            return wrappedAdapter;
        }

        bool isDisplayLoadingRow()
        {
            return _displayLoadingRow;
        }

        public void displayLoadingRow(bool displayLoadingRow)
        {
            if (this._displayLoadingRow != displayLoadingRow)
            {
                this._displayLoadingRow = displayLoadingRow;
                NotifyDataSetChanged();
            }
        }

        public bool isLoadingRow(int position)
        {
            return _displayLoadingRow && position == getLoadingRowPosition();
        }

        private int getLoadingRowPosition()
        {
            return _displayLoadingRow ? ItemCount - 1 : -1;
        }
    }
}