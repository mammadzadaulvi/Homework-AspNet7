namespace PurpleBuzz_Backend.Helpers
{
    public class FileService : IFileService
    {
        public async Task<string> UploadAsync(IFormFile file, string webRootPath)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(webRootPath, "assets/img", fileName);
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }
        public void Delete(string fileName, string webRootPath)
        {
            string fname = Path.Combine(webRootPath, "assets/img", fileName);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                File.Delete(fname);
                file.Delete();
            }
        }

        public bool IsImage(IFormFile formFile)
        {
            if (!formFile.ContentType.Contains("image/"))
            {
                return false;
            }
            return true;
        }

        public bool CheckSize(IFormFile formFile, int maxSize)
        {
            if (formFile.Length / 1024 > 60)
            {
                return false;
            }
            return true;
        }

        
    }
}
