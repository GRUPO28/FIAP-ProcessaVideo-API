using FIAP_ProcessaVideo_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Domain.Abstractions;

public interface IVideoRepository
{
    Task<List<Video>> GetByUser(string idUsuario);
    Task<Video> GetById(string id);
    Task<bool> CreateAsync(Video video);
    Task<bool> DeleteAsync(string id);
}
