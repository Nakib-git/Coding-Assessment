-- ============================================================
-- ERP Users — Sample Seed Data (development only)
-- ============================================================

INSERT INTO users (name, email, role, is_active) VALUES
  ('Rahim Hamza',    'rahim@erp.local',   'Admin',    TRUE),
  ('Karim Hamza',     'karim@erp.local',    'Manager',  TRUE),
  ('Rasid Hamza',      'rasid@erp.local',     'User',     TRUE),
  ('Nur Hamza',      'nur@erp.local',     'User',     FALSE),
  ('Azad Hamza',        'azad@erp.local',       'Manager',  TRUE),
  ('Rafiq Hamza',     'rafiq@erp.local',    'User',     TRUE),
  ('Kabir Hamza',        'kabir@erp.local',       'Admin',    TRUE),
  ('Nusrat Hamza',      'nusrat@erp.local',     'User',     FALSE),
  ('Rasel Hamza',       'rasel@erp.local',      'User',     TRUE),
  ('Jabbar Hamza',      'jabbar@erp.local',     'Manager',  TRUE)
ON CONFLICT (email) DO NOTHING;
