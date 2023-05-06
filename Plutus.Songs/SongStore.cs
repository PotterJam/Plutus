using Extensions;
using LiveLines.Api.Database;
using LiveLines.Api.Songs;

namespace LiveLines.Songs;

public class SongStore : ISongStore
{
    private readonly IDatabaseCommandExecutor _db;

    public SongStore(IDatabaseCommandExecutor db)
    {
        _db = db;
    }

    public async Task<Guid> AddSong(string spotifySongId)
    {
        return await _db.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@spotifySongId", spotifySongId);

            cmd.CommandText = @"
                    WITH new_song AS (
                        INSERT INTO songs (spotify_id)
                        VALUES (@spotifySongId)
                        ON CONFLICT(spotify_id) DO NOTHING
                        RETURNING id
                    ) SELECT COALESCE(
                        (SELECT id FROM new_song),
                        (SELECT id FROM songs WHERE spotify_id = @spotifySongId)
                    ) AS id;";

            var songId = (Guid?) await cmd.ExecuteScalarAsync();

            if (songId == null)
                throw new SongStoreException("Tried to add a song, nothing got returned");

            return songId.Value;
        });
    }
}