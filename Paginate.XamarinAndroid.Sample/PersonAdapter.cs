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
using Paginate.XamarinAndroid.Sample.Model;

namespace Paginate.XamarinAndroid.Sample
{
    public class RecyclerPersonAdapter : RecyclerView.Adapter
    {
        private List<Person> data;

        public RecyclerPersonAdapter(List<Person> data)
        {
            this.data = data;
        }

        public override int ItemCount
        {
            get
            {
                return data.Count;
            }
        }

        public void add(List<Person> data)
        {
            this.data.AddRange(data);
            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Person person = data[position];
            var personViewHolder = (PersonViewHolder)holder;
            personViewHolder.tvFullName.Text = string.Format("{0} {1}, {2}", person.FirstName, person.LastName, person.Age);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.person_list_item, parent, false);
            return new PersonViewHolder(view);
        }

        public class PersonViewHolder : RecyclerView.ViewHolder
        {
            public TextView tvFullName { get; set; }

            public PersonViewHolder(View view)
                : base(view)
            {
                this.tvFullName = view.FindViewById<TextView>(Resource.Id.tv_full_name);
            }
        }
    }
}