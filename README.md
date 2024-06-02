# Easy to use library for generating data using EntityFrameworkCore

## Quickstart

Create custom class for model configuration and override ConfigureModel() method

```
public class TestSeederModel : SeederModel<TestContext>
{
    public override void ConfigureModel(SeederModelBuilder builder)
    {
    }
}
```

Add Seeding services to IServiceCollection:

```
builder.Services.AddSeeder<TestDbContext, TestSeederModel>();
```

For additional configuration use built-in options builder:

```
builder.Services.AddSeeder<TestContext, TestSeederModel>(options =>
    {
        options.InitialBootup();
        options.SaveDataAfterFinishing();
        options.OverrideExistingData();
    });
```

<code>InitialBootup()</code> method adds IHostedService to IServiceCollection which will execute seeding process automaticaly.

Configure your model using FuentAPI:

```
public class TestSeederModel : SeederModel<TestContext>
    {
        public override void ConfigureModel(SeederModelBuilder builder)
        {
            builder.Entity<TestEntity1>()
                .ShuffleValues();
            builder.Entity<TestEntity1>()
                .TimesCreated(30)
                .HasNotRequiredRelationshipProbability<TestEntity2>(0.5)
                .Property(x => x.Value)
                .HasRandomValues();
            builder.Entity<TestEntity1>()
                .Property(x => x.Value2)
                .HasValues(new List<string> { "1", "2", "3", "4" });
            builder.Entity<TestEntity1>()
                .Property(x => x.Value3)
                .HasRandomValues();
            builder.Entity<TestEntity2>()
                .TimesCreated(30);
            builder.Entity<TestEntity2>()
                .Property(x => x.Value)
                .HasRandomValues();
            builder.Entity<TestEntity2>()
                .Property(x => x.Value2)
                .HasRandomValues();
            builder.Entity<TestEntity3>()
                .TimesCreated(90);
            builder.Entity<TestEntity3>()
                .HasNumberOfConnectionsInManyToMany<TestEntity4>(20, 5);
            builder.Entity<TestEntity4>()
                .TimesCreated(100);
            builder.Entity<JoinEntity34>()
                .HasRandomValues();

        }
    }
```



<b> Seeding services do not support shadow primary key in a compatment with existing foreign key property in entities</b>

Now run your application and seeding services will build entities according to your model

## Dependency Injection

If you do not want to have initial bootup of seeding services, you can inject it into your services 
<b> 
    If you want to do so, turn off InitialBootup from seeder options by either removing the method or setting it to false: 
</b>
<code> 
    Initialbootup(false) 
</code>


```
public class MyService
{
    private readonly Seeder<TestSeederModel, TestContext> _seeder;
    public MyService(Seeder<TestSeederModel, TestContext> seeder)
    {
        _seeder = seeder;
    }
    public async Task Execute()
    {
        await _seeder.ExecuteSeedingAsync(CancellationToken.None);
    }
}
```
<c><code>Seeder<TSeederModel, TDbContext></code>instances have scoped lifetime</c>

## Options
Options are not static. You can change them in runtime if you called <code>AllowOptionsReconfiguringInRuntime()</code> method in Program.cs

```
var optionsProvider = serviceProvider.GetKeyedService<SeederOptionsProvider>(typeof(YourDbContextType));
optionsProvider.ReconfigureOptions()
    .OverrideExistingData(false)
    ...
```

SeederOptionsProvider instances have singleton lifetime 

<br>
RTU MIREA
<br>
IPTIP 
<br>
dp. of Perspective Technologies and Industrial Programming
<br>
gr. EFBO-01-22 
<br>
Dementev Egor Pavlovich (student number - 22T0305)
<br>

#### The library was completed as a course work on the subject of "software creation" 