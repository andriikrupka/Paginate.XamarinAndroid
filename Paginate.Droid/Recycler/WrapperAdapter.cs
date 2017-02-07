using Android.Views;
using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public class WrapperAdapter : RecyclerView.Adapter
    {
        private const int ITEM_VIEW_TYPE_LOADING = int.MaxValue - 50; // Magic

        private RecyclerView.Adapter wrappedAdapter;
        private RecyclerLoadingListItemCreator loadingListItemCreator;
        private bool displayLoadingRow = true;

        public WrapperAdapter(RecyclerView.Adapter adapter, RecyclerLoadingListItemCreator creator)
        {
            this.wrappedAdapter = adapter;
            this.loadingListItemCreator = creator;
        }

        public override int ItemCount
            => displayLoadingRow ? wrappedAdapter.ItemCount + 1 : wrappedAdapter.ItemCount;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (IsLoadingRow(position))
            {
                loadingListItemCreator.OnBindViewHolder(holder, position);
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
                return loadingListItemCreator.OnCreateViewHolder(parent, viewType);
            }
            else
            {
                return wrappedAdapter.OnCreateViewHolder(parent, viewType);
            }
        }

        public override int GetItemViewType(int position)
        {
            return IsLoadingRow(position) ? ITEM_VIEW_TYPE_LOADING : wrappedAdapter.GetItemViewType(position);
        }

        public override long GetItemId(int position)
        {
            return IsLoadingRow(position) ? RecyclerView.NoId : wrappedAdapter.GetItemId(position);
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

        public RecyclerView.Adapter GetWrappedAdapter()
        {
            return wrappedAdapter;
        }

        public bool DisplayLoadingRow
        {
            get { return displayLoadingRow; }
            set
            {
                if (displayLoadingRow != value)
                {
                    displayLoadingRow = value;
                    NotifyDataSetChanged();
                }
            }
        }

        public bool IsLoadingRow(int position)
        {
            return displayLoadingRow && position == GetLoadingRowPosition();
        }

        private int GetLoadingRowPosition()
        {
            return displayLoadingRow ? ItemCount - 1 : -1;
        }
    }
}