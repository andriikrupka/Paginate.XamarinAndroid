using System;
using Android.Views;
using Android.Support.V7.Widget;

namespace Paginate.Droid
{
    public abstract class RecyclerLoadingListItemCreator
    {
        private static Lazy<DefaultLoadingListItemCreator> lazyDefaultItemCreator = new Lazy<DefaultLoadingListItemCreator>();
        public abstract RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType);
        public abstract void onBindViewHolder(RecyclerView.ViewHolder holder, int position);

        public static RecyclerLoadingListItemCreator Default => lazyDefaultItemCreator.Value;

        private class DefaultLoadingListItemCreator : RecyclerLoadingListItemCreator
        {
            public override void onBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                //no binding for default loading row
            }

            public override RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.loading_row, parent, false);
                return new LoadingViewHolder(view);
            }
        }

        private class LoadingViewHolder : RecyclerView.ViewHolder
        {
            public LoadingViewHolder(View view)
                : base(view)
            {

            }
        }
    }
}