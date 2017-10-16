using System.Linq;
using dona.Forms.Model;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace dona.Forms.Common
{
    public class CachedImageCell : ViewCell
    {
        private CachedImage _cachedImage;

        protected override void OnBindingContextChanged()
        {
            if (_cachedImage == null)
                _cachedImage = (CachedImage)View.LogicalChildren.FirstOrDefault(x => x is CachedImage);

            if (_cachedImage == null) return;

            _cachedImage.Source = null; // prevent showing old images occasionally

            var item = BindingContext as Institution;
            if (item == null)
                return;

            _cachedImage.Source = item.CoverSource;

            base.OnBindingContextChanged();
        }
    }
}
