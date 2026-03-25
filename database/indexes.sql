-- ============================================================
-- ERP Users — Query Optimisation Indexes
-- Run AFTER init.sql.  All indexes are created only if absent.
-- ============================================================

-- ── 1. Trigram index for ILIKE search on name & email ────────────────────────
--  Enables: WHERE name ILIKE '%john%' OR email ILIKE '%john%'
--  Requires the pg_trgm extension (ships with standard PostgreSQL).
CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE INDEX IF NOT EXISTS ix_users_name_trgm
    ON users USING gin (name gin_trgm_ops);

CREATE INDEX IF NOT EXISTS ix_users_email_trgm
    ON users USING gin (email gin_trgm_ops);

-- ── 2. Partial index — active users only (most common filter) ────────────────
--  Enables: WHERE is_active = TRUE  (very small, fast scan)
CREATE INDEX IF NOT EXISTS ix_users_is_active_true
    ON users (created_at DESC)
    WHERE is_active = TRUE;

-- ── 3. Full is_active index — also covers FALSE lookups ──────────────────────
CREATE INDEX IF NOT EXISTS ix_users_is_active
    ON users (is_active);

-- ── 4. Composite index for sorted paginated list ─────────────────────────────
--  Enables: ORDER BY name  + optional is_active filter without extra sort step
CREATE INDEX IF NOT EXISTS ix_users_is_active_name
    ON users (is_active, name);

-- ── 5. Covering index for the ADO.NET COUNT query ────────────────────────────
--  Avoids heap fetches when filtering only on is_active (index-only scan)
CREATE INDEX IF NOT EXISTS ix_users_covering_count
    ON users (is_active) INCLUDE (name, email);

-- ── Verify ───────────────────────────────────────────────────────────────────
SELECT indexname, indexdef
FROM   pg_indexes
WHERE  tablename = 'users'
ORDER  BY indexname;
