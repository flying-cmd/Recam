using Microsoft.AspNetCore.Http;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class CreateMediaRequestDto
{
    public IEnumerable<IFormFile> MediaFiles { get; set; } = null!;
    public string MediaType { get; set; } = null!;
}
