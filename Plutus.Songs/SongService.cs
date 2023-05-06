using LiveLines.Api.Songs;

namespace LiveLines.Songs;

public class SongService : ISongService
{
    private readonly ISongStore _songStore;

    public SongService(ISongStore songStore)
    {
        _songStore = songStore;
    }
    
    public async Task<Guid> AddSong(string spotifySongId)
    {
        return await _songStore.AddSong(spotifySongId);
    }
}