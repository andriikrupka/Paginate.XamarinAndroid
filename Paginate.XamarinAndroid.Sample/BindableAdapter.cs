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

namespace Paginate.XamarinAndroid.Sample
{
    public abstract class BindableAdapter<T> : BaseAdapter<T>
    {
        private Context context;
        private LayoutInflater inflater;

        public BindableAdapter(Context context)
        {
            this.context = context;
            this.inflater = LayoutInflater.From(context);
        }

        public Context getContext()
        {
            return context;
        }

        public sealed override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = newView(inflater, position, parent);
                if (convertView == null)
                {
                    throw new IllegalStateException("newView result must not be null.");
                }
            }

            bindView(this[position], position, convertView);

            return convertView;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = newDropDownView(inflater, position, parent);
                if (convertView == null)
                {
                    throw new IllegalStateException("newDropDownView result must not be null.");
                }
            }

            bindDropDownView(this[position], position, convertView);

            return convertView;
        }

        public abstract View newView(LayoutInflater inflater, int position, ViewGroup container);
        public abstract void bindView(T item, int position, View view);

        public virtual View newDropDownView(LayoutInflater inflater, int position, ViewGroup container)
        {
            return newView(inflater, position, container);
        }

        /** Bind the data for the specified {@code position} to the drop-down view. */
        public void bindDropDownView(T item, int position, View view)
        {
            bindView(item, position, view);
        }
    }
}