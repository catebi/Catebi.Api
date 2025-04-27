
-- file_storage
CREATE TABLE IF NOT EXISTS ctb.file_storage (
    file_storage_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    file_name TEXT NOT NULL,
    content_type TEXT NOT NULL,
    content BYTEA NOT NULL,
    size BIGINT NOT NULL,
    created timestamp default timezone('utc', now())
);

COMMENT ON TABLE file_storage IS 'Stores file data with metadata including original filename, content type, and upload timestamp';

COMMENT ON COLUMN file_storage.file_storage_id IS 'Unique identifier for the file, automatically generated using gen_random_uuid()';
COMMENT ON COLUMN file_storage.file_name IS 'Original filename of the uploaded file';
COMMENT ON COLUMN file_storage.content_type IS 'MIME type of the file (e.g., image/jpeg, application/pdf)';
COMMENT ON COLUMN file_storage.content IS 'Binary data of the file stored as BYTEA';
COMMENT ON COLUMN file_storage.size IS 'Size of the file in bytes';
COMMENT ON COLUMN file_storage.created IS 'Timestamp when the file was uploaded, automatically set to current time';
