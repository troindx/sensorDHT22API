SELECT 'Starting database setup...';

-- Create the table for storing sensor readings
CREATE TABLE IF NOT EXISTS SensorReadings (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Time DATETIME NOT NULL,
    Temperature DOUBLE NOT NULL,
    Humidity DOUBLE NOT NULL
);

SELECT 'Database setup complete.';
