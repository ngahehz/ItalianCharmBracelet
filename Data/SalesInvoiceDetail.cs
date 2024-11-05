using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class SalesInvoiceDetail
{
    public string InvoiceId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public string? Note { get; set; }

    public virtual SalesInvoice Invoice { get; set; } = null!;

    public virtual Charm Product { get; set; } = null!;
}
