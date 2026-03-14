import os
import time
import pytest

from pages.login_page import LoginPage
from pages.customers_page import CustomersPage


@pytest.mark.e2e
def test_add_customer_and_verify_then_cleanup(browser, base_url):
    username = os.getenv("TEST_USER_EMAIL")
    password = os.getenv("TEST_USER_PASSWORD")

    if not username or not password:
        pytest.skip("TEST_USER_EMAIL and TEST_USER_PASSWORD are required for add-customer test")

    unique_suffix = int(time.time())
    customer_name = f"AUTO_CUST_{unique_suffix}"
    customer_email = f"auto{unique_suffix}@example.com"

    login_page = LoginPage(browser, base_url)
    login_page.open()
    login_page.login_and_wait_success(username, password)

    customers_page = CustomersPage(browser, base_url)
    customers_page.open()

    assert customers_page.can_add_customer(), (
        "Add Customer button is not visible. Ensure test account has Admin role."
    )

    customers_page.open_add_modal()
    customers_page.create_customer(
        customer_name=customer_name,
        phone="555-1000",
        email=customer_email,
    )

    # Hosted environment can be slower; reload customers page before search to avoid stale UI states.
    customers_page.open()
    customers_page.search(customer_name)
    assert customers_page.is_customer_present(customer_name), (
        f"Customer '{customer_name}' was not found after creation. Current URL: {browser.current_url}"
    )

    deleted = customers_page.delete_customer_if_present(customer_name)
    assert deleted, f"Customer '{customer_name}' cleanup deletion failed"

    customers_page.search(customer_name)
    assert not customers_page.is_customer_present(customer_name), (
        f"Customer '{customer_name}' still appears after delete. Current URL: {browser.current_url}"
    )
