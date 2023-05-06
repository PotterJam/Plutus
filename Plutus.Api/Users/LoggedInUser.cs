using System;

namespace Plutus.Api.Users;

public record LoggedInUser(Guid InternalId, string Username);