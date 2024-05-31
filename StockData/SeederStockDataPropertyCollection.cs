namespace EntityFrameworkCore.Seeding.StockData;

public class SeederStockDataPropertyCollection
{
    public Guid ID { get; set; }
    public string PropertyName { get; set; }
    public Type PropertyType { get; set; }
    public SeederStockDataEntityCollection ParentEntityCollection { get; set; }

}