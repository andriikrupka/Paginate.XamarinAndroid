using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;

namespace Paginate.XamarinAndroid.Sample
{
    [Activity(Label = "Paginate.XamarinAndroid.Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.activity_main);

            var recyclerViewButton = FindViewById<Button>(Resource.Id.recycler_view_button);
            recyclerViewButton.Click += OnRecyclerViewButtonClick;
        }

        private void OnRecyclerViewButtonClick(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RecyclerViewExampleActivity)));
        }
    }
}

