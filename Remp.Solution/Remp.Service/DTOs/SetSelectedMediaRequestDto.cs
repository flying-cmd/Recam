namespace Remp.Service.DTOs;

public class SetSelectedMediaRequestDto
{
    public IEnumerable<int> MediaIds { get; set; } = new List<int>();
}
