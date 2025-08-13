using MegaStor.AddImge;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
namespace MegaStor.Controllers
{
	[Authorize(Roles = "Admin")]
	public class CategoryController : Controller
    {
        private readonly IRepository<Category> db;
        private readonly AddAndDeleteImageInServer addimgmethod;
		private readonly CategoryRepository _ctegory;
		private readonly IRepository<Product> repoProuduct;

		public CategoryController(IRepository<Category> _Db,AddAndDeleteImageInServer add, CategoryRepository ctegory, IRepository<Product> RepoProuduct) 
        {
            db = _Db;
            addimgmethod = add;
		    _ctegory = ctegory;
			repoProuduct = RepoProuduct;
		}
 
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> list = await db.GetAllWithIncludeAsync(s=>s.SubCategories); 
            ViewBag.counter=list.Count();
            return View(list);
        }
         
        public async Task<IActionResult> AllCategory()
        {
            IEnumerable<Category> list = await db.GetAllAsync(); 
            ViewBag.counter=list.Count();
            return View( nameof(AllCategory), list);
        }


 		[HttpGet]
		public IActionResult Create()
		{
			return View();
		} 
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryViewModelByName Care) 
        { 
			if (ModelState.IsValid) 
            {		
                Category add = new Category();
                add.Name = Care.Name;
                add.ImgUrl =await addimgmethod.SaveImageAsync(Care.clientFile);
                await db.AddAsync(add);
                await db.SaveAsync();
                return RedirectToAction(nameof(Index),"Category");
            }
            return View(Care);
        }


		[HttpPost]
		public async Task<IActionResult> Delete(int Id)
		{
			var foundImg = await db.GetByIdAsync(Id);

			if (foundImg == null)
				return NotFound();

			var result = await db.DeleteAsync(Id);

			if (result)
			{			
				var boole = addimgmethod.DeleteImage(foundImg.ImgUrl);

				return Ok(); 
			}

			return BadRequest();  
		}




		[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _ctegory.GetByIdWithProductsAsync(id);
             
			if (category == null)
				return NotFound();
               
			return View(category);
		}
       
         




        [HttpGet]
        public async Task<IActionResult> Edit(int Id) 
        {
            Category Ed = await db.GetByIdAsync(Id); 
            if (Ed == null) 
            {
				return NotFound();
			}
            DetailsCategoryViewModel detelse = new DetailsCategoryViewModel
            {
                Name = Ed.Name,
                CategoryId=Id,
                SubCategoryId=string.Empty,
                ImgUrl=Ed.ImgUrl,
            }; 
            return View(detelse);
        } 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DetailsCategoryViewModel NewCategory)
		{

			if (ModelState.IsValid)
			{
				var oldSub = await  db.GetByIdAsync(NewCategory.CategoryId);

				if (oldSub == null)
					return NotFound();

				oldSub.Name = NewCategory.Name;
 
				if (NewCategory.formFile != null)
				{
					oldSub.ImgUrl = await addimgmethod.SaveImageAsync(NewCategory.formFile);
				}
				else
				{
					oldSub.ImgUrl = NewCategory.ImgUrl;
				}
				db.Update(oldSub);
				await db.SaveAsync();

				return RedirectToAction("Dashboard", "Admin");
			}
			return View(NewCategory);
		}



	}
}
