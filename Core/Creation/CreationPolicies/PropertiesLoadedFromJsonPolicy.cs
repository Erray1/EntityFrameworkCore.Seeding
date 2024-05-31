using EntityFrameworkCore.Seeding.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core.Creation.CreationPolicies;
public class PropertiesLoadedFromJsonPolicy : SeederPropertiesCreationPolicy
{
    protected override void createPropertiesPool()
    {
        string jsonString = loadJsonString();
    }
    protected virtual string loadJsonString()
    {
        var jsonInfo = _propertiesFilledWithPolicy.First().JsonInfo;
        var jsonString = File.ReadAllText(jsonInfo!.JsonAbsolutePath);
        return jsonString;
    }
    protected Dictionary<SeederPropertyInfo, List<object>> deserializeJson(string jsonString, JsonSerializerOptions? options = null)
    {
        throw new NotImplementedException();
    }
    protected void fillPropertiesPool()
    {

    }

}

