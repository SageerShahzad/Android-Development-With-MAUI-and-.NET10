using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Common.Entities;

public class Group(string name)
{
    [Key]
    public string Name { get; set; } = name;

    // nav properties
    public ICollection<Connection> Connections { get; set; } = [];
}
