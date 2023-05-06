-- DB MIGRATIONS FILE
-- DON'T CHANGE, ONLY APPEND

CREATE EXTENSION "uuid-ossp";

CREATE ROLE dev;
GRANT CONNECT ON DATABASE plutus TO dev;

CREATE TABLE users
(
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    provider TEXT NOT NULL,
    username TEXT UNIQUE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    last_active TIMESTAMP WITH TIME ZONE NULL DEFAULT NULL
);

CREATE INDEX users_username_idx ON users (username);

GRANT UPDATE, INSERT, SELECT ON TABLE users TO dev;

-- above is initial commit