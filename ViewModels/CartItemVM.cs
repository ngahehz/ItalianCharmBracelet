namespace ItalianCharmBracelet.ViewModels
{
    public class CartItemVM
    {
        public string CharmId { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? CateId { get; set; }
        public double Total => Quantity * Price;
    }
}
