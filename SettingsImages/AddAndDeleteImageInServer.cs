 namespace MegaStor.AddImge
{
    public class AddAndDeleteImageInServer 
    {
        private readonly IWebHostEnvironment host;

        public AddAndDeleteImageInServer(IWebHostEnvironment _host)
        {
           host = _host;
        }



        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile.Length > 0)
            {
                 string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                string folderPath = Path.Combine(host.WebRootPath, "img"); //wwwroot/img
                string fullPath = Path.Combine(folderPath, fileName);      //wwwroot/img/اسم_الملف.jpg

                 if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                 return fileName;
            }

            return null;
        }
 

            

		public bool DeleteImage(string fileName)
		{
 			string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

 			string fullPath = Path.Combine(folderPath, fileName);

 			if (System.IO.File.Exists(fullPath))
			{

				System.IO.File.Delete(fullPath);  

				return true  ;
			}
			else
			{
				return false ;
			}
		}












         
        public bool DeleteImagewwww(string imageName)
        {
            // التأكد أن الصورة ليست صورة افتراضية
            if (!string.IsNullOrEmpty(imageName) && imageName != "DefultImage.jpg")
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagSubCategory", imageName);

                // التأكد من وجود الصورة
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        // حذف الصورة
                        System.IO.File.Delete(imagePath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // يمكنك التعامل مع الخطأ هنا
                        Console.WriteLine("Error deleting image: " + ex.Message);
                        return false;
                    }
                }
            }
            return false;
        }









    }
}
