---
name: entity-framework
description: Knowledge and best-practices for using Entity Framework Core in this project: schema changes, migrations, relationship modeling, data migrations and safe refactors.
---

Purpose
-------

This skill encapsulates expertise for making accurate, safe, and well-reasoned changes to the application's Entity Framework Core model and the underlying database schema. It helps agents and contributors:

- Identify which domain models, relationships, and constraints must change to implement a feature.
- Design schema changes that preserve data integrity and support migrations.
- Create and review EF Core migrations and accompanying data migration scripts.
- Evaluate and implement relationship changes (FKs, navigation properties, cascade rules) with minimal disruption.

When to use
-----------

- Adding, renaming, or removing model properties that map to columns.
- Changing cardinality or navigation properties between entities (one-to-many, many-to-many, optional/required).
- Introducing new tables or consolidating existing ones.
- Planning data migrations that transform existing rows to the new schema.
- Reviewing migrations for performance, indexing, and referential integrity.

Keywords
--------

EF Core, migration, schema, foreign key, relationship, navigation property, cascade delete, data migration, migration script, rollback, transaction, index, performance, seeding

Best practices & checklist
-------------------------

1. Model changes first: update C# domain models and navigation properties to express the desired shape.
2. Generate a migration: use `dotnet ef migrations add <Name>` in the correct project (`--project`/`--startup-project` as needed).
3. Inspect the migration: review `Up()` and `Down()` for destructive operations, renames vs drops, and SQL accuracy.
4. Add data migrations where needed: prefer explicit `Sql()` or `migrationBuilder` calls to transform rows safely.
5. Preserve IDs and FK relationships: when renaming columns/tables, use `RenameColumn`/`RenameTable` or raw SQL to avoid data loss.
6. Use transactions for multi-step migrations that cannot be expressed atomically by a single SQL DDL command.
7. Add or adjust indexes for query patterns altered by schema changes.
8. Run migrations on a staging copy of production data to observe runtime and correctness before applying to production.
9. Provide a rollback path: ensure `Down()` reverses changes or document manual rollback steps for complex data transformations.
10. Update seed data and repository/tests to align with the new model.

Common relation-change patterns
-----------------------------

- Optional -> Required FK: add a non-nullable FK with default values or backfill data, then alter column to non-nullable.
- Split a table into two: create new table, migrate rows with transaction, update FKs, then drop old columns.
- Introduce many-to-many: introduce join entity or use EF Core many-to-many with explicit join when extra attributes are needed.
- Rename property vs drop/add: prefer `RenameColumn`/`RenameIndex` to preserve history and avoid copying data.

Testing & verification
----------------------

- Run `dotnet ef database update` against a local/staging database and run integration tests.
- Validate referential integrity and sample queries (counts, joins) after migration.
- Check migration SQL for expensive operations (table scans, full rebuilds) and add batching/backfill strategies.

Safety notes
------------

- Avoid destructive migrations during peak traffic for production systems; schedule maintenance windows if required.
- Large data backfills should be performed out-of-band or in small batches, with progress and verification steps.
- When uncertain, prefer adding nullable columns and a compatibility layer in application code, then backfill and switch.

Examples (short)
----------------

- Make an FK required:

	1. Add property to model (e.g., `public Guid TournamentId { get; set; }`).
	2. Create migration, add a backfill step in `Up()` to populate missing values.
	3. Alter column to non-nullable.

- Rename `Player.FullName` to `FirstName`/`LastName` safely:

	1. Add `FirstName`/`LastName` to model while keeping `FullName` (mapped or copied).
	2. Create migration that copies values from `FullName` into the two new columns (SQL or C# data migration).
	3. Remove `FullName` in a later migration after verification.

Where to find help
------------------

- Project `TableTennisTracker.Domain` models are the source of truth. See `TableTennisTracker.Domain/Models`.
- Always review generated migrations under `TableTennisTracker.Web/Migrations` (or configured migrations folder).
- If a change affects public APIs or routes, review controllers and views in `TableTennisTracker.Web/Controllers` and `Views`.

This skill is intended to guide automated agents and human contributors to make database and model changes with more confidence and fewer regressions. It focuses on safe migrations, preserving data, and clear verification steps.