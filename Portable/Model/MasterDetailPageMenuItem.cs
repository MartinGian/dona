using System;
using Xamarin.Forms;

namespace dona.Forms.Model
{
    public class MasterDetailMenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Func<Page> NewPage { get; set; }
    }
}