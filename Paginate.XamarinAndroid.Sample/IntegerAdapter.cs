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
    public class IntegerAdapter : BindableAdapter<int>
    {
        private int[] VALUES;

        public IntegerAdapter(Context context, int[] values)
            : base(context)
        {
            this.VALUES = values;
        }

        public int getPositionForValue(long value)
        {
            for (int i = 0; i < VALUES.Length; i++)
            {
                if (VALUES[i] == value)
                {
                    return i;
                }
            }
            return 3; // Default to 2000 if something changes.
        }

        public override View newView(LayoutInflater inflater, int position, ViewGroup container)
        {
            return inflater.Inflate(Android.Resource.Layout.SimpleSpinnerItem, container, false);
        }

        public override void bindView(int item, int position, View view)
        {
            TextView tv = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            tv.Text = item.ToString();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get
            {
                return VALUES.Length;
            }
        }

        public override int this[int position]
        {
            get
            {
                return VALUES[position];
            }
        }

        public override View newDropDownView(LayoutInflater inflater, int position, ViewGroup container)
        {
            return inflater.Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, container, false);
        }
    }
}