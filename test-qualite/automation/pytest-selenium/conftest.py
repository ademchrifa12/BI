import os
import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options


@pytest.fixture(scope="session")
def base_url():
    return os.getenv("APP_BASE_URL", "http://localhost:4200")


@pytest.fixture(scope="session")
def browser():
    options = Options()
    options.add_argument("--start-maximized")

    # Use headless mode in CI if needed
    if os.getenv("HEADLESS", "false").lower() == "true":
        options.add_argument("--headless=new")
        options.add_argument("--window-size=1920,1080")

    driver = webdriver.Chrome(options=options)
    driver.implicitly_wait(8)
    yield driver
    driver.quit()
