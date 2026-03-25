-- ============================================================
-- ERP Users — Initial Schema
-- Run once to create the database objects.
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";   -- gen_random_uuid()

CREATE TABLE IF NOT EXISTS users (
    id         UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    name       VARCHAR(100)  NOT NULL,
    email      VARCHAR(200)  NOT NULL,
    role       VARCHAR(50)   NOT NULL,
    is_active  BOOLEAN       NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ   NULL,
    CONSTRAINT uq_users_email UNIQUE (email)
);

COMMENT ON TABLE  users              IS 'ERP system users managed via the REST API.';
COMMENT ON COLUMN users.is_active    IS 'Soft-disable flag — records are never hard-deleted automatically.';
COMMENT ON COLUMN users.updated_at   IS 'NULL until the record has been updated at least once.';
