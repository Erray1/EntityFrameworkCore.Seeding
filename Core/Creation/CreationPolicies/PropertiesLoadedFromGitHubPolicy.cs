using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public class PropertiesLoadedFromGitHubPolicy : SeederPropertiesCreationPolicy
{
    private readonly HttpClient _httpClient;
    public PropertiesLoadedFromGitHubPolicy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    protected override void createPropertiesPool()
    {
        var stockDataEntityCollection = _propertiesFilledWithPolicy.First()
            .PropertyStockCollection!
            .ParentEntityCollection;
        string downloadLink = stockDataEntityCollection.DownloadLink;
        var responseTask = _httpClient.GetAsync(downloadLink);
        responseTask.Wait();
        var response = responseTask.Result;

        string stringContent = response.Content.ReadAsStringAsync().Result;

        
    }

}

