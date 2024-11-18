using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class State
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
