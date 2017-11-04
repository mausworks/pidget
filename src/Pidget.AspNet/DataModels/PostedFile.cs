using Microsoft.AspNetCore.Http;

namespace Pidget.AspNet.DataModels
{
    public class PostedFile
    {
        public string FileName { get; }

        public long Length { get; }

        public string ContentType { get; }

        public PostedFile(string name,
            string contentType,
            long length)
        {
            FileName = name;
            Length = length;
            ContentType = contentType;
        }

        public static PostedFile FromFormFile(IFormFile formFile)
            => new PostedFile(
                name: formFile.Name,
                contentType: formFile.ContentType,
                length: formFile.Length);
    }
}
