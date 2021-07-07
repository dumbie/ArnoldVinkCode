using System;
using System.IO;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public interface AVImages
    {
        //Download and resize image
        Task<Stream> DownloadResizeImage(Uri downloadUri, uint maxWidth, uint maxHeight);
    }
}