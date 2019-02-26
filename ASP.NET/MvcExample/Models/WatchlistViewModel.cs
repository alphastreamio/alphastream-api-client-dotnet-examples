using AlphaStream.ApiClient.Watchlists.Models;
using System.ComponentModel.DataAnnotations;

namespace MvcExample.Models
{
    public class WatchlistViewModel : Watchlist
    {
        [Display(Name = "New Watchlist Id")]
        public string NewWatchlistId { get; set; }
    }
}