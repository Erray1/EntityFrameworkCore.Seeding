using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.StockData;
public class SeederStockDataCollection
{
    public virtual string DownloadLink { get; set; }
    public void MarkEveryPropertyMappedToCollectionAsConfigured(IEnumerable<SeederPropertyInfo> properties, bool strictPropertyNameMatching)
    {
        
    }
}
