using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Modelling.Utilities;
public class EntityCreatedFromJsonInfo
{
    private bool partialCreation = true;
    public bool PartialCreation { 
        get 
        {
            return partialCreation; 
        } 
        set 
        { 
            if (partialCreation && value)
            {
                throw new InvalidOperationException("Cannot partially specify properties of entity created from json after using WithAllProperties() method")
            };
            partialCreation = value;
        } 
    }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public Dictionary<SeederPropertyInfo, string> JSONNamesAndProperties = new();
    public string JsonAbsolutePath { get; set; }
    public EntityCreatedFromJsonInfo(
        string jsonAbsolutePath,
        JsonSerializerOptions options)
    {
        JsonAbsolutePath  = jsonAbsolutePath;
        JsonSerializerOptions = options;
    }
}

