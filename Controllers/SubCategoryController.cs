using AspNetCoreGeneratedDocument;
using MegaStor.AddImge;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SubCategoryController : Controller
	{
		private readonly IRepository<SubCategory> _repoSubCategory;
 		private readonly CategoryRepository _ctegory;
		private readonly IRepository<Product> repoProuduct;
		private readonly AddAndDeleteImageInServer _addImge;

		public SubCategoryController(IRepository<SubCategory> RepoSubCategory,   
			CategoryRepository ctegory, IRepository<Product> RepoProuduct, AddAndDeleteImageInServer AddImge)
		{
			_repoSubCategory = RepoSubCategory;
 			_ctegory = ctegory;
			repoProuduct = RepoProuduct;
			_addImge = AddImge;
		}

		public async Task<IActionResult> Index()
		{
			IEnumerable<SubCategory> list = await _repoSubCategory.GetAllWithIncludeAsync(o => o.Products,i=>i.Category);
			return View(list);
		}
		 


		[HttpGet]
		public IActionResult Create(int id)
		{
			var model = new AddSubCategoryViewModelByName
			{
				CategoryId = id
			};
			return View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AddSubCategoryViewModelByName Care)
		{ 
			
 			if (ModelState.IsValid)
			{
				SubCategory add = new SubCategory();
				add.CategoryName = Care.Name;
				add.CategoryId = Care.CategoryId;
				add.ImgUrl = await _addImge.SaveImageAsync(Care.clientFile); 
				await _repoSubCategory.AddAsync(add);
				await _repoSubCategory.SaveAsync();
				return RedirectToAction(nameof(Index), "Category");
			}
			return View(Care);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(String Id)
		{
			var foundImg = await _ctegory.GetByIdWithProductsforSubAsync(Id);

			if(foundImg==null)
				return NotFound();

			var result = await _ctegory.DeleteSubAsync(Id);
			if (result)
			{			
				var boole = _addImge.DeleteImage(foundImg.ImgUrl); 

				return Ok();
			}
			return BadRequest();

		}


		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Details(string id)
		{
			var subcategory = await _ctegory.GetByIdWithProductsforSubAsync(id);

			var nameVinder = await repoProuduct.GetAllWithIncludeAsync(o => o.Vendor,i=>i.Category);//because assine NameVinder for Category 


			if (subcategory == null)
				return NotFound();
			 

			return View(subcategory);
		}


		[HttpGet]
		public async Task<IActionResult> Edit(string Id)
		{
			SubCategory Ed = await _ctegory.GetByIdWithProductsforSubAsync(Id);
			if (Ed == null)
			{
				return NotFound();
			}
			DetailsCategoryViewModel NewEdit = new DetailsCategoryViewModel
			{
				Name = Ed.CategoryName,
				ImgUrl = Ed.ImgUrl,
				SubCategoryId = Id,
			};
		
			return View(NewEdit);
		}
 	
           
	
 		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(DetailsCategoryViewModel NewSub)
		{
			
			if (ModelState.IsValid)
			{
				var oldSub = await _ctegory.GetByIdWithProductsforSubAsync(NewSub.SubCategoryId);

				if (oldSub == null)
					return NotFound();
				 
				oldSub.CategoryName = NewSub.Name;

				if (NewSub.formFile != null)
				{
					oldSub.ImgUrl = await _addImge.SaveImageAsync(NewSub.formFile);
				}
				else
				{
					oldSub.ImgUrl = NewSub.ImgUrl;
				}

				_repoSubCategory.Update(oldSub);  
				await _repoSubCategory.SaveAsync();  

				return RedirectToAction("Dashboard", "Admin"); 
			}
			return View(NewSub);
		}





	}
}
