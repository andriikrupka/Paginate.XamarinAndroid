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

namespace Paginate.Droid
{
    public class EndScrollListener : Object, AbsListView.IOnScrollListener
    {
        private Callback callback;
        private int visibleThreshold = 5;
        private AbsListView.IOnScrollListener scrollListenerCallback;

        public EndScrollListener(Callback callback)
        {
            this.callback = callback;
        }

        public IntPtr Handle { get; }

        public interface Callback
        {
            void onEndReached();
        }

        public void Dispose()
        {

        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if ((totalItemCount - visibleItemCount) <= (firstVisibleItem + visibleThreshold))
            {
                callback.onEndReached();
            }

            scrollListenerCallback?.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
            scrollListenerCallback?.OnScrollStateChanged(view, scrollState);
        }

        public void setThreshold(int threshold)
        {
            this.visibleThreshold = Math.Max(0, threshold);
        }

        public void setDelegate(AbsListView.IOnScrollListener scrollListenerCallback)
        {
            this.scrollListenerCallback = scrollListenerCallback;
        }

        public AbsListView.IOnScrollListener getDelegateScrollListener()
        {
            return scrollListenerCallback;
        }
    }
}