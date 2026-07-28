"""
Smoke test for the InventoryApi container.
 
Waits for the API to come up, then exercises a couple of real endpoints
(GET /api/categories, POST /api/categories, GET /api/items) to confirm the
built Docker image actually serves working requests — not just that it
compiles and starts.
 
Exits with a non-zero status code on any failure, so CI can fail the job.
"""

import sys
import time
import requests
 
BASE_URL = "http://localhost:8080"
MAX_RETRIES = 15
RETRY_DELAY_SECONDS = 2
 
 
def wait_for_api():
    """Poll the API until it responds, or give up after MAX_RETRIES."""
    for attempt in range(1, MAX_RETRIES + 1):
        try:
            response = requests.get(f"{BASE_URL}/api/categories", timeout=3)
            if response.status_code == 200:
                print(f"API is up (attempt {attempt}).")
                return
        except requests.exceptions.ConnectionError:
            pass
 
        print(f"Waiting for API... (attempt {attempt}/{MAX_RETRIES})")
        time.sleep(RETRY_DELAY_SECONDS)
 
    print("API did not become ready in time.")
    sys.exit(1)
 
 
def check(condition: bool, message: str):
    """Fail loudly and exit if a check doesn't pass."""
    if not condition:
        print(f"FAILED: {message}")
        sys.exit(1)
    print(f"OK: {message}")
 
 
def run_smoke_test():
    wait_for_api()
 
    # 1. GET /api/categories should return 200 with a JSON array
    response = requests.get(f"{BASE_URL}/api/categories", timeout=5)
    check(response.status_code == 200, "GET /api/categories returns 200")
    check(isinstance(response.json(), list), "GET /api/categories returns a JSON array")
 
    # 2. POST /api/categories should create a category and return 201
    payload = {"name": "SmokeTestCategory"}
    response = requests.post(f"{BASE_URL}/api/categories", json=payload, timeout=5)
    check(response.status_code == 201, "POST /api/categories returns 201")
    created = response.json()
    check(created.get("name") == "SmokeTestCategory", "created category has the expected name")
 
    # 3. GET /api/items should return 200 (even if empty)
    response = requests.get(f"{BASE_URL}/api/items", timeout=5)
    check(response.status_code == 200, "GET /api/items returns 200")
 
    print("\nAll smoke tests passed.")
 
 
if __name__ == "__main__":
    run_smoke_test()