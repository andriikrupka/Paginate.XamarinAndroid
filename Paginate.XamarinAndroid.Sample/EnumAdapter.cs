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

namespace Paginate.XamarinAndroid.Sample
{
    public class EnumAdapter<T> : BindableAdapter<T> where T : struct
    {
        private T[] enumConstants;

        public override int Count
        {
            get
            {
                return enumConstants.Length;
            }
        }

        public override T this[int position]
        {
            get
            {
                return enumConstants[position];
            }
        }

        public EnumAdapter(Context context)
            : base(context)
        {
            enumConstants = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        public override View newView(LayoutInflater inflater, int position, ViewGroup container)
        {
            return inflater.Inflate(Android.Resource.Layout.SimpleSpinnerItem, container, false);
        }

        public override void bindView(T item, int position, View view)
        {
            TextView tv = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            tv.Text = item.ToString();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View newDropDownView(LayoutInflater inflater, int position, ViewGroup container)
        {
            return inflater.Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, container, false);
        }

    }
}