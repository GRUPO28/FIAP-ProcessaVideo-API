using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Application.Abstractions;

public interface IVideoUploadService
{
    Task<string> UploadVideoAsync(Stream videoStream, string fileName);
    Task<bool> VideoExistsAsync(string fileName);
}
