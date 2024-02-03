#!/usr/bin/env bash

until printf "" 2>>/dev/null >>/dev/tcp/cassandra/9042; do 
    sleep 5;
    echo "Waiting for cassandra...";
done

echo "Creating keyspaces..."
cqlsh cassandra -e "CREATE KEYSPACE IF NOT EXISTS process_manager WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};"
cqlsh cassandra -e "CREATE KEYSPACE IF NOT EXISTS single_current_aggregate WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};"
cqlsh cassandra -e "CREATE KEYSPACE IF NOT EXISTS two_phase_commit WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};"
cqlsh cassandra -e "CREATE KEYSPACE IF NOT EXISTS pending_creation WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};"
echo "Keyspaces created."
