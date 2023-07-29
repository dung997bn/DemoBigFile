namespace DemoBigFile.Models.RelationalModels
{
    public class ProductsVariantsEntity
    {
        public long id { get; set; }
        public long? product_id { get; set; }
        public string variant_name { get; set; }
        public string reference_id { get; set; }
        public string variant_reference_id { get; set; }
    }
}
