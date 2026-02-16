#!/bin/bash
# wait-for-it.sh: wait for a TCP service to be ready
# Usage: ./wait-for-it.sh host port [-- command args]

set -e

host="$1"
port="$2"
shift 2
cmd="$@"

# Wait for the service
echo "Waiting for $host:$port..."
timeout=60
while [ $timeout -gt 0 ]; do
  if nc -z "$host" "$port" 2>/dev/null; then
    echo "$host:$port is available!"
    break
  fi
  
  echo "Waiting... ($timeout seconds remaining)"
  timeout=$((timeout - 1))
  sleep 1
done

if [ $timeout -eq 0 ]; then
  echo "Timeout waiting for $host:$port"
  exit 1
fi

# Give SQL Server a moment to fully initialize
echo "Waiting 10 more seconds for full initialization..."
sleep 10

# Execute the command
echo "Starting application..."
exec $cmd
