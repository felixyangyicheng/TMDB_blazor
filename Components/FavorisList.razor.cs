using Microsoft.AspNetCore.Components;
using System.Text.Json;
using TMDB_blazor.Data;

namespace TMDB_blazor.Components
{
	public partial class FavorisList
	{
		[Parameter] public UserMovie LikedItem { get; set; }

	}
}
