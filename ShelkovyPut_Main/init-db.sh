#!/bin/bash

# Wait for SQL Server to start
sleep 30s

# Run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P nguyenquocbinh214@BB -d master -i /docker-entrypoint-initdb.d/init-db.sql

# Wait for DB creation to complete
sleep 10s
