CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Players" (
    "Id" uuid NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "DateOfBirth" date NOT NULL,
    "CountryCode" character varying(2) NOT NULL,
    "CurrentRankingPoints" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Players" PRIMARY KEY ("Id")
);

CREATE TABLE "Venues" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "City" character varying(100) NOT NULL,
    "CountryCode" character varying(2) NOT NULL,
    "AddressLine" character varying(250) NOT NULL,
    "NumberOfTables" integer NOT NULL,
    "Capacity" integer NOT NULL,
    "TimeZoneId" character varying(100) NOT NULL,
    CONSTRAINT "PK_Venues" PRIMARY KEY ("Id")
);

CREATE TABLE "Tournaments" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "SeasonLabel" character varying(50) NOT NULL,
    "StartUtc" timestamp with time zone NOT NULL,
    "EndUtc" timestamp with time zone NOT NULL,
    "MaxPlayers" integer NOT NULL,
    "BestOfSets" integer NOT NULL,
    "OrganizerName" character varying(200) NOT NULL,
    "IsRankingEvent" boolean NOT NULL,
    "VenueId" uuid NOT NULL,
    CONSTRAINT "PK_Tournaments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tournaments_Venues_VenueId" FOREIGN KEY ("VenueId") REFERENCES "Venues" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Matches" (
    "Id" uuid NOT NULL,
    "TournamentId" uuid NOT NULL,
    "RoundNumber" integer NOT NULL,
    "TableNumber" integer NOT NULL,
    "ScheduledStartUtc" timestamp with time zone NOT NULL,
    "ActualStartUtc" timestamp with time zone,
    "CompletedUtc" timestamp with time zone,
    "Status" integer NOT NULL,
    "BestOfSets" integer NOT NULL,
    "WinnerPlayerId" uuid,
    CONSTRAINT "PK_Matches" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Matches_Players_WinnerPlayerId" FOREIGN KEY ("WinnerPlayerId") REFERENCES "Players" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Matches_Tournaments_TournamentId" FOREIGN KEY ("TournamentId") REFERENCES "Tournaments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Registrations" (
    "Id" uuid NOT NULL,
    "PlayerId" uuid NOT NULL,
    "TournamentId" uuid NOT NULL,
    "RegisteredUtc" timestamp with time zone NOT NULL,
    "SeedNumber" integer NOT NULL,
    "IsCheckedIn" boolean NOT NULL,
    CONSTRAINT "PK_Registrations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Registrations_Players_PlayerId" FOREIGN KEY ("PlayerId") REFERENCES "Players" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Registrations_Tournaments_TournamentId" FOREIGN KEY ("TournamentId") REFERENCES "Tournaments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "MatchParticipants" (
    "Id" uuid NOT NULL,
    "MatchId" uuid NOT NULL,
    "PlayerId" uuid NOT NULL,
    "Slot" integer NOT NULL,
    "ScoreSetsWon" integer NOT NULL,
    CONSTRAINT "PK_MatchParticipants" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MatchParticipants_Matches_MatchId" FOREIGN KEY ("MatchId") REFERENCES "Matches" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MatchParticipants_Players_PlayerId" FOREIGN KEY ("PlayerId") REFERENCES "Players" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "MatchSetResults" (
    "Id" uuid NOT NULL,
    "MatchId" uuid NOT NULL,
    "SetNumber" integer NOT NULL,
    "PlayerOnePoints" integer NOT NULL,
    "PlayerTwoPoints" integer NOT NULL,
    CONSTRAINT "PK_MatchSetResults" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MatchSetResults_Matches_MatchId" FOREIGN KEY ("MatchId") REFERENCES "Matches" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Matches_TournamentId" ON "Matches" ("TournamentId");

CREATE INDEX "IX_Matches_WinnerPlayerId" ON "Matches" ("WinnerPlayerId");

CREATE UNIQUE INDEX "IX_MatchParticipants_MatchId_PlayerId" ON "MatchParticipants" ("MatchId", "PlayerId");

CREATE UNIQUE INDEX "IX_MatchParticipants_MatchId_Slot" ON "MatchParticipants" ("MatchId", "Slot");

CREATE INDEX "IX_MatchParticipants_PlayerId" ON "MatchParticipants" ("PlayerId");

CREATE UNIQUE INDEX "IX_MatchSetResults_MatchId_SetNumber" ON "MatchSetResults" ("MatchId", "SetNumber");

CREATE UNIQUE INDEX "IX_Registrations_PlayerId_TournamentId" ON "Registrations" ("PlayerId", "TournamentId");

CREATE INDEX "IX_Registrations_TournamentId" ON "Registrations" ("TournamentId");

CREATE INDEX "IX_Tournaments_VenueId" ON "Tournaments" ("VenueId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260506233551_InitialCreate', '10.0.0-preview.5.25277.114');

COMMIT;

