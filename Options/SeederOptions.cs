using EntityFrameworkCore.Seeding.History;
using EntityFrameworkCore.Seeding.Logging;


namespace EntityFrameworkCore.Seeding.Options;

/// <summary>
/// Class containing options for certain seeder
/// </summary>
public class SeederOptions
{
    public bool OverrideExistingData { get; set; } = true;
    public bool SaveDataAfterFinishing { get; set; } = true;
    public bool CanIncreaseDataVolume { get; set; } = false;
    public bool HasVolumeIncreasingFunction { get; set; } = false;
    public bool HasInitialBootup { get; set; } = false;
    public Func<int, int>? VolumeIncreasingFunction { get; set; }
    public HistoryStoreTypes HistoryStorageLocation { get; set; } = HistoryStoreTypes.NoHistory;
    public bool HasLogger { get; set;} = false;
    public bool HasLoggerFactory { get; set; } = false;
    public SeederCommands CommandsLogged { get; set; } = SeederCommands.Seeding;
    public Type? LoggerType { get; set; }
    public bool ArtificialModelConfiguring { get; set; } = false;
    public string? GPTToken { get; set; }
    public bool AllowOptionsChanging { get; set; } = true;
}
