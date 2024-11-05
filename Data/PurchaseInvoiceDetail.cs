using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class PurchaseInvoiceDetail
{
    public string InvoiceId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public string Link { get; set; } = null!;

    public string? Note { get; set; }

    public virtual PurchaseInvoice Invoice { get; set; } = null!;

    public virtual Charm Product { get; set; } = null!;
}
