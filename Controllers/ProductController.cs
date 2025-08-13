using System.Security.Claims;
using MegaStor.AddImge;
 using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
 namespace MegaStor.Controllers
{
    [Authorize(Roles = "Admin,Seller")]
    public class ProductController : Controller
    {

		private readonly IRepository<Product> _productRepository;
		private readonly ProductRepository _repoProduct;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IRepository<EditForProductViewModel> editForProductViewModel;
        private readonly IRepository<Category> CategoryRepository;
        private readonly IRepository<Review> _repoReview;
        private readonly IRepository<SubCategory> _subCategoryRepository;
		private readonly AddAndDeleteImageInServer saveImage;
 
        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository,
            IRepository<EditForProductViewModel> _EditForProductViewModel, AddAndDeleteImageInServer SaveImage,
			IRepository<SubCategory> SubCategoryRepository, ProductRepository RepoProduct,UserManager<ApplicationUser> UserManager,
			IRepository<Review> RepoReview)
        {
            
            editForProductViewModel = _EditForProductViewModel;
            _productRepository = productRepository;
			CategoryRepository = categoryRepository;
			saveImage = SaveImage;
			_subCategoryRepository = SubCategoryRepository;
			_repoProduct = RepoProduct;
			_userManager = UserManager;
			_repoReview = RepoReview;
		}


		[Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllWithIncludeAsync(c => c.Category, v => v.Vendor, r => r.Reviews);
 
			return View(products);
        }




        [AllowAnonymous]
        public async Task<IActionResult> AllProduct(List<string>? categoryIds, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
			int pageSize = 12; 
			var results =  await _productRepository.GetAllWithIncludeAsync(o => o.Category);
			 
			// فلترة بالفئة 
			var CategoryIds = new List<string>();
			if (page == 1 || categoryIds.Count == 0)
			{
				if (categoryIds != null && categoryIds.Any())
					results = results.Where(p => categoryIds.Contains(p.SubCategoryId));
			}
			else
			{
				string OneString = categoryIds[0];
				CategoryIds = OneString.Split(',').ToList();
				if (CategoryIds != null && CategoryIds.Any())
					results = results.Where(p => CategoryIds.Contains(p.SubCategoryId));
			} 

			//فلترة بالسعر الأدنى
			if (minPrice.HasValue)
				results = results.Where(p => p.DiscountPrice >= minPrice.Value);

			// فلترة بالسعر الأقصى     
			if (maxPrice.HasValue)
				results = results.Where(p => p.DiscountPrice <= maxPrice.Value);

			var pagedProducts = results
				.OrderBy(p => p.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var Categories = _subCategoryRepository.GetAllAsQuery()
				.Select(c => new SubCategory
				{
					SubCategoryId = c.SubCategoryId,
					CategoryName = c.CategoryName,
					Products = c.Products // لو انت محتاجها للعداد
				}).ToList();

			AllProductAndFultterViewModel allProductAndFultterViewModel = new AllProductAndFultterViewModel
			{
				Products = pagedProducts,
				SubCategories = Categories,
 				maxPrice = maxPrice,
				minPrice = minPrice,
				page = page,
				categoryIds = categoryIds,
			};
			if (page != 1 && categoryIds.Count != 0)
			{
				allProductAndFultterViewModel.categoryIds = CategoryIds;
			}

			ViewBag.pagination = page;

			return View(allProductAndFultterViewModel); 
		}
		 


		[HttpGet]
        public async Task<IActionResult> Create()
        {          
 			IEnumerable<SubCategory> SubCategorylist = await _subCategoryRepository.GetAllAsync(); 
            ViewBag.SubCategores = new SelectList(SubCategorylist, "SubCategoryId", "CategoryName");

            return View();
        }   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel product)
        {   
            if (ModelState.IsValid)
            {
                var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier); // هذا هو الـ UserId

                var Categoryid = _subCategoryRepository.GetAllAsQuery().Include(c => c.Category)
                    .FirstOrDefault(c => c.SubCategoryId == product.SubcategoryId);

				Product productNew =new Product(); 
                productNew.CategoryId= Categoryid.CategoryId ?? 0;
                productNew.SubCategoryId=product.SubcategoryId;
                productNew.Name=product.ProductName;
                productNew.ImageUrl =await saveImage.SaveImageAsync(product.ClientFile);
                if (product.Images != null)
                {
                    List<ProductImage> productImages = new List<ProductImage>();
                    foreach (var i in product.Images)
                    {
                        ProductImage productImage = new ProductImage();
                        productImage.ImageURL = await saveImage.SaveImageAsync(i);
                        productImages.Add(productImage);
                    }
                    productNew.ProductImages = productImages;
                }
                else
                {
                    productNew.ProductImages = new List<ProductImage>();
                }

                productNew.CreatedAt = DateTime.Now;
                productNew.IsAvailable = true;

                var discountValue = product.discount ?? 0;
                 
				productNew.DiscountPrice = discountValue != 0
	            ? product.Price - (product.Price * (discountValue / 100))
	            : product.Price;

				productNew.discount = discountValue; 

				productNew.Quantity = product.Quantity;
                productNew.Price = product.Price;
                productNew.Description=product.ProductDescription;
                productNew.VendorId = customerId;   

                await _productRepository.AddAsync(productNew);
                await _productRepository.SaveAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("DashboardSeller","Seller");

            }
 			IEnumerable<SubCategory> SubCategorylist = await _subCategoryRepository.GetAllAsync();
 			ViewBag.SubCategores = new SelectList(SubCategorylist, "SubCategoryId", "CategoryName");

			return View(product);
        }
        



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product==null) 
            {
                return View(NotFound());
            }

            EditForProductViewModel editForProductViewModel = new EditForProductViewModel
            {
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price,
                discount = product.discount,
                DiscountPrice = product.DiscountPrice,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
				SubCategoryId = product.SubCategoryId,
             };

			IEnumerable<SubCategory> SubCategorylist = await _subCategoryRepository.GetAllAsync();
			IEnumerable<Category> listCategory = await CategoryRepository.GetAllAsync();
            ViewBag.categores = new SelectList(listCategory, "Id", "Name");
			ViewBag.SubCategores = new SelectList(SubCategorylist, "SubCategoryId", "CategoryName");

			return View(editForProductViewModel);
              
    }   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditForProductViewModel newproduct)
        {
            if (!ModelState.IsValid) 
            {

				IEnumerable<SubCategory> SubCategorylist = await _subCategoryRepository.GetAllAsync();
				IEnumerable<Category> listCategory = await CategoryRepository.GetAllAsync();
				ViewBag.categores = new SelectList(listCategory, "Id", "Name");
				ViewBag.SubCategores = new SelectList(SubCategorylist, "SubCategoryId", "CategoryName");

				return View(newproduct); 
            }

            var OldProduct= await _productRepository.GetByIdAsync(newproduct.Id);

  			OldProduct.Name = newproduct.Name;
			OldProduct.Quantity = newproduct.Quantity;
			OldProduct.Price = newproduct.Price;
             
			var discountValue = newproduct.discount ?? 0;

			OldProduct.DiscountPrice = discountValue != 0
			? newproduct.Price - (newproduct.Price * (discountValue / 100))
			: newproduct.Price;

			OldProduct.discount = discountValue;
             
			OldProduct.Description = newproduct.Description;
            OldProduct.CategoryId = newproduct.CategoryId;
			OldProduct.SubCategoryId = newproduct.SubCategoryId;

            if (newproduct.ClientFile != null)
            {
                bool result = saveImage.DeleteImage(OldProduct.ImageUrl);
                if(result)
				    OldProduct.ImageUrl =await saveImage.SaveImageAsync(newproduct.ClientFile); 
            }
            else 
            {
				OldProduct.ImageUrl= newproduct.ImageUrl;
            }
			 
            _productRepository.Update(OldProduct);
            await _productRepository.SaveAsync();
			if (User.IsInRole("Admin"))
				return RedirectToAction(nameof(Index));
			else
				return RedirectToAction("DashboardSeller", "Seller");           
        }

         


		[HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var foundImg = await _productRepository.GetByIdAsync(Id);

			if (foundImg == null)
				return NotFound();

			var result = await _productRepository.DeleteAsync(Id);
            if (result )
            {
                 var boole = saveImage.DeleteImage(foundImg.ImageUrl);

				return Ok();
			}
			return BadRequest(); 
		}
		 

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var product = await _repoProduct.GetProdById(id);

			if (product == null)
				return NotFound();

			var products = await _productRepository.GetAllWithIncludeAsync(p => p.ProductImages, p => p.Category,i=>i.Reviews,v=>v.Vendor,s=>s.SubCategory);
			var TopReviews = product.Reviews
				.OrderByDescending(r => r.Rating)  
				.ThenByDescending(r => r.CreatedAt)     
				.Take(4)                          
				.ToList();
			var model = new DetailsProductViewModel
			{
				product = product,
				listProduct = products,
                ProductId = id,
				AverageRating= product.AverageRating,
				TopReviews=TopReviews
			};

			return View(model);
		}



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(DetailsProductViewModel addview)
        {
            if (!ModelState.IsValid) 
            {
				addview.product = await _repoProduct.GetProdById(addview.ProductId);
				addview.listProduct = await _productRepository.GetAllWithIncludeAsync(p => p.ProductImages, p => p.Category);
				return View("Details",addview);
            }

		    if (!User.Identity.IsAuthenticated)
			return RedirectToAction("Login", "Account");

			var user = await _userManager.GetUserAsync(User);

            var existingReview =  await _repoProduct.GetExistingReviewAsync(addview.ProductId, user.Id);
         
			if (existingReview != null)
			{
 				existingReview.Rating = addview.Rating;
                existingReview.Comment = addview.Comment ?? GetDefaultComment(addview.Rating);
				existingReview.CreatedAt = DateTime.Now;
			}
			else
			{
				var review = new Review
				{
					ProductId = addview.ProductId,
					Rating = addview.Rating,
					Comment = addview.Comment ?? GetDefaultComment(addview.Rating),
					CustomerId = user.Id,
					CreatedAt = DateTime.Now
				};

				await _repoReview.AddAsync(review);
			}

			await _repoReview.SaveAsync();
			
            return RedirectToAction("Details", new { id = addview.ProductId });
        }
		private string GetDefaultComment(int rating)
		{
			return rating switch
			{
				1 => "Very Bad",
				2 => "Bad",
				3 => "Average",
				4 => "Very Good",
				5 => "Excellent",
				_ => "Average"
			};
		}




	}
}
