using ItalianCharmBracelet.Data;

namespace ItalianCharmBracelet.ViewModels
{
    public class InfoProductVM
    {
        public string CharmId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Img { get; set; }
        public double Discount { get; set; }
        public string? Description { get; set; }
        public string? CateId { get; set; }
        public string CateName { get; set; }

        public InfoProductVM() { }
        public InfoProductVM(string CharmId, string Name, double Price, string Img, string CateId, string CateName)
        {
            this.CharmId = CharmId;
            this.Name = Name;
            this.Price = Price;
            this.Img = Img;
            this.CateId = CateId;
            this.CateName = CateName;
        }

        public InfoProductVM(string CharmId, string Name, int Quantity, double Price, string Img, string Description, string CateId, string CateName)
        {
            this.CharmId = CharmId;
            this.Name = Name;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Img = Img;
            this.Description = Description;
            this.CateId = CateId;
            this.CateName = CateName;
        }

    }
}
