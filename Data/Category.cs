using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Note { get; set; }

    public virtual ICollection<Charm> Charms { get; set; } = new List<Charm>();
}
