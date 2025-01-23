using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Repository.Components
{
	public class CombosViewComponent :ViewComponent
	{
		private readonly DataContext _dataContext;
		public CombosViewComponent(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Combos.ToListAsync());
	}
}
