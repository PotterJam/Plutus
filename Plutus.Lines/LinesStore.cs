using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Extensions;
using LiveLines.Api.Database;
using LiveLines.Api.Lines;
using LiveLines.Api.Users;

namespace LiveLines.Lines;

public class LinesStore : ILinesStore
{
    private readonly IDatabaseCommandExecutor _dbExecutor;

    public LinesStore(IDatabaseCommandExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<IEnumerable<Line>> GetLines(LoggedInUser loggedInUser)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@userid", loggedInUser.InternalId);

            cmd.CommandText = @"
                    SELECT l.id, l.body, l.date_for, s.spotify_id, l.privacy
                    FROM lines l
                    LEFT JOIN songs s on s.id = l.song_id
                    WHERE l.user_id = @userid;";

            var reader = await cmd.ExecuteReaderAsync();
            var lines = new List<Line>();
            
            while (await reader.ReadAsync())
            {
                var line = ReadLine(reader);
                lines.Add(line);
            }

            return lines;
        });
    }

    public async Task<IEnumerable<Line>> GetLinesWithPrivacy(LoggedInUser loggedInUser, LinePrivacy privacy)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@userId", loggedInUser.InternalId);
            cmd.AddEnumParam("@privacy", privacy);

            cmd.CommandText = @"
                    SELECT l.id, l.body, l.date_for, s.spotify_id, l.privacy
                    FROM lines l
                    LEFT JOIN songs s on s.id = l.song_id
                    WHERE l.user_id = @userid
                        AND l.privacy = @privacy;";

            var reader = await cmd.ExecuteReaderAsync();
            var lines = new List<Line>();
            
            while (await reader.ReadAsync())
            {
                var line = ReadLine(reader);
                lines.Add(line);
            }

            return lines;
        });
    }

    public async Task<Line> CreateLine(LoggedInUser loggedInUser, string body, Guid? songId, bool forYesterday, LinePrivacy privacy)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            var today = DateTime.UtcNow.Date;
            
            cmd.AddParam("@userid", loggedInUser.InternalId);
            cmd.AddParam("@body", body);
            cmd.AddParam("@songId", songId);
            cmd.AddParam("@dateFor", forYesterday ? today.AddDays(-1) : today);
            cmd.AddEnumParam("@privacy", privacy);

            cmd.CommandText = @"
                    INSERT INTO lines (user_id, body, song_id, date_for, privacy)
                    VALUES (@userid, @body, @songId, @dateFor, @privacy)
                    RETURNING id;";

            var guid = (Guid?) await cmd.ExecuteScalarAsync();

            if (guid == null)
                throw new LinesStoreException($"Tried to create line for user {loggedInUser.InternalId}, id not returned");

            return await GetLine(loggedInUser, guid.Value);
        });
    }

    private async Task<Line> GetLine(LoggedInUser loggedInUser, Guid lineId)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@lineid", lineId);
            cmd.AddParam("@userid", loggedInUser.InternalId);

            cmd.CommandText = @"
                    SELECT l.id, l.body, l.date_for, s.spotify_id, l.privacy
                    FROM lines l
                    LEFT JOIN songs s on s.id = l.song_id
                    WHERE l.id = @lineid
                        AND l.user_id = @userid
                    LIMIT 1;";

            var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new LinesStoreException($"Couldn't get line {lineId} for user {loggedInUser.InternalId}");

            return ReadLine(reader);
        });
    }

    public async Task<IEnumerable<DateTime>> GetLatestLineDates(LoggedInUser loggedInUser, int limit)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@limit", limit);
            cmd.AddParam("@userId", loggedInUser.InternalId);

            cmd.CommandText = @"
                    SELECT date_for
                    FROM lines
                    WHERE user_id = @userId
                    ORDER BY date_for DESC
                    LIMIT @limit;";

            var reader = await cmd.ExecuteReaderAsync();
            
            var dates = new List<DateTime>();
                
            while (await reader.ReadAsync())
            {
                var date = reader.Get<DateTime>("date_for");
                dates.Add(date);
            }

            return dates;
        });
    }

    private Line ReadLine(DbDataReader reader)
    {
        var id = reader.Get<Guid>("id");
        var body = reader.Get<string>("body");
        var dateFor = reader.Get<DateTime>("date_for");
        var spotifyId = reader.GetNullable<string?>("spotify_id"); 
        var privacy = reader.GetEnum<LinePrivacy>("privacy");
        
        return new Line(id, body, spotifyId, dateFor, privacy);
    }
}